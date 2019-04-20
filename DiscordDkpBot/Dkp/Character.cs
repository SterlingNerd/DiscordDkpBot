using System;

namespace DiscordDkpBot.Dkp
{
	public class Character
	{
		public string Class { get; set; }
		public int Level { get; set; }
		public string Name { get; set; }

		public Character(string name, int level, string characterClass)
		{
			Name = name;
			Level = level;
			Class = characterClass;
		}
	}
}
