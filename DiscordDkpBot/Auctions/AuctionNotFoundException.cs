using System;

namespace DiscordDkpBot.Auctions
{
	public class AuctionNotFoundException : Exception
	{
		public AuctionNotFoundException (string item) : base($"Could not find auction \"{item}\".")
		{
		}
	}
}
