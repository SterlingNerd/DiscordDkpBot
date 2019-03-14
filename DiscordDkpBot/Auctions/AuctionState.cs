using System;
using System.Collections.Concurrent;
using System.Threading;

using Discord.WebSocket;

namespace DiscordDkpBot.Auctions
{
	public class AuctionState
	{
		private readonly AuctionIdHider hider = new AuctionIdHider();
		public ConcurrentDictionary<string, Auction> Auctions { get; } = new ConcurrentDictionary<string, Auction>();
		public ConcurrentDictionary<int, CompletedAuction> CompletedAuctions { get; } = new ConcurrentDictionary<int, CompletedAuction>();

		public int NextAuctionId => hider.NextAuctionId;

		public void Add (CompletedAuction completedAuction)
		{
			CompletedAuctions.TryAdd(completedAuction.ID, completedAuction);
		}

		public Auction CreateAuction (int quantity, string name, int minutes, SocketUser author)
		{
			Auction auction = new Auction(NextAuctionId, quantity, name, minutes, author);
			if (!Auctions.TryAdd(auction.Name, auction))
			{
				throw new AuctionAlreadyExistsException($"Auction for {auction.Name} already exists.");
			}
			auction.Completed += () => Auctions.TryRemove(auction.Name, out Auction _);
			return auction;
		}

		private class AuctionIdHider
		{
			private int _nextAuctionId = new Random().Next(100000, 999999);

			public int NextAuctionId => Interlocked.Increment(ref _nextAuctionId);
		}
	}
}
