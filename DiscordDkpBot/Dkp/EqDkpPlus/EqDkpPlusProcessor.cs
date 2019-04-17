using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;

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

		public EqDkpPlusProcessor (EqDkpPlusConfiguration config, EqDkpPlusClient client, AuctionState state, ILogger<EqDkpPlusProcessor> log)
		{
			this.config = config;
			this.client = client;
			this.state = state;
			this.log = log;
		}

		public async Task ChargeWinners (CompletedAuction completedAuction)
		{
			List<Task> chargeTasks = new List<Task>();

			foreach (WinningBid winner in completedAuction.WinningBids)
			{
				chargeTasks.Add(client.AddItem(winner.Bid.CharacterId, DateTime.Now, completedAuction.Auction.Name, winner.Price, completedAuction.Auction.Raid.Id));
			}

			await Task.WhenAll(chargeTasks);
		}

		public async Task<int> GetCharacterId (string character)
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

		public async Task<PlayerPoints> GetDkp (string character)
		{
			int id = await GetCharacterId(character);
			return await GetDkp(id);
		}

		public async Task<PlayerPoints> GetDkp (int id)
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

		public async Task<IEnumerable<DkpEvent>> GetEvents (string name = null)
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

		public async Task<RaidInfo> StartRaid (int eventId, string creator)
		{
			AddRaidResponse response = await client.AddRaid(DateTimeOffset.Now, eventId, $"Created by {creator}.");
			RaidInfo raid;
			if (config.AddRaidUri.Contains("test=true"))
			{
				// We're testing, so just return the last raid.
				raid = (await client.GetRaids(1, 0)).FirstOrDefault();
			}
			else
			{
				raid = await GetRaid(response.Id);
			}

			state.Raids.TryAdd(raid.Id, raid);
			state.CurrentRaid = raid;

			return raid;
		}

		public async Task UpdatePlayerIds ()
		{
			PointsResponse points = await client.GetPoints();
			state.PlayerIds = new ReadOnlyDictionary<string, int>(points.Players.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase));
		}

		public async Task<RaidInfo> UseRaid (int raidId, IMessage message)
		{
			RaidInfo raid = await GetRaid(raidId);
			state.CurrentRaid = raid;

			return raid;
		}

		private async Task<RaidInfo> GetRaid (int raidId)
		{
			//Maybe it's in the most recent 10?
			RaidInfo[] raids = await client.GetRaids(10, 0);
			RaidInfo raid = raids?.FirstOrDefault(x => x.Id == raidId);

			if (raid != null && raid.Id == raidId)
			{
				return raid;
			}
			else
			{
				// We could try to get fancy and binary search this thing.
				// However, requests have overhead, and we don't actually know how many raids there are (there could be gaps in IDs)
				// So until proven otherwise, getting the whole damn list of raids is probably just as quick.
				raids = await client.GetAllRaids();
				raid = raids?.FirstOrDefault(x => x.Id == raidId);

				if (raid != null)
				{
					return raid;
				}
				else
				{
					throw new RaidNotFoundException($"Could not find raid id '{raidId}'.");
				}

			}
		}
	}
}
