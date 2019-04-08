using System;

namespace DiscordDkpBot.Configuration
{
	public class EqDkpPlusConfiguration
	{
		public string AddItemUri { get; set; } = "api.php?function=add_item&test=true";
		public string AddRaidUri { get; set; } = "api.php?function=add_raid&test=true";
		public string BaseAddress { get; set; }
		public string EventsUri { get; set; } = "api.php?function=events";
		public int[] FavoriteEvents { get; set; } = { };
		public string GetRaidsUri { get; set; } = "api.php?function=raids";
		public string PointsUri { get; set; } = "api.php?function=points";
		public string Token { get; set; }
	}
}
