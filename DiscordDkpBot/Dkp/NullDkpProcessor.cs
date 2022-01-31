using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Dkp.EqDkpPlus.Xml;

namespace DiscordDkpBot.Dkp
{
	public class NullDkpProcessor : IDkpProcessor
	{
		public Task<RaidInfo> AddDkp(int eventId, int value, string comment, string lines)
		{
			return Task.FromResult(FakeRaid);
		}

		public Task ChargeWinners(CompletedAuction completedAuction)
		{
			return Task.CompletedTask;
		}

		public Task<int> GetCharacterId(string characterName)
		{
			return Task.FromResult(characterName.GetHashCode());
		}

		public Task<RaidInfo> GetDailyItemsRaid()
		{
			return Task.FromResult(FakeRaid);
		}

		public Task<PlayerPoints> GetDkp(string characterName)
		{
			return Task.FromResult(FakePlayerPoints);
		}

		private static readonly RaidInfo FakeRaid = new RaidInfo
		{
			EventId = 1,
			AddedById = 1,
			AddedByName = "Fake",
			DateString = "",
			DateTimestamp = 1,
			EventName = "Fake",
			Id = 1,
			Value = 1
		};

		private static readonly PlayerPoints FakePlayerPoints = new PlayerPoints
		{
			DkpPoolId = 1,
			PointsAdjustment = 0,
			PointsAdjustmentWithTwink = 0,
			PointsCurrent = int.MaxValue,
			PointsCurrentWithTwink = int.MaxValue,
			PointsEarned = 0,
			PointsEarnedWithTwink = 0,
			PointsSpent = 0,
			PointsSpentWithTwink = 0
		};

		public Task<PlayerPoints> GetDkp(int characterId)
		{
			return Task.FromResult(FakePlayerPoints);
		}

		public Task<IEnumerable<DkpEvent>> GetEvents(string name = null)
		{
			return Task.FromResult<IEnumerable<DkpEvent>>(Array.Empty<DkpEvent>());
		}

		public Task<RaidInfo> StartRaid(int EventId, IEnumerable<int> characterIds, int value, string note)
		{
			return Task.FromResult(FakeRaid);
		}

		public Task<RaidInfo> UseRaid(int raidId)
		{
			return Task.FromResult(FakeRaid);
		}
	}
}
