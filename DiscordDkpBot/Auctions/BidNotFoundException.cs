using System;

using DiscordDkpBot.Dkp;

namespace DiscordDkpBot.Auctions
{
	public class BidNotFoundException : DkpBotException
	{
		public BidNotFoundException (string item) : base($"Could not find your bid on item \"{item}\".")
		{
		}
	}
}
