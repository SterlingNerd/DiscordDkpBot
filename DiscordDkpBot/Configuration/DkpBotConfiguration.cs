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
		public int MaxBid { get; set; }

		public string Name { get; set; }
		public int PriceMultiplier { get; set; }

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
