using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Discord;

namespace DiscordDkpBot.Items
{
	public interface IItemSource
	{
		Task<Embed> BuildEmbed(int min);
		Embed BuildFakeEmbed (string itemName);
		Task<List<int>> GetItemIds(string itemName);
	}
}
