using System;

using Discord;

using DiscordDkpBot.Configuration;

namespace DiscordDkpBot.Auctions
{
	public class AuctionBid : IEquatable<AuctionBid>, IComparable<AuctionBid>
	{
		public Auction Auction { get; }
		public IUser Author { get; }
		public int BidAmount { get; }
		public string CharacterName { get; }
		public int CharacterId { get; }
		public string RevealString => $"{{{CharacterName}: {BidAmount}}} ({Rank.Name}) #{Author.Username}";
		public RankConfiguration Rank { get; }
		public int MaxBid => Rank.MaxBid;

		public AuctionBid (Auction auction, string characterName, int characterId, int bidAmount, RankConfiguration rank, IUser author)
		{
			CharacterName = characterName;
			CharacterId = characterId;
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
				return -1;
			}

			if (MaxBid > other.MaxBid && BidAmount > other.MaxBid)
			{
				// We outbid their cap, so reduce their bid.
				return Math.Min(other.BidAmount, other.MaxBid) - BidAmount;
			}

			if (other.MaxBid > MaxBid && other.BidAmount > MaxBid)
			{
				// they outbid our cap, so reduce our bid..
				return other.BidAmount - Math.Min(BidAmount, MaxBid);
			}

			// Caps are the same, compare bids.
			return other.BidAmount - BidAmount;
		}

		public bool Equals (AuctionBid other)
		{
			return CompareTo(other) == 0;
		}

		public override bool Equals (object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((AuctionBid)obj);
		}

		public override int GetHashCode ()
		{
			unchecked
			{
				return (BidAmount * 397) ^ (Rank?.MaxBid.GetHashCode() ?? 0);
			}
		}

		public override string ToString ()
		{
			return $"{CharacterName} {BidAmount} {Rank.Name}";
		}

		public static bool operator == (AuctionBid left, AuctionBid right)
		{
			return Equals(left, right);
		}

		public static bool operator != (AuctionBid left, AuctionBid right)
		{
			return !Equals(left, right);
		}
	}
}
