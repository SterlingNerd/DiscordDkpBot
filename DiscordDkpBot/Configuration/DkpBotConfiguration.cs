using System;
using System.Collections.Generic;
using System.Reflection;

namespace DiscordDkpBot.Configuration
{
	public class DkpBotConfiguration
	{
		public int DefaultAuctionDurationMinutes { get; set; } = 4;
		public DiscordConfiguration Discord { get; set; } = new DiscordConfiguration();
		public EqDkpPlusConfiguration EqDkpPlus { get; set; } = new EqDkpPlusConfiguration();
		public Dictionary<string, RankConfiguration> ExpandedRanks => ExpandRanks();
		public string ItemSource { get; set; } = "Allakhazam";
		public RankConfiguration[] Ranks { get; set; } = new RankConfiguration[0];
		public string Version => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

		private Dictionary<string, RankConfiguration> ExpandRanks()
		{
			Dictionary<string, RankConfiguration> er = new Dictionary<string, RankConfiguration>(StringComparer.InvariantCultureIgnoreCase);
			foreach (RankConfiguration rank in Ranks)
			{
				er.Add(rank.Name, rank);
				if (!string.IsNullOrWhiteSpace(rank.Aliases))
				{
					foreach (string alias in rank.Aliases?.Split(',', StringSplitOptions.RemoveEmptyEntries))
					{
						er.Add(alias.Trim(), rank);
					}
				}
			}
			return er;
		}
	}
}
