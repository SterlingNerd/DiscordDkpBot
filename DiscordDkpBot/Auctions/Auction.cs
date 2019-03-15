using System;

using Discord;
using Discord.WebSocket;

namespace DiscordDkpBot.Auctions
{
	public class Auction
	{
		public string Announcement => $"**[{ShortDescription}]**\nBids are open for **{ShortDescription}** for **{MinutesRemaining}** minutes.\n```\"{Name}\" character 69 main/box/alt/recruit```";
		public string ClosedText => $"***[{ShortDescription}]**\nBids are now closed.";
		public IUser Author { get; }
		public BidCollection Bids { get; } = new BidCollection();
		public string DetailString => $"({ID}) {Quantity}x {Name} for {MinutesRemaining} min.";
		public int ID { get; }
		public double MinutesRemaining { get; set; }
		public string Name { get; }
		public int Quantity { get; }
		public string ShortDescription => $"{Quantity}x {Name}";

		public Auction (int id, int quantity, string name, double minutesRemaining, IUser author)
		{
			ID = id;
			Name = name;
			Quantity = quantity;
			MinutesRemaining = minutesRemaining;
			Author = author;
		}

		public override string ToString ()
		{
			return ShortDescription;
		}
	}
}
