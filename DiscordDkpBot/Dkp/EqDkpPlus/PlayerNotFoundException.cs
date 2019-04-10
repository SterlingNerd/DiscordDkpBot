using System;

namespace DiscordDkpBot.Dkp.EqDkpPlus
{
	public class PlayerNotFoundException : DkpBotException
	{
		public PlayerNotFoundException (string player) : base($"Could not find player '{player}'.")
		{
		}
	}
}
