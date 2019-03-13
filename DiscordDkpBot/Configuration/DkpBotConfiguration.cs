using System;

namespace DiscordDkpBot.Configuration
{
	public class DkpBotConfiguration
	{
		public string CommandPrefix { get; set; } = "!";
		public DiscordConfiguration Discord { get; set; }
	}
}
