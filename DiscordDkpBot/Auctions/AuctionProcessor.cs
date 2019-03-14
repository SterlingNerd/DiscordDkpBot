using System;
using System.Collections.Generic;
using System.Linq;

using Discord.WebSocket;

using DiscordDkpBot.Configuration;
using DiscordDkpBot.Extensions;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Auctions
{
	public interface IAuctionProcessor
	{
		Auction CreateAuction (int? quantity, string name, int? minutes, ISocketMessageChannel messageChannel, SocketUser author);
	}

	public class AuctionProcessor : IAuctionProcessor
	{
		private readonly AuctionState auctionState;
		private readonly DkpBotConfiguration configuration;
		private readonly ILogger<AuctionProcessor> log;

		public AuctionProcessor (DkpBotConfiguration configuration, AuctionState auctionState, ILogger<AuctionProcessor> log)
		{
			this.configuration = configuration;
			this.auctionState = auctionState;
			this.log = log;
		}

		public CompletedAuction CalculateWinners (Auction auction)
		{
			log.LogTrace("Finding winners for {0}", auction.DetailString);

			List<AuctionBid> bids = auction.Bids.Values.ToList();
			List<WinningBid> winners = new List<WinningBid>();

			for (int i = 0; i < auction.Quantity; i++)
			{
				// Grab the first winner.
				WinningBid winner = CalculateWinner(bids);

				if (winner == null)
				{
					// No more winners to be found. we're done.
					break;
				}
				else
				{
					winners.Add(winner);

					// Remove that winner and go again.
					bids.Remove(winner.Bid);
				}
			}

			log.LogInformation("{0} found {1} winners: {2}", auction.DetailString, winners.Count, string.Join(", ", winners));

			return new CompletedAuction(auction, winners);
		}

		public Auction CreateAuction (int? quantity, string name, int? minutes, ISocketMessageChannel channel, SocketUser author)
		{
			Auction auction = auctionState.CreateAuction(quantity ?? 1, name, minutes ?? configuration.DefaultAuctionDurationMinutes, author);
			auction.Completed += () => FinishAuction(auction, channel);
			auction.Start();
			log.LogTrace("Started Auction: {0}", auction.DetailString);

			return auction;
		}

		private WinningBid CalculateWinner (List<AuctionBid> bids)
		{
			bids.Sort((c1, c2) => c1.CompareTo(c2));

			List<AuctionBid> winningBids = new List<AuctionBid>();
			AuctionBid loser = null;

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
					// You lose! Good DAY sir!
					loser = bid;
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

			int price = loser?.BidAmount ?? 0 + 1;
			int finalPrice = price * winner.PriceMultiplier;
			return new WinningBid(winner, finalPrice);
		}


		private void FinishAuction (Auction auction, ISocketMessageChannel channel)
		{
			CompletedAuction completedAuction = CalculateWinners(auction);
			auctionState.Add(completedAuction);
			channel.SendMessageAsync(completedAuction.ToString());
		}
	}
}
