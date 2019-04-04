using System;

using Discord;

namespace DiscordDkpBot.Items
{
	public class ItemLookupResult
	{
		public Embed Embed { get; }
		public int MatchesFound { get; }

		public ItemLookupResult(Embed embed, int matchesFound)
		{
			Embed = embed;
			MatchesFound = matchesFound;
		}
	}
}
