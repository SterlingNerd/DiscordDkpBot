using System;

namespace DiscordDkpBot.Configuration
{
	public class DkpBotConfiguration
	{
		public string CommandPrefix { get; } = "!";
		public int DefaultAuctionDurationMinutes { get; } = 5;
		public DiscordConfiguration Discord { get; }

		public RankConfiguration[] Ranks { get; } = {
			new RankConfiguration("Main", 0, 1)
		};
	}

	public class RankConfiguration
	{
		public int MaxBid { get; }

		public string Name { get; }
		public int PriceMultiplier { get; }

		public RankConfiguration (string name, int maxBid, int priceMultiplier)
		{
			Name = name;
			MaxBid = maxBid;
			PriceMultiplier = priceMultiplier;
		}
	}
}
