using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Discord;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Dkp.EqDkpPlus.Xml;

namespace DiscordDkpBot.Dkp
{
	public interface IDkpProcessor
	{
		Task ChargeWinners(CompletedAuction completedAuction);
		Task<int> GetCharacterId(string characterName);
		Task<PlayerPoints> GetDkp(string characterName);
		Task<PlayerPoints> GetDkp(int characterId);
		Task<IEnumerable<DkpEvent>> GetEvents(string name = null);
		Task<RaidInfo> StartRaid(int EventId, string creator);
		Task<RaidInfo> UseRaid(int raidId);
	}
}
