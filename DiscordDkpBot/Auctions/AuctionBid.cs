using System;

using Discord.WebSocket;

using DiscordDkpBot.Configuration;

namespace DiscordDkpBot.Auctions
{
	public class AuctionBid : IComparable<AuctionBid>
	{
		public Auction Auction { get; }
		public SocketUser Author { get; }
		public int BidAmount { get; }
		public string Character { get; }
		public RankConfiguration Rank { get; }

		public AuctionBid (Auction auction, string character, int bidAmount, RankConfiguration rank, SocketUser author)
		{
			Character = character;
			BidAmount = bidAmount;
			Rank = rank;
			Author = author;
			Auction = auction;
		}

		public int CompareTo (AuctionBid other)
		{
			if (ReferenceEquals(this, other))
			{
				return 0;
			}

			if (ReferenceEquals(null, other))
			{
				return 1;
			}

			if (Rank.MaxBid > other.Rank.MaxBid)
			{
				// Our cap is bigger, so reduce their bid.
				return BidAmount - Math.Min(other.BidAmount, other.Rank.MaxBid);
			}

			if (Rank.MaxBid < other.Rank.MaxBid)
			{
				// their cap is bigger, so reduce our bid.
				return Math.Min(BidAmount, Rank.MaxBid) - other.BidAmount;
			}

			// Caps are the same, compare bids.
			return BidAmount - other.BidAmount;
		}

		public override string ToString ()
		{
			return $"{Character} {BidAmount} {Rank.Name}";
		}
	}
}
