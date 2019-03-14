using System;
using System.Collections.Concurrent;
using System.Timers;

using Discord.WebSocket;

namespace DiscordDkpBot.Auctions
{
	public class Auction
	{
		public SocketUser Author { get; }
		public ConcurrentDictionary<string, AuctionBid> Bids { get; } = new ConcurrentDictionary<string, AuctionBid>();

		public string DetailString => $"({ID}) {Quantity}x {Name} for {Minutes} min.";
		public int ID { get; }
		public int Minutes { get; }
		public string Name { get; }
		public int Quantity { get; }

		public Auction (int id, int quantity, string name, int minutes, SocketUser author)
		{
			ID = id;
			Name = name;
			Quantity = quantity;
			Minutes = minutes;
			Author = author;
		}

		public event Action Completed;

		public void Start ()
		{
			Timer timer = new Timer(TimeSpan.FromMinutes(Minutes).TotalMilliseconds);
			timer.Elapsed += (o, s) => { Completed(); };
			timer.Start();
		}

		public override string ToString ()
		{
			return $"{Quantity}x {Name}";
		}
	}
}
