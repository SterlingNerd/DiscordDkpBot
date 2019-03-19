﻿using System;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Extensions
{
	internal static class LoggerExtensions
	{
		public static void LogError<T> (this ILogger<T> logger, Exception ex)
		{
			logger.LogError(ex, ex.Message);
		}

		public static void LogWarning<T> (this ILogger<T> logger, Exception ex)
		{
			logger.LogWarning(ex, ex.Message);
		}
	}
}
