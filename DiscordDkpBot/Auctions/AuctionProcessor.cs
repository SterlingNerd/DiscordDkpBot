using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Discord;

using DiscordDkpBot.Configuration;
using DiscordDkpBot.Extensions;
using DiscordDkpBot.Items;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Auctions
{
	public class AuctionProcessor : IAuctionProcessor
	{
		private readonly AuctionState auctionState;
		private readonly DkpBotConfiguration configuration;
		private readonly IItemProcessor itemProcessor;
		private readonly ILogger<AuctionProcessor> log;
		private readonly Dictionary<string, RankConfiguration> ranks;

		public AuctionProcessor(DkpBotConfiguration configuration, AuctionState auctionState, IItemProcessor itemProcessor, ILogger<AuctionProcessor> log)
		{
			ranks = configuration.Ranks.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
			this.configuration = configuration;
			this.auctionState = auctionState;
			this.itemProcessor = itemProcessor;
			this.log = log;
		}

		public Task<AuctionBid> AddOrUpdateBid(string item, string character, string rank, int bid, IMessage message)
		{
			// First make sure we can make a valid bid out of it.
			if (!ranks.TryGetValue(rank, out RankConfiguration rankConfig))
			{
				throw new ArgumentException($"Rank {rank} does not exist.");
			}

			if (!auctionState.Auctions.TryGetValue(item, out Auction auction))
			{
				throw new AuctionNotFoundException(item);
			}

			AuctionBid newBid = auction.Bids.AddOrUpdate(new AuctionBid(auction, character, bid, rankConfig, message.Author));

			log.LogInformation($"Created bid: {newBid}");

			message.Channel.SendMessageAsync($"Bid accepted for **{newBid.Auction}**\n"
				+ $"```\"{auction.Name}\" {newBid}```"
				+ $"If you win, you could pay up to **{newBid.BidAmount * newBid.Rank.PriceMultiplier}**.\n"
				+ $"If you wish to modify your bid before the auction completes, simply enter a new bid in the next **{auction.MinutesRemaining:##.#}** minutes.\n"
				+ "If you wish to cancel your bid use the following syntax:\n"
				+ $"```\"{newBid.Auction.Name}\" cancel```");

			return Task.FromResult(newBid);
		}

		public CompletedAuction CalculateWinners(Auction auction)
		{
			List<AuctionBid> bids = auction.Bids.ToList();
			log.LogTrace("Finding winners for {0} from bids submitted: ({1})", auction.DetailDescription, string.Join("', ", auction.Bids));
			List<AuctionBid> winningBids = new List<AuctionBid>();

			for (int i = 0; i < auction.Quantity; i++)
			{
				// Grab the first winner.
				AuctionBid winner = CalculateWinner(bids);

				if (winner == null)
				{
					// No more bids to be found. we're done.
					break;
				}
				else
				{
					winningBids.Add(winner);

					// Remove that winner and go again.
					bids.Remove(winner);
				}
			}

			//Now, how much do they pay?
			bids.Sort();

			// You lose! Good DAY sir!
			AuctionBid loser = bids.FirstOrDefault();
			List<WinningBid> winners = new List<WinningBid>();
			foreach (AuctionBid winner in winningBids)
			{
				int applicableLooserBid;
				if (loser == null)
				{
					applicableLooserBid = 0;
				}
				else if (winner.Rank.MaxBid > loser.Rank.MaxBid && winner.BidAmount > loser.Rank.MaxBid)
				{
					// If our bid cap and is higher than their bid cap, and we bid over their cap. reduce their bid.
					applicableLooserBid = Math.Min(loser.BidAmount, loser.Rank.MaxBid);
				}
				else
				{
					// Otherwise, 
					applicableLooserBid = loser.BidAmount;
				}

				int price = applicableLooserBid + 1;
				int finalPrice = price * winner.Rank.PriceMultiplier;
				winners.Add(new WinningBid(winner, finalPrice));
			}

			log.LogInformation("{0} found {1} winners: {2}", auction.DetailDescription, winners.Count, string.Join(", ", winners));

			return new CompletedAuction(auction, winners);
		}

		public async Task<Auction> CancelAuction(string name, IMessage message)
		{
			if (!auctionState.Auctions.TryRemove(name, out Auction auction))
			{
				throw new AuctionAlreadyExistsException($"Auction for {name} does not exists.");
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

		public Task<AuctionBid> CancelBid(string item, IMessage message)
		{
			if (!auctionState.Auctions.TryGetValue(item, out Auction auction))
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

		public async Task<Auction> StartAuction(int? quantity, string name, int? minutes, IMessage message)
		{
			Auction auction = new Auction(auctionState.NextAuctionId, quantity ?? 1, name, minutes ?? configuration.DefaultAuctionDurationMinutes, message);
			if (!auctionState.Auctions.TryAdd(auction.Name, auction))
			{
				throw new AuctionAlreadyExistsException($"Auction for {auction.Name} already exists.");
			}
			try
			{
				ItemLookupResult itemResult = await itemProcessor.GetItemEmbed(name);
				string announcementText = auction.AnnouncementText;
				if (itemResult.MatchesFound > 1)
				{
					announcementText += "There were several items found with the same name, this might not actually be the correct item stats.";
				}

				IUserMessage announcement = await auction.Channel.SendMessageAsync(announcementText, false, itemResult.Embed);

				auction.Tick += async (o, s) => await announcement.ModifyAsync(m => m.Content = auction.AnnouncementText);
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
				auctionState.Auctions.TryRemove(auction.Name, out Auction _);
				throw;
			}
		}

		private AuctionBid CalculateWinner(List<AuctionBid> bids)
		{
			bids.Sort();
			log.LogTrace("Finding best winner from: ({0})", string.Join(", ", bids));
			List<AuctionBid> winningBids = new List<AuctionBid>();

			foreach (AuctionBid bid in bids)
			{
				if (winningBids.None())
				{
					// First winner
					winningBids.Add(bid);
				}
				else if (winningBids.Last().CompareTo(bid) == 0)
				{
					// Tied for first winner.
					winningBids.Add(bid);
				}
				else
				{
					// Done
					break;
				}
			}

			log.LogTrace("Found {0} winners.", winningBids.Count);

			if (winningBids.None())
			{
				return null;
			}

			Random random = new Random();
			AuctionBid winner = winningBids.OrderBy(x => random.Next()).First();

			return winner;
		}

		private Task FinishAuction(Auction auction, IUserMessage announcement)
		{
			auctionState.Auctions.TryRemove(auction.Name, out Auction _);

			CompletedAuction completedAuction = CalculateWinners(auction);

			auctionState.CompletedAuctions.TryAdd(completedAuction.ID, completedAuction);

			return announcement.Channel.SendMessageAsync(completedAuction.ToString());
		}
	}
}
