using System;

namespace DiscordDkpBot.Auctions
{
	public class BidNotFoundException : Exception
	{
		public BidNotFoundException (string item) : base($"Could not find your bid on item \"{item}\".")
		{
		}
	}
}
