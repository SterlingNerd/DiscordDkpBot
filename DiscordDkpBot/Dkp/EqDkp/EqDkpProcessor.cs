using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Dkp.EqDkp.Xml;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Dkp.EqDkp
{
	public class EqDkpProcessor : IDkpProcessor
	{
		private readonly EqDkpClient client;
		private readonly AuctionState state;
		private readonly ILogger<EqDkpProcessor> log;

		public EqDkpProcessor (EqDkpClient client, AuctionState state, ILogger<EqDkpProcessor> log)
		{
			this.client = client;
			this.state = state;
			this.log = log;
		}

		public async Task<PlayerPoints> GetDkp (string character)
		{
			if (!state.PlayerIds.TryGetValue(character, out int id))
			{
				log.LogInformation("Getting new player list.");
				await UpdatePlayerIds();
				if (!state.PlayerIds.TryGetValue(character, out id))
				{
					log.LogWarning($"player {character} not in list.");
					throw new PlayerNotFoundException(character);
				}
			}
			PointsResponse points = await client.GetPoints(id);
			var player = points?.Players?.FirstOrDefault();
			if (player?.MainId != id)
			{
				log.LogInformation($"{character} is an alt. Getting points for main.");
				points = await client.GetPoints(player?.MainId);
				player = points?.Players?.FirstOrDefault();
			}

			return player?.Points?.FirstOrDefault() ?? throw new PlayerNotFoundException($"Player {character} not found.");
		}

		public async Task UpdatePlayerIds ()
		{
			PointsResponse points = await client.GetPoints();
			state.PlayerIds = new ReadOnlyDictionary<string, int>(points.Players.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase));
		}
	}
}
