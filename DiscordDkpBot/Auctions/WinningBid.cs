using System;

namespace DiscordDkpBot.Auctions
{
	public class WinningBid : AuctionBid
	{
		public int Price { get; }

		public WinningBid (AuctionBid bid, int price) : base(bid.Character, bid.BidAmount, bid.BidCap, bid.PriceMultiplier, bid.CharacterRank, bid.Author)
		{
			Price = price;
		}

		public override string ToString ()
		{
			return $"**{Character}** ({CharacterRank}) for **{Price}**";
		}
	}
}
