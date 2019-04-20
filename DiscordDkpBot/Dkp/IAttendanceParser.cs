using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordDkpBot.Dkp
{
	public interface IAttendanceParser
	{
		IEnumerable<Character> Parse(string lines);
	}
}
