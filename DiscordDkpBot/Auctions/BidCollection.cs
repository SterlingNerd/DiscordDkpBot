using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DiscordDkpBot.Auctions
{
	public class BidCollection : IEnumerable<AuctionBid>
	{
		private readonly ConcurrentDictionary<ulong, AuctionBid> bids = new ConcurrentDictionary<ulong, AuctionBid>();

		public AuctionBid AddOrUpdate (AuctionBid bid)
		{
			return bids.AddOrUpdate(bid.Author.Id, bid, (k, e) => bid);
		}

		public IEnumerator<AuctionBid> GetEnumerator ()
		{
			return bids.Values.GetEnumerator();
		}

		public bool TryRemove (ulong authorId, out AuctionBid bid)
		{
			return bids.TryRemove(authorId, out bid);
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}
	}
}
