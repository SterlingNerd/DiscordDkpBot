using System;

namespace DiscordDkpBot.Auctions
{
	public class WinningBid
	{
		public AuctionBid Bid { get; }
		public int Price { get; }

		public WinningBid (AuctionBid bid, int price)
		{
			Bid = bid;
			Price = price;
		}

		public override string ToString ()
		{
			return $"{Bid.Character} ({Bid.Rank.Name}) for {Price}";
		}
	}
}
