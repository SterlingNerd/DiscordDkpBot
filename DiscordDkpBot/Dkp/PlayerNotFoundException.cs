using System;

namespace DiscordDkpBot.Dkp
{
	public class PlayerNotFoundException : DkpBotException
	{
		public PlayerNotFoundException (string player) : base($"Could not find player '{player}'.")
		{
		}
		public PlayerNotFoundException (int playerId) : base($"Could not find player with id '{playerId}'.")
		{
		}
	}
}
