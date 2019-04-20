using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

using DiscordDkpBot.Dkp.EqDkpPlus.Xml;

namespace DiscordDkpBot.Auctions
{
	public class AuctionState
	{
		private readonly AuctionIdHider hider = new AuctionIdHider();
		public ConcurrentDictionary<string, Auction> Auctions { get; } = new ConcurrentDictionary<string, Auction>(StringComparer.OrdinalIgnoreCase);
		public ConcurrentDictionary<int, CompletedAuction> CompletedAuctions { get; } = new ConcurrentDictionary<int, CompletedAuction>();

		public int NextAuctionId => hider.NextAuctionId;

		private class AuctionIdHider
		{
			private int _nextAuctionId = new Random().Next(100000, 999999);

			public int NextAuctionId => Interlocked.Increment(ref _nextAuctionId);
		}
	}
}
