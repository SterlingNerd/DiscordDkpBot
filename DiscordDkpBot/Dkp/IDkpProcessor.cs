using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Dkp.EqDkpPlus.Xml;

namespace DiscordDkpBot.Dkp
{
	public interface IDkpProcessor
	{
		Task<RaidInfo> AddDkp(int eventId, int value, string comment, string lines);
		Task ChargeWinners(CompletedAuction completedAuction);
		Task<int> GetCharacterId(string characterName);
		Task<RaidInfo> GetDailyItemsRaid();
		Task<PlayerPoints> GetDkp(string characterName);
		Task<PlayerPoints> GetDkp(int characterId);
		Task<IEnumerable<DkpEvent>> GetEvents(string name = null);
		Task<RaidInfo> StartRaid(int EventId, string note);
		Task<RaidInfo> UseRaid(int raidId);
	}
}
