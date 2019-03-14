using System;

namespace DiscordDkpBot.Extensions
{
	public static class StringExtensions
	{
		public static string RemoveFirstWord (this string str)
		{
			{
				if (str == null)
				{
					return null;
				}

				return str.IndexOf(' ', StringComparison.OrdinalIgnoreCase) > -1
					? str.Substring(str.IndexOf(' ', StringComparison.OrdinalIgnoreCase))
					: string.Empty;
			}
		}
	}
}
