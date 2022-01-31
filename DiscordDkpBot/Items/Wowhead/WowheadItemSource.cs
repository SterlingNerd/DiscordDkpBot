using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Discord;

using HtmlAgilityPack;

using Newtonsoft.Json;

namespace DiscordDkpBot.Items.Wowhead
{
	public class ListViewItem
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }
	}

	public class WowheadItemSource : ItemSourceBase
	{
		protected override string BaseUrl => "https://www.wowhead.com";

		public WowheadItemSource (IHttpClientFactory clientFactory) : base(clientFactory)
		{
		}

		protected override Task<HttpResponseMessage> DoItemLookup (string itemName, HttpClient client)
		{
			return client.GetAsync($@"{BaseUrl}/items/name:{itemName}");
		}

		protected override List<int> GetItemIdsFromHtml (string itemName, string html)
		{
			string listViewItems = null;

			using (StringReader reader = new StringReader(html))
			{
				string line = reader.ReadLine();

				while (line != null && !line.StartsWith("var listviewitems"))
				{
					line = reader.ReadLine();
				}
				listViewItems = line;

				line = reader.ReadLine();
				while (line != null && !line.StartsWith("var listviewitemsgallery"))
				{
					listViewItems += line;
					line = reader.ReadLine();
				}
			}
			listViewItems = listViewItems?.Substring(20, listViewItems.IndexOf(';') - 20);

			if (listViewItems != null)
			{
				List<ListViewItem> items = JsonConvert.DeserializeObject<List<ListViewItem>>(listViewItems);

				return items.Where(x => itemName.Equals(x.Name, StringComparison.CurrentCultureIgnoreCase)).Select(x => x.Id).ToList();
			}
			else
			{
				return new List<int>();
			}
		}

		protected override async Task<ItemTooltip> GetTooltip (int itemId, HttpClient client)
		{
			HttpResponseMessage response = await client.GetAsync($@"{BaseUrl}/tooltip/item/{itemId}");
			string json = await response.Content.ReadAsStringAsync();
			WowheadTooltip wowheadTooltip = JsonConvert.DeserializeObject<WowheadTooltip>(json);

			//remove these now so we can format later.
			wowheadTooltip.TooltipHtml = wowheadTooltip.TooltipHtml.Replace("\r", "").Replace("\n", " ");

			HtmlDocument htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(wowheadTooltip.TooltipHtml);

			ItemTooltip tooltip = new ItemTooltip();
			tooltip.ThumbnailUrl = $"https://wow.zamimg.com/images/wow/icons/large/{wowheadTooltip.Icon}.jpg";

			// Remove the item name from the html (because we're moving it to the embed title.
			//documentNode->table->tr->td->comment->b
			HtmlNode itemNameNode = htmlDoc.DocumentNode.FirstChild.FirstChild.FirstChild.FirstChild.NextSibling;
			itemNameNode.Remove();
			tooltip.ItemName = wowheadTooltip.Name;
			tooltip.Color = GetColorCode(itemNameNode.Attributes["class"].Value);

			FormatMoney(htmlDoc);

			tooltip.Description = ConvertToText(htmlDoc);

			tooltip.ItemUrl = $"{BaseUrl}/item={itemId}";
			tooltip.FooterText = "www.wowhead.com";
			tooltip.FooterIconUrl = $"{BaseUrl}/images/logos/classic-logo.gif";
			return tooltip;
		}

		private void FormatMoney(HtmlDocument htmlDoc)
		{
			foreach (HtmlNode node in SafeSelectNodes(htmlDoc.DocumentNode, " //*[@class='moneygold']"))
			{
				node.ParentNode.InsertAfter(htmlDoc.CreateTextNode("g"), node);
			}
			foreach (HtmlNode node in SafeSelectNodes(htmlDoc.DocumentNode, " //*[@class='moneysilver']"))
			{
				node.ParentNode.InsertAfter(htmlDoc.CreateTextNode("s"), node);
			}
			foreach (HtmlNode node in SafeSelectNodes(htmlDoc.DocumentNode, " //*[@class='moneycopper']"))
			{
				node.ParentNode.InsertAfter(htmlDoc.CreateTextNode("c"), node);
			}

		}
		private Color? GetColorCode (string itemQuality)
		{
			switch (itemQuality)
			{
				case "q0":
					return Color.DarkGrey;
				case "q1":
					return Color.LightGrey;
				case "q2":
					return Color.Green;
				case "q3":
					return Color.Blue;
				case "q4":
					return Color.Purple;
				case "q5":
					return Color.Orange;
				case "q6":
					return Color.Gold;
				default:
					return null;
			}
		}

		protected async Task<ItemTooltip> GetTooltip2 (int itemId, HttpClient client)
		{
			ItemTooltip tooltip = new ItemTooltip();

			HttpResponseMessage response = await client.GetAsync($@"{BaseUrl}/item={itemId}");
			string html = await response.Content.ReadAsStringAsync();

			//WowheadTooltip wowheadTooltip = JsonConvert.DeserializeObject<WowheadTooltip>(json);

			HtmlDocument htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(html);

			HtmlNode h1Node = htmlDoc.DocumentNode.SelectSingleNode("//h1");
			tooltip.ItemName = h1Node.InnerText;
			tooltip.Description = h1Node.NextSibling.NextSibling.InnerText;

			HtmlNode iconNode = htmlDoc.DocumentNode.SelectSingleNode("//link[@rel='image_src']");
			tooltip.ThumbnailUrl = iconNode.Attributes["href"].Value;

			tooltip.ItemUrl = $"{BaseUrl}/item={itemId}";
			tooltip.FooterText = "www.wowhead.com";
			tooltip.FooterIconUrl = $"{BaseUrl}/images/logos/classic-logo.gif";
			return tooltip;
		}

		protected override int ParseId (HtmlNode link)
		{
			string idString = link.Attributes["href"].Value.Replace("/item=", string.Empty);
			idString = idString.Substring(0, idString.IndexOf('/'));
			int id = int.Parse(idString);
			return id;
		}
	}
}
