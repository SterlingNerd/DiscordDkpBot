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

		public Task AddItem(int buyer, DateTime itemDate, string itemName, int value, int raidId)
		{
			AddItemRequest item = new AddItemRequest(buyer, itemDate, itemName, 1, raidId, value);
			log.LogInformation($"Creating item: {item}");

			StringBuilder uri = new StringBuilder(config.EqDkpPlus.AddItemUri);
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri.ToString());

			return SendAsync<AddItemRequest, AddItemResponse>(request, item);

		}

		public Task<AddRaidResponse> AddRaid(DateTimeOffset date, int eventId, string note)
		{
			AddRaidRequest addRequest = new AddRaidRequest(date.UtcDateTime, eventId, note);

			log.LogInformation($"Creating raid: {eventId} @ {date.LocalDateTime} \"{note}\"");

			StringBuilder uri = new StringBuilder(config.EqDkpPlus.AddRaidUri);
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri.ToString());

			return SendAsync<AddRaidRequest, AddRaidResponse>(request, addRequest);
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

		public Task<GetRaidsResponse> GetRaids(int number, int start)
		{
			log.LogInformation("Getting raids.");

			StringBuilder uri = new StringBuilder(config.EqDkpPlus.GetRaidsUri);
			uri.Append($"&number={number}&start={start}");

			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri.ToString());
			return SendAsync<GetRaidsResponse>(request);
		}

		private HttpClient GetClient()
		{
			HttpClient client = clientFactory.CreateClient(nameof(EqDkpPlusClient));

			client.BaseAddress = new Uri(config.EqDkpPlus.BaseAddress);
			client.DefaultRequestHeaders.Add("Accept", "application/xml");
			client.DefaultRequestHeaders.Add("X-Custom-Authorization", $"token={config.EqDkpPlus.Token}&type=user");
			client.DefaultRequestHeaders.Add("User-Agent", $"DiscordDkpBot-{config.Version}");

			return client;
		}

		private async Task<TResponse> SendAsync<TRequest, TResponse>(HttpRequestMessage request, TRequest content) where TResponse : class
		{
			if (content != null)
			{
				XmlSerializer ser = new XmlSerializer(typeof(TRequest));

				using (MemoryStream stream = new MemoryStream())
				{
					ser.Serialize(stream, content);
					stream.Position = 0;
					return await SendAsync<TResponse>(request, stream);
				}
			}
			else
			{
				return await SendAsync<TResponse>(request);
			}
		}

		private async Task<TResponse> SendAsync<TResponse>(HttpRequestMessage request, Stream content = null) where TResponse : class
		{
			HttpClient client = GetClient();

			if (content != null)
			{
				request.Content = new StreamContent(content);
			}

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

					serializer = new XmlSerializer(typeof(TResponse));

					using (StringReader reader = new StringReader(xml))
					{
						return serializer.Deserialize(reader) as TResponse;
					}
				}
			}
			else
			{
				throw new ApplicationException($"EqDkpPlus Api call failed: {response.StatusCode} {await response.Content.ReadAsStringAsync()}");
			}
		}
	}
}
