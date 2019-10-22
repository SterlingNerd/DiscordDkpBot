using System;
using System.Threading;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using DiscordDkpBot.Commands;
using DiscordDkpBot.Configuration;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot
{
	public class DkpBot
	{
		private readonly DiscordSocketClient client;
		private readonly ICommandProcessor commandProcessor;
		private readonly ILogger<DkpBot> log;
		private readonly string token;

		public DkpBot (DkpBotConfiguration configuration, DiscordSocketClient client, ICommandProcessor commandProcessor, ILogger<DkpBot> log)
		{
			this.client = client;
			this.commandProcessor = commandProcessor;
			this.log = log;

			token = configuration.Discord.Token;

			this.client.Log += LogAsync;
			this.client.Ready += ReadyAsync;
			this.client.MessageReceived += MessageReceivedAsync;
		}

		public async Task Run ()
		{
			await client.LoginAsync(TokenType.Bot, token);
			await client.StartAsync();

			// Block the program until it is closed.
			await Task.Delay(Timeout.Infinite);
		}

		private static LogLevel ConvertSeverity (LogSeverity messageSeverity)
		{
			switch (messageSeverity)
			{
				case LogSeverity.Critical:
					return LogLevel.Critical;

				case LogSeverity.Error:
					return LogLevel.Error;
				case LogSeverity.Warning:
					return LogLevel.Warning;
				case LogSeverity.Info:
					return LogLevel.Information;
				case LogSeverity.Verbose:
					return LogLevel.Trace;
				case LogSeverity.Debug:
					return LogLevel.Debug;
				default:
					throw new ArgumentOutOfRangeException(nameof(messageSeverity), messageSeverity, null);
			}
		}

		private Task LogAsync (LogMessage message)
		{
			log.Log(ConvertSeverity(message.Severity), message.ToString());
			return Task.CompletedTask;
		}

		// This is not the recommended way to write a bot - consider
		// reading over the Commands Framework sample.
		private Task MessageReceivedAsync (SocketMessage message)
		{
			// The bot should never respond to itself.
			if (message.Author.Id == client.CurrentUser.Id)
			{
				return Task.CompletedTask;
			}

			Task.Run(() => ProcessCommand(message));
			return Task.CompletedTask;
		}

		private async Task ProcessCommand (SocketMessage message)
		{
			CancellationTokenSource src = new CancellationTokenSource();

			Task task = Task.Run(() => commandProcessor.ProcessCommand(message), src.Token);

			if (Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(30))) == task)
			{
				// ok.
			}
			else
			{
				src.Cancel();
				await message.Channel.SendMessageAsync($"'{message.Content}' took too long to process.");
				log.LogError($"Timed out: '{message.Content}'");
			}
		}

		// The Ready event indicates that the client has opened a
		// connection and it is now safe to access the cache.
		private Task ReadyAsync ()
		{
			log.LogInformation($"{client.CurrentUser} is connected!");

			return Task.CompletedTask;
		}
	}
}
