using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DiscordDkpBot.Dkp.EqDkp.Xml;

namespace DiscordDkpBot.Dkp
{
	public interface IDkpProcessor
	{
		Task<PlayerPoints> GetDkp (string character);
	}
}