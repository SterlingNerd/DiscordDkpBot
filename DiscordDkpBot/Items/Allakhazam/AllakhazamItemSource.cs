using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Discord;

using HtmlAgilityPack;

namespace DiscordDkpBot.Items.Allakhazam
{
	public class AllakhazamItemSource : IItemSource
	{
		private readonly IHttpClientFactory clientFactory;

		public AllakhazamItemSource(IHttpClientFactory clientFactory)
		{
			this.clientFactory = clientFactory;
		}

		public async Task<Embed> BuildEmbed(int id)
		{
			EmbedBuilder builder = new EmbedBuilder();
			HtmlDocument htmlDoc = new HtmlDocument();

			htmlDoc.LoadHtml(await GetItemTooltipHtml(id));

			builder.Color = Color.Gold;

			HtmlNode itemNameNode = htmlDoc.DocumentNode.SelectSingleNode("//span[@class=\"iname\"]");
			builder.Title = itemNameNode.InnerText;

			itemNameNode.Remove();

			ReplaceHyperlinks(htmlDoc);

			builder.ThumbnailUrl = htmlDoc.DocumentNode.SelectSingleNode("//body/img").Attributes["src"].Value;

			builder.Description = htmlDoc.DocumentNode.SelectSingleNode("//body/div").InnerText;
			builder.Url = $"http://everquest.allakhazam.com/db/item.html?item={id}";

			builder.WithFooter("everquest.allakhazam.com", "http://zam.zamimg.com/images/8/2/821dfe832c32bfd442c7a322d901fed6.png");

			return builder.Build();
		}

		private static void ReplaceHyperlinks(HtmlDocument htmlDoc)
		{
			HtmlNodeCollection effects = htmlDoc.DocumentNode.SelectNodes("//a");
			if (effects?.Any() == true)
			{
				foreach (HtmlNode link in effects)
				{
					HtmlNode newNode = HtmlNode.CreateNode($"[{link.InnerText}](http://everquest.allakhazam.com/{link.Attributes["href"].Value.TrimStart('/')})");

					link.ParentNode.ReplaceChild(newNode, link);
				}
			}
		}

		public async Task<List<int>> GetItemIds(string itemName)
		{
			HttpClient client = clientFactory.CreateClient();
			HttpResponseMessage response = await client.GetAsync($@"http://everquest.allakhazam.com/search.html?q={itemName}");
			string html = await response.Content.ReadAsStringAsync();

			HtmlDocument htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(html);

			HtmlNodeCollection links = htmlDoc.GetElementbyId("Items_t").SelectNodes("//td[2]/a");
			List<int> itemIds = new List<int>();
			if (links?.Any() == true)
			{
				foreach (HtmlNode link in links)
				{
					string idString = link.Attributes["href"].Value.Replace("/db/item.html?item=", string.Empty);
					int id = int.Parse(idString);

					string name = link.FirstChild.InnerText.Trim();

					if (name == itemName)
					{
						itemIds.Add(id);
					}
				}
			}

			return itemIds;
		}

		public async Task<string> GetItemTooltipHtml(int id)
		{
			HttpClient client = clientFactory.CreateClient();
			HttpResponseMessage response = await client.GetAsync($@"http://everquest.allakhazam.com/cluster/ihtml.pl?item={id}");
			return await response.Content.ReadAsStringAsync();
		}
	}
}
