﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Discord;

using DiscordDkpBot.Configuration;
using DiscordDkpBot.Dkp;
using DiscordDkpBot.Dkp.EqDkpPlus.Xml;
using DiscordDkpBot.Extensions;
using DiscordDkpBot.Items;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Auctions
{
	public class AuctionProcessor : IAuctionProcessor
	{
		private readonly DkpBotConfiguration configuration;
		private readonly IDkpProcessor dkpProcessor;
		private readonly IItemProcessor itemProcessor;
		private readonly ILogger<AuctionProcessor> log;
		private readonly Random random = new Random();
		private readonly Dictionary<string, RankConfiguration> ranks;
		private readonly AuctionState state;

		public AuctionProcessor (DkpBotConfiguration configuration, AuctionState state, IItemProcessor itemProcessor, IDkpProcessor dkpProcessor, ILogger<AuctionProcessor> log)
		{
			ranks = configuration.ExpandedRanks;
			this.configuration = configuration;
			this.state = state;
			this.itemProcessor = itemProcessor;
			this.dkpProcessor = dkpProcessor;
			this.log = log;
		}

		public async Task<AuctionBid> AddOrUpdateBid (string item, string character, string rank, int bid, IMessage message)
		{
			// First make sure we can make a valid bid out of it.
			if (!ranks.TryGetValue(rank, out RankConfiguration rankConfig))
			{
				throw new ArgumentException($"Rank {rank} does not exist.");
			}

			if (!state.Auctions.TryGetValue(item, out Auction auction))
			{
				throw new AuctionNotFoundException(item);
			}

			int characterId = await dkpProcessor.GetCharacterId(character);
			PlayerPoints points = await dkpProcessor.GetDkp(characterId);
			int pointsAlreadyBid = state.Auctions.Values.SelectMany(x => x.Bids)
				.Where(b => b.CharacterId == characterId // From This character
							&& b.Auction != auction) // Don't count this current auction, because we're replacing it.
				.Sum(x => x.BidAmount) * rankConfig.PriceMultiplier;
			decimal availableDkp = (points.PointsCurrentWithTwink - pointsAlreadyBid) / rankConfig.PriceMultiplier;

			if (availableDkp < bid)
			{
				throw new InsufficientDkpException($"{character} only has {availableDkp} left to bid with. Cancel some bids, or bid less!");
			}

			AuctionBid newBid = auction.Bids.AddOrUpdate(new AuctionBid(auction, character, characterId, bid, rankConfig, message.Author));

			log.LogInformation($"Created bid: {newBid}");

			await message.Channel.SendMessageAsync($"Bid accepted for **{newBid.Auction}**\n"
				+ $"```\"{auction.Name}\" {newBid}```"
				+ $"If you win, you could pay up to **{newBid.BidAmount * newBid.Rank.PriceMultiplier}**.\n"
				+ $"If you wish to modify your bid before the auction completes, simply enter a new bid in the next **{auction.MinutesRemaining:##.#}** minutes.\n"
				+ "If you wish to cancel your bid use the following syntax:\n"
				+ $"```\"{newBid.Auction.Name}\" cancel```");

			return newBid;
		}

		public CompletedAuction CalculateWinners (Auction auction)
		{
			List<AuctionBid> bids = auction.Bids.ToList();
			log.LogTrace("Finding winners for {0} from bids submitted: ({1})", auction.DetailDescription, string.Join("', ", auction.Bids));
			List<AuctionBid> winners = new List<AuctionBid>();

			for (int i = 0; i < auction.Quantity; i++)
			{
				if (bids.Any())
				{
					// Grab the first winner.
					AuctionBid winner = RemoveWinner(bids);
					winners.Add(winner);
				}
			}

			List<WinningBid> winningBids = CalculatePrices(winners, bids);

			log.LogInformation("{0} found {1} winners: {2}", auction.DetailDescription, winners.Count, string.Join(", ", winners));

			return new CompletedAuction(auction, winningBids);
		}

		public async Task<Auction> CancelAuction (string name, IMessage message)
		{
			if (!state.Auctions.TryRemove(name, out Auction auction))
			{
				throw new AuctionNotFoundException($"Auction for {name} does not exist.");
			}
			auction.Stop();
			string cancelMessage = auction.CancelledText;

			await auction.Channel.SendMessageAsync(cancelMessage);

			if (message.Channel.Id != auction.Channel.Id)
			{
				await message.Channel.SendMessageAsync(cancelMessage);
			}

			log.LogTrace(cancelMessage);

			return auction;
		}

		public Task<AuctionBid> CancelBid (string item, IMessage message)
		{
			if (!state.Auctions.TryGetValue(item, out Auction auction))
			{
				throw new AuctionNotFoundException(item);
			}

			if (!auction.Bids.TryRemove(message.Author.Id, out AuctionBid bid))
			{
				throw new BidNotFoundException(item);
			}

			message.Channel.SendMessageAsync($"Bid cancelled for **{auction}**.\nYou have **{auction.MinutesRemaining:##.#}** minutes to re bid.");

			return Task.FromResult(bid);
		}

		public async Task<Auction> StartAuction (int? quantity, string name, int? minutes, IMessage message)
		{
			RaidInfo raid = await dkpProcessor.GetDailyItemsRaid();
			Auction auction = new Auction(state.NextAuctionId, quantity ?? 1, name, minutes ?? configuration.DefaultAuctionDurationMinutes, raid, message);
			if (!state.Auctions.TryAdd(auction.Name, auction))
			{
				throw new AuctionAlreadyExistsException($"Auction for {auction.Name} already exists.");
			}
			try
			{
				ItemLookupResult itemResult = await itemProcessor.GetItemEmbed(name);
				string announcementText = auction.GetAnnouncementText(configuration.Ranks);
				if (itemResult?.MatchesFound > 1)
				{
					announcementText += "There were several items found with the same name, this might not actually be the correct item stats.";
				}

				IUserMessage announcement = await auction.Channel.SendMessageAsync(announcementText, false, itemResult?.Embed);

				auction.Tick += async (o, s) => await announcement.ModifyAsync(m => m.Content = auction.GetAnnouncementText(configuration.Ranks));
				auction.Completed += async (o, s) =>
											{
												await announcement.ModifyAsync(m => m.Content = auction.ClosedText);
												await FinishAuction(auction, announcement);
											};

				auction.Start();

				log.LogTrace("Started auction: {0}", auction.DetailDescription);

				return auction;
			}
			catch (Exception)
			{
				state.Auctions.TryRemove(auction.Name, out Auction _);
				throw;
			}
		}

		private List<WinningBid> CalculatePrices (ICollection<AuctionBid> winners, ICollection<AuctionBid> losers)
		{
			List<WinningBid> winningBids = new List<WinningBid>(winners.Count);
			
			int winnerNumber = 1;
			foreach (AuctionBid winner in winners.OrderBy(x => x))
			{
				int applicableLooserBid = 0;
				bool loserTied = false;
				if (losers.Any())
				{
					AuctionBid loser = null;

					// check and see if we won because we out-bid somebody's cap.
					AuctionBid outbidWinner = winners.OrderBy(x => x).Skip(winners.Count).FirstOrDefault();

					if (outbidWinner != null && outbidWinner.MaxBid < winner.BidAmount)
					{
						// If we outbid their cap, we'll use them to calculate our price instead of the first unsuccessful bid.
						loser = outbidWinner;
					}

					if (loser == null)
					{
						// You lose! Good DAY sir!
						loser = losers.OrderBy(x=>x).FirstOrDefault();

						//If the loser tied a winner, that adjusts the final price.
						//But only if they didn't outbid their cap'
						if (loser?.BidAmount == winners.LastOrDefault()?.BidAmount
						&& loser.BidAmount <= loser.MaxBid)
						{
							loserTied = true;
						}
					}

					if (loser != null)
					{
						if (winner.Rank.MaxBid > loser.Rank.MaxBid && winner.BidAmount > loser.Rank.MaxBid)
						{
							// If our bid cap and is higher than their bid cap, and we bid over their cap. reduce their bid.
							applicableLooserBid = Math.Min(loser.BidAmount, loser.Rank.MaxBid);
						}
						else
						{
							// Otherwise
							applicableLooserBid = loser.BidAmount;
						}

					}
				}

				int price;
				if (loserTied)
				{
					// If the first loser was a tie, everybody pays the tie price.
					price = applicableLooserBid;
				}
				else
				{
					// They pay loser+1, but not more than their bid. Aka full price if a tie.
					price = applicableLooserBid + 1;
				}

				// Never go over their bid amount.
				price = Math.Min(price, winner.BidAmount);
				int finalPrice = price * winner.Rank.PriceMultiplier;

				winningBids.Add(new WinningBid(winner, finalPrice));
				winnerNumber++;
			}
			return
				winningBids;
		}

		private AuctionBid RemoveWinner (List<AuctionBid> bids)
		{
			bids.Sort();
			log.LogTrace("Finding best winner from: ({0})", string.Join(", ", bids));
			List<int> winningIndexes = new List<int>();

			for (int i = 0; i < bids.Count; i++)
			{
				AuctionBid bid = bids[i];

				if (winningIndexes.None())
				{
					// First winner
					winningIndexes.Add(i);
				}
				else if (bids[winningIndexes.Last()].CompareTo(bid) == 0)
				{
					// Tied for first winner.
					winningIndexes.Add(i);
				}
				else
				{
					// Done
					break;
				}
			}

			log.LogTrace("Found {0} winners.", winningIndexes.Count);

			if (winningIndexes.None())
			{
				return null;
			}

			int winningIndex = winningIndexes.OrderBy(x => random.Next()).First();
			AuctionBid winner = bids[winningIndex];
			bids.RemoveAt(winningIndex);

			return winner;
		}

		private async Task FinishAuction (Auction auction, IUserMessage announcement)
		{
			state.Auctions.TryRemove(auction.Name, out Auction _);

			CompletedAuction completedAuction = CalculateWinners(auction);

			Task charge = dkpProcessor.ChargeWinners(completedAuction);

			state.CompletedAuctions.TryAdd(completedAuction.ID, completedAuction);

			Task<IUserMessage> announce = announcement.Channel.SendMessageAsync(completedAuction.ToString());

			await Task.WhenAll(charge, announce);
		}
	}
}
