using System;
using System.Collections.Generic;
using System.IO;

namespace DiscordDkpBot.Dkp
{
	public class RaidWindowParser : IAttendanceParser
	{
		public IEnumerable<Character> Parse(string lines)
		{
			using (StringReader reader = new StringReader(lines))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					string[] split = line.Split('\t');
					//int group = int.Parse(split[0]);
					string name = split[1];
					int level = int.Parse(split[2]);
					string characterClass = split[3];

					yield return new Character(name, level, characterClass);
					;
				}
			}
		}
	}
}
