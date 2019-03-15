using System;

namespace DiscordDkpBot.Configuration
{
	public class DkpBotConfiguration
	{
		public string CommandPrefix { get; set; } = "!";
		public int DefaultAuctionDurationMinutes { get; set; } = 5;
		public DiscordConfiguration Discord { get; set; }

		public RankConfiguration[] Ranks { get; set; } = new RankConfiguration[0];
	}

	public class RankConfiguration
	{
		private int maxBid;
		private int priceMultiplier;

		public int MaxBid
		{
			get => maxBid <= 0 ? int.MaxValue : maxBid;
			set => maxBid = value;
		}

		public string Name { get; set; }

		public int PriceMultiplier
		{
			get => priceMultiplier <= 0 ? 1 : priceMultiplier;
			set => priceMultiplier = value;
		}

		public RankConfiguration ()
		{
		}

		public RankConfiguration (string name, int? maxBid, int? priceMultiplier)
		{
			Name = name;
			MaxBid = maxBid ?? int.MaxValue;
			PriceMultiplier = priceMultiplier ?? 1;
		}
	}
}
