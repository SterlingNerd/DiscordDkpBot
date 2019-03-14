using System;
using System.Collections.Generic;
using System.Text;

using DiscordDkpBot.Extensions;

namespace DiscordDkpBot.Auctions
{
	public class CompletedAuction
	{
		public Auction Auction { get; }
		public int ID => Auction.ID;
		public List<WinningBid> WinningBids { get; }

		public CompletedAuction (Auction auction, List<WinningBid> winningBids)
		{
			Auction = auction;
			WinningBids = winningBids;
		}

		public override string ToString ()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendLine($"**[{Auction}]** Results:");

			if (WinningBids.None())
			{
				builder.Append("```No bids received.```");
			}
			else
			{
				foreach (WinningBid winner in WinningBids)
				{
					builder.AppendLine($"```{winner}```");
				}

				if (Auction.Quantity > WinningBids.Count)
				{
					builder.AppendLine($"```{Auction.Quantity - WinningBids.Count}x Rot```");
				}
			}

			builder.AppendLine($"(AuctionID: {Auction.ID} {Auction.Author.Mention}");
			return builder.ToString();
		}

		public string GetBids ()
		{
			return string.Join("\n", Auction.Bids);
		}
	}
}
