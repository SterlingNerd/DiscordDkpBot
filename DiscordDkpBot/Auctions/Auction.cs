using System;
using System.Timers;

using Discord;

using DiscordDkpBot.Dkp.EqDkpPlus.Xml;

namespace DiscordDkpBot.Auctions
{
	public class Auction
	{
		private readonly Timer timer;
		public string AnnouncementText => $"**[{ShortDescription}]**\nBids are open for **{ShortDescription}** for **{MinutesRemaining}** minutes.\n```\"{Name}\" character 69 main/box/alt/recruit```";
		public IUser Author { get; }
		public BidCollection Bids { get; } = new BidCollection();
		public IMessageChannel Channel { get; }
		public string ClosedText => $"**[{ShortDescription}]** Bids are now closed.";
		public string DetailDescription => $"({ID}) {Quantity}x {Name} for {MinutesRemaining} min.";
		public int ID { get; }
		public double MinutesRemaining { get; private set; }
		public string Name { get; }
		public int Quantity { get; }
		public string ShortDescription => $"{Quantity}x {Name}";
		public string CancelledText => $"Cancelled auction: {ShortDescription}.";
		public RaidInfo Raid { get; }
		public event Action<object, Auction> Completed;
		public event Action<object, Auction> Tick;

		public Auction (int id, int quantity, string name, double minutesRemaining,RaidInfo raid, IMessage message)
		{
			ID = id;
			Name = name;
			Quantity = quantity;
			MinutesRemaining = minutesRemaining;
			Author = message.Author;
			Channel = message.Channel;
			Raid = raid;

			timer = new Timer(TimeSpan.FromMinutes(0.5).TotalMilliseconds);
			timer.AutoReset = true;
			timer.Elapsed += OnTick;
		}

		public void Start ()
		{
			timer.Start();
		}

		public void Stop ()
		{
			timer.Stop();
		}

		public override string ToString ()
		{
			return ShortDescription;
		}

		private void OnTick (object sender, ElapsedEventArgs e)
		{
			MinutesRemaining -= 0.5;

			if (MinutesRemaining > 0)
			{
				Tick?.Invoke(timer, this);
			}
			else
			{
				timer.Stop();
				Completed?.Invoke(timer, this);
			}
		}
	}
}
