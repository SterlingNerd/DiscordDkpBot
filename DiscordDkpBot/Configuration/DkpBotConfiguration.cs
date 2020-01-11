using System;
using System.Reflection;

namespace DiscordDkpBot.Configuration
{
	public class DkpBotConfiguration
	{
		public string ItemSource { get; set; } = "Allakhazam";
		public string CommandPrefix { get; set; } = ".dkp";
		public int DefaultAuctionDurationMinutes { get; set; } = 4;
		public DiscordConfiguration Discord { get; set; } = new DiscordConfiguration();
		public EqDkpPlusConfiguration EqDkpPlus { get; set; } = new EqDkpPlusConfiguration();
		public RankConfiguration[] Ranks { get; set; } = new RankConfiguration[0];
		public string Version => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
	}
}
