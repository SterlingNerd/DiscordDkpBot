using System;

namespace DiscordDkpBot.Auctions
{
	public class AuctionNotFoundException : DkpBotException
	{
		public AuctionNotFoundException (string item) : base($"Could not find auction \"{item}\".")
		{
		}
	}
}
