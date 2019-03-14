using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordDkpBot.Extensions
{
	public static class CollectionExtensions
	{
		public static bool None<T> (this ICollection<T> collection)
		{
			return collection?.Any() == false;
		}
	}
}
