using System;

using Discord;

namespace DiscordDkpBot.Items
{
	public class ItemTooltip
	{
		public string ItemName { get; set; }
		public string ThumbnailUrl { get; set; }
		public string Description { get; set; }
		public string ItemUrl { get; set; }
		public string FooterText { get; set; }
		public string FooterIconUrl { get; set; }
		public Color? Color { get; set; }
	}
}
