using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Configuration;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Dkp.NullDkp
{
	public class NullDkpProcessor : IDkpProcessor
	{
		private readonly List<DkpEvent> Events = new();
		private readonly List<DkpItem> Items = new();
		private readonly ILogger<NullDkpProcessor> log;
		private readonly IAttendanceParser parser;

		private readonly Dictionary<int, DkpPlayer> Players = new();

		private readonly Dictionary<int, RaidInfo> Raids = new();

		public NullDkpProcessor (EqDkpPlusConfiguration config, IAttendanceParser parser, ILogger<NullDkpProcessor> log)
		{
			this.parser = parser;
			this.log = log;
		}

		public async Task<RaidInfo> AddDkp (int eventId, int value, string comment, string lines)
		{
			IEnumerable<Character> chars = this.parser.Parse(lines);
			List<int> characterIds = await this.GetCharacterIds(chars.Select(x => x.Name));
			int nextRaidId = this.GetNextRaidId();
			RaidInfo raid = new(nextRaidId, DateTime.Today, eventId, value, comment);
			this.Raids.Add(nextRaidId, raid);
			return raid;
		}

		public Task ChargeWinners (CompletedAuction completedAuction)
		{
			foreach (WinningBid winner in completedAuction.WinningBids)
			{
				DkpPlayer player = this.Players[winner.Bid.CharacterId] ?? throw new PlayerNotFoundException($"Player {winner.Bid.CharacterId} not found.");
				player.ChargeDkp(winner.Price);
				this.Items.Add(new(winner.Bid.CharacterId, DateTime.Now, completedAuction.Auction.Name, winner.Price, completedAuction.Auction.Raid.Id));
			}

			return Task.CompletedTask;
		}

		public Task<int> GetCharacterId (string character)
		{
			return Task.FromResult(this.Players.Values.Where(x => x.Name == character)?.FirstOrDefault()?.Id ?? throw new PlayerNotFoundException(character));
		}

		public async Task<List<int>> GetCharacterIds (IEnumerable<string> names)
		{
			List<int> characterIds = new();
			foreach (string name in names)
			{
				characterIds.Add(await this.GetCharacterId(name));
			}
			return characterIds;
		}

		public async Task<RaidInfo> GetDailyItemsRaid ()
		{
			DateTime today = DateTime.Today.Date;

			RaidInfo todaysRaid = this.Raids.Values.FirstOrDefault(x => x.Date == today);

			if (todaysRaid is null)
			{
				todaysRaid = new(this.GetNextRaidId(), today, 1, 0, $"Items for {today.ToShortDateString()}");
				this.Raids.Add(todaysRaid.Id, todaysRaid);
			}

			return todaysRaid;
		}

		public async Task<PlayerPoints> GetDkp (string character)
		{
			int id = await this.GetCharacterId(character);
			return await this.GetDkp(id);
		}

		public Task<PlayerPoints> GetDkp (int id)
		{
			DkpPlayer player = this.Players[id] ?? throw new PlayerNotFoundException(id);

			if (player?.MainId != id)
			{
				this.log.LogInformation($"Character id '{id}' is an alt. Getting points for main.");
				player = this.Players[player.MainId];
			}

			return Task.FromResult(player.Points);
		}

		public Task<IEnumerable<DkpEvent>> GetEvents (string name = null)
		{
			if (!string.IsNullOrWhiteSpace(name))
			{
				return Task.FromResult(this.Events?.Where(x => x?.Name?.Contains(name, StringComparison.OrdinalIgnoreCase) == true));
			} else
			{
				return Task.FromResult<IEnumerable<DkpEvent>>(this.Events);
			}
		}

		public async Task<RaidInfo> StartRaid (int eventId, IEnumerable<int> characterIds, int value, string note)
		{
			RaidInfo raid = new(this.GetNextRaidId(), DateTime.Now, eventId, value, note);
			this.Raids.TryAdd(raid.Id, raid);
			return raid;
		}

		private int GetNextRaidId ()
		{
			return this.Raids.Keys.Max() + 1;
		}
	}
}
