using System;

namespace DiscordDkpBot.Configuration
{
	public class EqDkpPlusConfiguration
	{
		public string BaseAddress { get; set; }
		public string PointsUri { get; set; } = "api.php?function=points";
		public string EventsUri { get; set; } = "api.php?function=events";
		public string AddRaidUri { get; set; } = "api.php?function=add_raid&test=true";
		public string Token { get; set; }
		public int[] FavoriteEvents { get; set; } = { };
		public string GetRaidsUri { get; set; } = "api.php?function=raids";
	}
}
