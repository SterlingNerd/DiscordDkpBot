using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using HtmlAgilityPack;

namespace DiscordDkpBot.Items.Allakhazam
{
	public class AllakhazamItemSource : ItemSourceBase
	{
		protected override string BaseUrl => "https://everquest.allakhazam.com";

		public AllakhazamItemSource(IHttpClientFactory clientFactory) : base(clientFactory)
		{
		}

		protected override Task<HttpResponseMessage> DoItemLookup(string itemName, HttpClient client)
		{
			return client.GetAsync($@"{BaseUrl}/db/searchdb.html?iname={itemName}");
		}

		protected override List<int> GetItemIdsFromHtml(string itemName, string html)
		{
			List<int> itemIds = new List<int>();
			HtmlDocument htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(html);

			HtmlNode div = htmlDoc.GetElementbyId("Items_t");
			HtmlNode table = div?.FirstChild;// table
			HtmlNode body = table?.ChildNodes.Single(x => x.Name == "tbody");
			IEnumerable<HtmlNode> trs = body?.ChildNodes.Where(x => x.Name == "tr");
			IEnumerable<HtmlNode> tds = trs?.Select(tr => tr.ChildNodes.Skip(1).First());
			IEnumerable<HtmlNode> links1 = tds?.Select(td => td.ChildNodes.First(x => x.Name == "a"));
			IEnumerable<HtmlNode> links = links1;

			if (links?.Any() == true)
			{
				foreach (HtmlNode link in links)
				{
					int id = ParseId(link);

					string name = link.FirstChild?.InnerText?.Trim();

					if (name?.Equals(itemName, StringComparison.CurrentCultureIgnoreCase) == true)
					{
						itemIds.Add(id);
					}
				}
			}

			return itemIds;
		}

		protected override async Task<ItemTooltip> GetTooltip(int itemId, HttpClient client)
		{
			string tooltipHtml = await GetItemTooltipHtml(itemId, client);

			HtmlDocument htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(tooltipHtml);
			HtmlNode itemNameNode = htmlDoc.DocumentNode.SelectSingleNode("//span[@class=\"iname\"]");
			ItemTooltip tooltip = new ItemTooltip();
			tooltip.ItemName = itemNameNode.InnerText;
			itemNameNode.Remove();
			tooltip.ThumbnailUrl = htmlDoc.DocumentNode.SelectSingleNode("//body/img").Attributes["src"].Value;
			ReplaceHyperlinks(htmlDoc);
			tooltip.Description = htmlDoc.DocumentNode.SelectSingleNode("//body/div").InnerText;
			tooltip.ItemUrl = $"{BaseUrl}/db/item.html?item={itemId}";
			tooltip.FooterText = "everquest.allakhazam.com";
			tooltip.FooterIconUrl = "https://zam.zamimg.com/images/8/2/821dfe832c32bfd442c7a322d901fed6.png";

			return tooltip;
		}

		protected override int ParseId(HtmlNode link)
		{
			string idString = link.Attributes["href"].Value.Replace("/db/item.html?item=", string.Empty);
			return int.Parse(idString);
		}

		private async Task<string> GetItemTooltipHtml(int id, HttpClient client)
		{
			HttpResponseMessage response = await client.GetAsync($@"{BaseUrl}/cluster/ihtml.pl?item={id}");
			return await response.Content.ReadAsStringAsync();
		}
	}
}
