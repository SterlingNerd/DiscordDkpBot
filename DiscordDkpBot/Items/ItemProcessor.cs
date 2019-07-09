using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Discord;

using DiscordDkpBot.Configuration;

namespace DiscordDkpBot.Items
{
	public interface IItemProcessor
	{
		Task<ItemLookupResult> GetItemEmbed(string itemName);
	}
	public class ItemProcessor : IItemProcessor
	{
		private readonly DkpBotConfiguration configuration;
		private readonly IItemSource itemSource;
		private readonly IHttpClientFactory clientFactory;

		public ItemProcessor(DkpBotConfiguration configuration, IItemSource itemSource)
		{
			this.configuration = configuration;
			this.itemSource = itemSource;
			this.clientFactory = clientFactory;
		}
		public async Task<ItemLookupResult> GetItemEmbed(string itemName)
		{
			List<int> itemIds = await itemSource.GetItemIds(itemName);

			Embed embed;
			if (itemIds?.Any() == false)
			{
				embed = BuildFakeEmbed(itemName);
			}
			else
			{
				// TODO: make this configurable for live to use max.
				embed = await itemSource.BuildEmbed(itemIds.Min());
			}

			return new ItemLookupResult(embed, itemIds?.Count ?? 0);
		}



		private Embed BuildFakeEmbed (string itemName)
		{
			EmbedBuilder builder = new EmbedBuilder();
			builder.Description = "Item Not Found. This could be because (in order of probability)\n- The item name has a typo in it\n- eq.allakahazam.com is down/uber slow\nThis item is not in the item database";
			builder.Color = Color.Red;
			builder.Title = itemName;

			return builder.Build();
		}



	}
}
