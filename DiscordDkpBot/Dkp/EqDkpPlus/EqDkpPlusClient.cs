using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using DiscordDkpBot.Configuration;
using DiscordDkpBot.Dkp.EqDkpPlus.Xml;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Dkp.EqDkpPlus
{
	public class EqDkpPlusClient
	{
		private readonly IHttpClientFactory clientFactory;
		private readonly DkpBotConfiguration config;
		private readonly ILogger<EqDkpPlusClient> log;
		private readonly XmlSerializer pointsSerializer = new XmlSerializer(typeof(PointsResponse));

		public EqDkpPlusClient(DkpBotConfiguration config, IHttpClientFactory clientFactory, ILogger<EqDkpPlusClient> log)
		{
			this.config = config;
			this.clientFactory = clientFactory;
			this.log = log;
		}

		public async Task<EventsResponse> GetEvents()
		{
			log.LogInformation("Getting events list.");

			StringBuilder uri = new StringBuilder(config.EqDkpPlus.EventsUri);

			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri.ToString());

			HttpClient client = GetClient();
			HttpResponseMessage response = await client.SendAsync(request);

			if (response.IsSuccessStatusCode)
			{
				//Stream xml = await response.Content.ReadAsStreamAsync();
				//return pointsSerializer.Deserialize(xml) as PointsResponse;
				string xml = await response.Content.ReadAsStringAsync();
				return pointsSerializer.Deserialize(new StringReader(xml)) as EventsResponse;
			}
			else
			{
				throw new ApplicationException($"GetEvents Failed: {response.StatusCode} {await response.Content.ReadAsStringAsync()}");
			}
		}

		public async Task<PointsResponse> GetPoints(int? playerId = null)
		{
			log.LogInformation($"Getting dkp for playerId:{playerId}.");

			StringBuilder uri = new StringBuilder(config.EqDkpPlus.PointsUri);
			if (playerId != null)
			{
				uri.Append($"&filter=character&filterid={playerId}");
			}

			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri.ToString());

			HttpClient client = GetClient();
			HttpResponseMessage response = await client.SendAsync(request);

			if (response.IsSuccessStatusCode)
			{
				//Stream xml = await response.Content.ReadAsStreamAsync();
				//return pointsSerializer.Deserialize(xml) as PointsResponse;
				string xml = await response.Content.ReadAsStringAsync();
				return pointsSerializer.Deserialize(new StringReader(xml)) as PointsResponse;
			}
			else
			{
				throw new ApplicationException($"GetPoints Failed: {response.StatusCode} {await response.Content.ReadAsStringAsync()}");
			}
		}

		//public async Task CreateRaid(DateTimeOffset date, )
		//{
		//	log.LogInformation($"Creating raid: ");

		//	StringBuilder uri = new StringBuilder(config.EqDkpPlus.AddRaidUri);

		//	AddRaidRequest request = new AddRaidRequest()

		//	var client = GetClient();

		//}

		private HttpClient GetClient()
		{
			HttpClient client = clientFactory.CreateClient(nameof(EqDkpPlusClient));

			client.BaseAddress = new Uri(config.EqDkpPlus.BaseAddress);
			client.DefaultRequestHeaders.Add("Accept", "application/xml");
			client.DefaultRequestHeaders.Add("X-Custom-Authorization", $"token={config.EqDkpPlus.Token}&type=user");
			client.DefaultRequestHeaders.Add("User-Agent", $"DiscordDkpBot-{config.Version}");

			return client;
		}
	}
}
