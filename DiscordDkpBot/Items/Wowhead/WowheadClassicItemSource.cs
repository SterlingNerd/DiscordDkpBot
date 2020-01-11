using System;
using System.Net.Http;

namespace DiscordDkpBot.Items.Wowhead
{
	public class WowheadClassicItemSource : WowheadItemSource
	{
		protected override string BaseUrl => "https://classic.wowhead.com";

		public WowheadClassicItemSource(IHttpClientFactory clientFactory) : base(clientFactory)
		{
		}
	}
}
