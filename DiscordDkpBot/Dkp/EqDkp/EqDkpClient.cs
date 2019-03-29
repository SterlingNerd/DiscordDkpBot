using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using DiscordDkpBot.Configuration;
using DiscordDkpBot.Dkp.EqDkp.Xml;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Dkp.EqDkp
{
	public class EqDkpClient
	{
		private readonly IHttpClientFactory clientFactory;
		private readonly ILogger<EqDkpClient> log;
		private readonly DkpBotConfiguration config;
		private readonly XmlSerializer pointsSerializer = new XmlSerializer(typeof(PointsResponse));

		public EqDkpClient (DkpBotConfiguration config, IHttpClientFactory clientFactory, ILogger<EqDkpClient> log)
		{
			this.config = config;
			this.clientFactory = clientFactory;
			this.log = log;
		}

		public async Task<PointsResponse> GetPoints (int? playerId = null)
		{
			log.LogInformation($"Getting dkp for playerId:{playerId}.");


			StringBuilder uri = new StringBuilder(config.EqDkp.PointsUri);
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

		//	StringBuilder uri = new StringBuilder(config.EqDkp.AddRaidUri);

		//	AddRaidRequest request = new AddRaidRequest()


		//	var client = GetClient();

			
		//}

		private HttpClient GetClient ()
		{
			HttpClient client = clientFactory.CreateClient(nameof(EqDkpClient));

			client.BaseAddress = new Uri(config.EqDkp.BaseAddress);
			client.DefaultRequestHeaders.Add("Accept", "application/xml");
			client.DefaultRequestHeaders.Add("X-Custom-Authorization", $"token={config.EqDkp.Token}&type=user");
			client.DefaultRequestHeaders.Add("User-Agent", $"DiscordDkpBot-{config.Version}");

			return client;
		}
	}
}
