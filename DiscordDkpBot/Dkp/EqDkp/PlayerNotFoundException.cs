using System;

namespace DiscordDkpBot.Dkp.EqDkp
{
	public class PlayerNotFoundException : Exception
	{
		public PlayerNotFoundException (string player) : base($"Could not find player '{player}'.")
		{
		}
	}
}
