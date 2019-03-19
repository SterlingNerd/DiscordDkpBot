using System;
using System.Reflection;

namespace DiscordDkpBot.Configuration
{
	public class DkpBotConfiguration
	{
		public string CommandPrefix { get; set; } = ".dkp";
		public int DefaultAuctionDurationMinutes { get; set; } = 5;
		public DiscordConfiguration Discord { get; set; }
		public EqDkpConfiguration EqDkp { get; set; }
		public RankConfiguration[] Ranks { get; set; } = new RankConfiguration[0];
		public string Version => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
	}
}
