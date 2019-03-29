using System;

namespace DiscordDkpBot.Dkp.EqDkpPlus
{
	public class PlayerNotFoundException : Exception
	{
		public PlayerNotFoundException (string player) : base($"Could not find player '{player}'.")
		{
		}
	}
}
