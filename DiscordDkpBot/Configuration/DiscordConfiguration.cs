using System;

namespace DiscordDkpBot.Configuration
{
	public class DiscordConfiguration
	{
		public string Token { get; set; }
		public string CommandPrefix { get; set; } = ".dkp";
		public string RevealBidsChannelName { get; set; } = "mgmt_issues";
		public string SilentAuctionsChannelName { get; set; } = "silent_auctions";
		public bool EnableMemeDkp {get; set; } = true;
		public string MemeDkpPath { get; set; } = "";
	}
}
