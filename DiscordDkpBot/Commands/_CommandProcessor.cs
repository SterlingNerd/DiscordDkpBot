using System;
using System.Threading.Tasks;

using Discord.WebSocket;

using DiscordDkpBot.Configuration;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Commands
{
	public interface ICommandProcessor
	{
		Task ProcessCommand (SocketMessage message);
	}

	public class CommandProcessor : ICommandProcessor
	{
		private readonly string commandPrefix;
		private readonly CommandCollection commands;
		private readonly ILogger<CommandProcessor> log;

		public CommandProcessor (DkpBotConfiguration configuration, CommandCollection commands, ILogger<CommandProcessor> log)
		{
			commandPrefix = configuration.CommandPrefix;
			this.commands = commands;
			this.log = log;
		}

		public Task ProcessCommand (SocketMessage message)
		{
			string commandWord = GetCommandWord(message);

			if (!string.IsNullOrWhiteSpace(commandWord)
				&& commands.TryGetValue(commandWord, out IChatCommand command))
			{
				log.LogInformation($"Executing command {command.CommandTrigger}, message: {message}");

				return command.InvokeAsync(message);
			}

			return Task.CompletedTask;
		}

		private string GetCommandWord (SocketMessage message)
		{
			if (message?.Content?.StartsWith(commandPrefix) == false)
			{
				return null;
			}

			string firstWord = message.Content.IndexOf(" ", StringComparison.OrdinalIgnoreCase) > -1
				? message.Content.Substring(0, message.Content.IndexOf(" ", StringComparison.OrdinalIgnoreCase))
				: message.Content;

			return firstWord.Replace(commandPrefix, string.Empty);
		}
	}
}
