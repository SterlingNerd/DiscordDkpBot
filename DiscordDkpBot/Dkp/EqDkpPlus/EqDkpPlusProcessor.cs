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
		private readonly EqDkpPlusClient client;
		private readonly EqDkpPlusConfiguration config;
		private readonly ILogger<EqDkpPlusProcessor> log;
		private readonly AuctionState state;

		public EqDkpPlusProcessor(EqDkpPlusConfiguration config, EqDkpPlusClient client, AuctionState state, ILogger<EqDkpPlusProcessor> log)
		{
			this.config = config;
			this.client = client;
			this.state = state;
			this.log = log;
		}

		public async Task ChargeWinners(CompletedAuction completedAuction)
		{
			List<Task> chargeTasks = new List<Task>();

			foreach (WinningBid winner in completedAuction.WinningBids)
			{
				chargeTasks.Add(client.AddItem(winner.Bid.CharacterId, DateTime.Now, completedAuction.Auction.Name, winner.Price, completedAuction.Auction.Raid.Id));
			}

			await Task.WhenAll(chargeTasks);
		}

		public async Task<int> GetCharacterId(string character)
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
			return id;
		}

		public async Task<PlayerPoints> GetDkp(string character)
		{
			int id = await GetCharacterId(character);
			return await GetDkp(id);
		}

		public async Task<PlayerPoints> GetDkp(int id)
		{
			PointsResponse points = await client.GetPoints(id);
			Player player = points?.Players?.FirstOrDefault();
			if (player?.MainId != id)
			{
				log.LogInformation($"Character id '{id}' is an alt. Getting points for main.");
				points = await client.GetPoints(player?.MainId);
				player = points?.Players?.FirstOrDefault();
			}

			return player?.Points?.FirstOrDefault() ?? throw new PlayerNotFoundException($"Player id '{id}' not found.");
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

		public async Task<RaidInfo> StartRaid(int eventId, string creator)
		{
			AddRaidResponse response = await client.AddRaid(DateTimeOffset.Now, eventId, $"Created by {creator}.");
			GetRaidsResponse raids = await client.GetRaids(10, 0);//Hopefully it's in the last 10 raids.

			RaidInfo raid;
			if (config.AddRaidUri.Contains("test=true"))
			{
				// We're testing, so just return the last raid.
				raid = raids.Raids.First();
			}
			else
			{
				raid = raids.Raids.Single(x => x.Id == response.Id);
			}

			state.Raids.TryAdd(raid.Id, raid);
			state.CurrentRaid = raid;

			return raid;
		}

		public async Task UpdatePlayerIds()
		{
			PointsResponse points = await client.GetPoints();
			state.PlayerIds = new ReadOnlyDictionary<string, int>(points.Players.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase));
		}
	}
}
