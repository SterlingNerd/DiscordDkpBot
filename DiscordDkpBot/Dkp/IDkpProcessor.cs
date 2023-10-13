using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DiscordDkpBot.Auctions;

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
		Task<RaidInfo> StartRaid(int EventId, IEnumerable<int> characterIds, int value, string note);		
	}

	public class DkpEvent {
		public DkpEvent (int id, string name, decimal value)
		{
			
		}

		public string Name { get; set; }
	}

	public class DkpItem {
		public DkpItem (int bidCharacterId, DateTime now, string auctionName, int winnerPrice, int raidId) {
			throw new NotImplementedException();
		}
	}
}
