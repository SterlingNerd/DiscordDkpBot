using System;

using Discord.WebSocket;

namespace DiscordDkpBot.Auctions
{
	public class AuctionBid : IComparable<AuctionBid>
	{
		public SocketUser Author { get; }
		public int BidAmount { get; }
		public int BidCap { get; }
		public string Character { get; }
		public string CharacterRank { get; }
		public int PriceMultiplier { get; }

		public AuctionBid (string character, int bidAmount, int bidCap, int priceMultiplier, string characterRank, SocketUser author)
		{
			Character = character;
			BidAmount = bidAmount;
			BidCap = bidCap;
			PriceMultiplier = priceMultiplier;
			CharacterRank = characterRank;
			Author = author;
		}

		public override string ToString ()
		{
			return $"{Character} {BidAmount} {CharacterRank}";
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

			if (BidCap > other.BidCap)
			{
				// Our cap is bigger, so cap their bid.
				return BidAmount - Math.Min(other.BidAmount, other.BidCap);
			}

			if (BidCap > other.BidCap)
			{
				// their cap is bigger, so cap our bid.
				return Math.Min(BidAmount, BidCap) - other.BidAmount;
			}

			// Caps are the same, compare bids.
			return BidAmount - other.BidAmount;
		}
	}
}
