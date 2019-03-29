using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DiscordDkpBot.Dkp.EqDkpPlus.Xml;

namespace DiscordDkpBot.Dkp
{
	public interface IDkpProcessor
	{
		Task<PlayerPoints> GetDkp(string character);
		Task<IEnumerable<DkpEvent>> GetEvents(string name = null);
	}
}
