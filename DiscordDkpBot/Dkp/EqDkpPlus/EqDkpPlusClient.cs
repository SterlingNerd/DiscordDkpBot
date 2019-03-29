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

		public EqDkpPlusClient(DkpBotConfiguration config, IHttpClientFactory clientFactory, ILogger<EqDkpPlusClient> log)
		{
			this.config = config;
			this.clientFactory = clientFactory;
			this.log = log;
		}

		public Task<EventsResponse> GetEvents()
		{
			log.LogInformation("Getting events list.");

			StringBuilder uri = new StringBuilder(config.EqDkpPlus.EventsUri);

			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri.ToString());
			return SendAsync<EventsResponse>(request);
		}

		public Task<PointsResponse> GetPoints(int? playerId = null)
		{
			log.LogInformation($"Getting dkp for playerId:{playerId}.");

			StringBuilder uri = new StringBuilder(config.EqDkpPlus.PointsUri);
			if (playerId != null)
			{
				uri.Append($"&filter=character&filterid={playerId}");
			}

			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri.ToString());
			return SendAsync<PointsResponse>(request);
		}

		public async Task<T> SendAsync<T>(HttpRequestMessage request) where T : class
		{
			HttpClient client = GetClient();

			HttpResponseMessage response = await client.SendAsync(request);
			if (response.IsSuccessStatusCode)
			{
				//Stream xml = await response.Content.ReadAsStreamAsync();
				//return pointsSerializer.Deserialize(xml) as PointsResponse;
				string xml = await response.Content.ReadAsStringAsync();
				XmlSerializer serializer;

				// But did we actually succeed?
				if (xml?.Contains("</error>\r\n<response>") == true)
				{
					log.LogWarning($"{request.RequestUri} Failed.\t{xml}");

					serializer = new XmlSerializer(typeof(ErrorResponse));

					using (StringReader reader = new StringReader(xml))
					{
						ErrorResponse error = serializer.Deserialize(reader) as ErrorResponse;
						throw new EqDkpPlusException(error);
					}
				}
				else
				{
					log.LogDebug($"{request.RequestUri} success.");

					serializer = new XmlSerializer(typeof(T));

					using (StringReader reader = new StringReader(xml))
					{
						return serializer.Deserialize(reader) as T;
					}
				}
			}
			else
			{
				throw new ApplicationException($"EqDkpPlus Api call failed: {response.StatusCode} {await response.Content.ReadAsStringAsync()}");
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
