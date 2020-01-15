using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;

using Discord;

using HtmlAgilityPack;

namespace DiscordDkpBot.Items
{
	public abstract class ItemSourceBase : IItemSource
	{
		private readonly IHttpClientFactory clientFactory;

		protected abstract string BaseUrl { get; }

		protected ItemSourceBase(IHttpClientFactory clientFactory)
		{
			this.clientFactory = clientFactory;
		}

		public Embed BuildFakeEmbed (string itemName)
		{
			EmbedBuilder builder = new EmbedBuilder();
			builder.Description = "Item Not Found. This could be because (in order of probability)\n- The item name has a typo in it\n- The item website is down/uber slow\n- This item is not in the item database";
			builder.Color = Color.Red;
			builder.Title = itemName;

			return builder.Build();
		}

		public async Task<Embed> BuildEmbed(int itemId)
		{
			ItemTooltip tooltip = await GetTooltip(itemId, GetHttpClient());

			EmbedBuilder builder = new EmbedBuilder();
			builder.Title = tooltip.ItemName;
			builder.Color = tooltip.Color ?? Color.Gold;
			builder.ThumbnailUrl = tooltip.ThumbnailUrl;
			builder.Description = tooltip.Description;
			builder.Url = tooltip.ItemUrl;
			builder.WithFooter(tooltip.FooterText, tooltip.FooterIconUrl);

			return builder.Build();
		}

		public async Task<List<int>> GetItemIds(string itemName)
		{
			HttpClient client = GetHttpClient();

			Task<HttpResponseMessage> lookup = DoItemLookup(itemName, client);

			await Task.WhenAny(lookup, Task.Delay(TimeSpan.FromSeconds(10)));
			if (lookup.IsCompleted)
			{
				string html = await lookup.Result.Content.ReadAsStringAsync();
				return GetItemIdsFromHtml(itemName, html);
			}
			else
			{
				return new List<int>();
			}
		}

		protected virtual void ConfigureClient(HttpClient client)
		{
			client.DefaultRequestHeaders.UserAgent.Clear();
			ProductHeaderValue header = new ProductHeaderValue("DiscordDkpBot", Assembly.GetExecutingAssembly().GetName().Version.ToString());
			ProductInfoHeaderValue userAgent = new ProductInfoHeaderValue(header);
			client.DefaultRequestHeaders.UserAgent.Add(userAgent);
		}

		protected string ConvertToText(HtmlDocument htmlDoc)
		{
			//remove comments, head, style and script tags
			foreach (HtmlNode node in SafeSelectNodes(htmlDoc.DocumentNode, "//comment() | //script | //style | //head"))
			{
				node.ParentNode.RemoveChild(node);
			}

			//now remove all "meaningless" inline elements like "span"
			foreach (HtmlNode node in SafeSelectNodes(htmlDoc.DocumentNode, "//span | //label"))//add "b", "i" if required
			{
				node.ParentNode.ReplaceChild(HtmlNode.CreateNode(node.InnerHtml), node);
			}

			//block-elements - convert to line-breaks
			foreach (HtmlNode node in SafeSelectNodes(htmlDoc.DocumentNode, "//p | //div "))//you could add more tags here
			{
				//we add a "\n" ONLY if the node contains some plain text as "direct" child
				//meaning - text is not nested inside children, but only one-level deep

				//use XPath to find direct "text" in element
				HtmlNode txtNode = node.SelectSingleNode("text()");

				//no "direct" text - NOT ADDDING the \n !!!!
				if (txtNode == null || txtNode.InnerHtml.Trim() == "")
				{
					continue;
				}

				//"surround" the node with line breaks
				node.ParentNode.InsertBefore(htmlDoc.CreateTextNode("\n"), node);
				node.ParentNode.InsertAfter(htmlDoc.CreateTextNode("\n"), node);
			}

			foreach (HtmlNode node in SafeSelectNodes(htmlDoc.DocumentNode, " //table"))
			{
				node.ParentNode.InsertBefore(htmlDoc.CreateTextNode("\n"), node);
				node.ParentNode.InsertAfter(htmlDoc.CreateTextNode("\n"), node);
			}

			foreach (HtmlNode node in SafeSelectNodes(htmlDoc.DocumentNode, "//tr"))
			{
				node.ParentNode.InsertAfter(htmlDoc.CreateTextNode("\n"), node);
			}
			foreach (HtmlNode node in SafeSelectNodes(htmlDoc.DocumentNode, "//th"))
			{
				node.ParentNode.InsertBefore(htmlDoc.CreateTextNode("\n"), node);
			}

			foreach (HtmlNode node in SafeSelectNodes(htmlDoc.DocumentNode, "//br"))
			{
				node.ParentNode.ReplaceChild(htmlDoc.CreateTextNode("\n"), node);
			}

			ReplaceHyperlinks(htmlDoc);



			string text = htmlDoc.DocumentNode.InnerText.Trim();

			return text
				.Replace("\n\n\n", "\n\n")
				.Replace("\n\n\n", "\n\n")
				.Replace("\n\n\n", "\n\n")
				.Replace("&quot;", "\"")
				.Replace("&lt;", "<")
				.Replace("&gt;", ">");
		}

		protected abstract Task<HttpResponseMessage> DoItemLookup(string itemName, HttpClient client);

		protected abstract List<int> GetItemIdsFromHtml(string itemName, string html);

		protected abstract Task<ItemTooltip> GetTooltip(int itemId, HttpClient createClient);

		protected virtual string MakeAbsoluteLink(string relativeLink)
		{
			return $"{BaseUrl}/{relativeLink}";
		}

		protected abstract int ParseId(HtmlNode link);

		protected void ReplaceHyperlinks(HtmlDocument htmlDoc)
		{
			HtmlNodeCollection effects = htmlDoc.DocumentNode.SelectNodes("//a");
			if (effects?.Any() == true)
			{
				foreach (HtmlNode link in effects)
				{
					string relativeLink = link.Attributes["href"].Value.TrimStart('/');
					string absoluteLink = MakeAbsoluteLink(relativeLink);
					HtmlNode newNode = HtmlNode.CreateNode($"[{link.InnerText}]({absoluteLink})");
					link.ParentNode.ReplaceChild(newNode, link);
				}
			}
		}

		private HttpClient GetHttpClient()
		{
			HttpClient client = clientFactory.CreateClient();
			ConfigureClient(client);
			return client;
		}

		protected static HtmlNodeCollection SafeSelectNodes(HtmlNode node, string selector)
		{
			return node.SelectNodes(selector) ?? new HtmlNodeCollection(node);
		}
	}
}
