using System;
using System.Globalization;

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

		public static string UppercaseFirst (this string str)
		{
			if (string.IsNullOrWhiteSpace(str))
			{
				return str;
			}
			return str[0].ToString().ToUpper(CultureInfo.CurrentCulture) + (str.Length > 1 ? str.Substring(1) : string.Empty);
		}
	}
}
