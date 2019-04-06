using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DiscordDkpBot.Auctions
{
	public class BidCollection : IEnumerable<AuctionBid>
	{
		private readonly ConcurrentDictionary<int, AuctionBid> bids = new ConcurrentDictionary<int, AuctionBid>();

		public AuctionBid AddOrUpdate (AuctionBid bid)
		{
			return bids.AddOrUpdate(bid.CharacterId, bid, (k, e) => bid);
		}

		public IEnumerator<AuctionBid> GetEnumerator ()
		{
			return bids.Values.GetEnumerator();
		}

		public bool TryRemove (ulong authorId, out AuctionBid bid)
		{
			bool success = false;
			bid = null;

			//return bids.TryRemove(authorId, out bid);
			foreach (AuctionBid liveBid in bids.Values.Where(x => x.Author.Id == authorId))
			{
				success |= bids.TryRemove(liveBid.CharacterId, out bid);
			}

			return success;
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}
	}
}
