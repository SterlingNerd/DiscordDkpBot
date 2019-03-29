using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Configuration;
using DiscordDkpBot.Dkp.EqDkpPlus.Xml;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Dkp.EqDkpPlus
{
	public class EqDkpPlusProcessor : IDkpProcessor
	{
		private readonly EqDkpPlusConfiguration config;
		private readonly EqDkpPlusClient client;
		private readonly ILogger<EqDkpPlusProcessor> log;
		private readonly AuctionState state;

		public EqDkpPlusProcessor(EqDkpPlusConfiguration config, EqDkpPlusClient client, AuctionState state, ILogger<EqDkpPlusProcessor> log)
		{
			this.config = config;
			this.client = client;
			this.state = state;
			this.log = log;
		}

		public async Task<PlayerPoints> GetDkp(string character)
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
			Player player = points?.Players?.FirstOrDefault();
			if (player?.MainId != id)
			{
				log.LogInformation($"{character} is an alt. Getting points for main.");
				points = await client.GetPoints(player?.MainId);
				player = points?.Players?.FirstOrDefault();
			}

			return player?.Points?.FirstOrDefault() ?? throw new PlayerNotFoundException($"Player {character} not found.");
		}

		public async Task<IEnumerable<DkpEvent>> GetEvents(string name = null)
		{
			EventsResponse response = await client.GetEvents();
			if (!string.IsNullOrWhiteSpace(name))
			{
				return response?.Events?.Where(x => x?.Name?.Contains(name, StringComparison.OrdinalIgnoreCase) == true);
			}
			else
			{
				return response?.Events?.Where(x => config.FavoriteEvents?.Contains(x.Id) == true);
			}
		}

		public async Task UpdatePlayerIds()
		{
			PointsResponse points = await client.GetPoints();
			state.PlayerIds = new ReadOnlyDictionary<string, int>(points.Players.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase));
		}
	}
}
