using System;

using DiscordDkpBot.Commands;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Extensions
{
	internal static class LoggerExtensions
	{
		public static void LogError<T> (this ILogger<T> logger, Exception ex)
		{
			logger.LogError(ex, ex.Message);
		}

		public static void LogError (this ILogger<ICommand> logger, string message)
		{
			logger.LogError("{0}: {1}", nameof(ICommand), message);
		}

		public static void LogError (this ILogger<ICommand> logger, Exception ex)
		{
			logger.LogError(ex, "{0}: {1}", nameof(ICommand), ex.Message);
		}

		public static void LogInformation (this ILogger<ICommand> logger, string message)
		{
			logger.LogInformation("{0}: {1}", nameof(ICommand), message);
		}

		public static void LogTrace (this ILogger<ICommand> logger, string message)
		{
			logger.LogTrace("{0}: {1}", nameof(ICommand), message);
		}

		public static void LogWarning<T> (this ILogger<T> logger, Exception ex)
		{
			logger.LogWarning(ex, ex.Message);
		}

		public static void LogWarning (this ILogger<ICommand> logger, string message)
		{
			logger.LogWarning("{0}: {1}", nameof(ICommand), message);
		}

		public static void LogWarning (this ILogger<ICommand> logger, Exception ex)
		{
			logger.LogWarning(ex, "{0}: {1}", nameof(ICommand), ex.Message);
		}
	}
}
