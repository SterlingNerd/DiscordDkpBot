using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DiscordDkpBot.Auctions
{
	public class BidCollection :IEnumerable<AuctionBid>
	{
		private readonly ConcurrentDictionary<string, AuctionBid> bids = new ConcurrentDictionary<string, AuctionBid>();

		public AuctionBid AddOrUpdate (AuctionBid bid)
		{
			return bids.AddOrUpdate(bid.Character, bid, (k, e) => bid);
		}

		public IEnumerator<AuctionBid> GetEnumerator ()
		{
			return bids.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}
	}
}