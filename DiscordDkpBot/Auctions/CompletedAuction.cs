using System;
using System.Collections.Generic;
using System.Linq;
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
			builder.AppendLine($"**\n[{Auction}] Results:**\n");

			if (WinningBids.None())
			{
				builder.Append("```css\nNo bids received.\n```");
			}
			else
			{
				builder.Append("Gratz Winners:");
				builder.AppendLine(string.Join(", ", WinningBids.Select(x => x.Bid.Author.Mention.ToString())));

				builder.AppendLine("```css");
				foreach (WinningBid winner in WinningBids)
				{
					builder.AppendLine(winner.ToString());
				}

				if (Auction.Quantity > WinningBids.Count)
				{
					builder.AppendLine($"{{{Auction.Quantity - WinningBids.Count}x Rot}}");
				}
				builder.AppendLine("```");
			}

			builder.AppendLine($"(AuctionID: {Auction.ID}) {Auction.Author.Mention}");
			return builder.ToString();
		}

		public string GetBids ()
		{
			return string.Join("\n", Auction.Bids);
		}
	}
}
