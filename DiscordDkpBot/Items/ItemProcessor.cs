using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Discord;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Items
{
	public interface IItemProcessor
	{
		Task<ItemLookupResult> GetItemEmbed(string itemName);
	}

	public class ItemProcessor : IItemProcessor
	{
		private readonly IItemSource itemSource;
		private readonly ILogger<ItemProcessor> log;

		public ItemProcessor(IItemSource itemSource, ILogger<ItemProcessor> log)
		{
			this.itemSource = itemSource;
			this.log = log;
		}

		public async Task<ItemLookupResult> GetItemEmbed(string itemName)
		{
			Embed embed = null;
			int matchesFound = 0;
			try
			{
				List<int> itemIds = await itemSource.GetItemIds(itemName);

				if (itemIds?.Any() == true)
				{
					// TODO: make this configurable for live to use max.
					embed = await itemSource.BuildEmbed(itemIds.Min());
					matchesFound = itemIds.Count;
				}
			}
			catch (Exception ex)
			{
				log.LogError(ex, $"Failed to lookup item '{itemName}'");
			}

			if (embed == null)
			{
				embed = BuildFakeEmbed(itemName);
			}

			return new ItemLookupResult(embed, matchesFound);
		}

		private Embed BuildFakeEmbed(string itemName)
		{
			EmbedBuilder builder = new EmbedBuilder();
			builder.Description = "Item Not Found. This could be because (in order of probability)\n- The item name has a typo in it\n- the item website is down/uber slow\n- This item is not in the item database";
			builder.Color = Color.Red;
			builder.Title = itemName;

			return builder.Build();
		}
	}
}
