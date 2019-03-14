using System;
using System.Collections.Generic;
using System.Linq;
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
		private readonly List<IChatCommand> commands;
		private readonly ILogger<CommandProcessor> log;

		public CommandProcessor (DkpBotConfiguration configuration, IEnumerable<IChatCommand> commands, ILogger<CommandProcessor> log)
		{
			commandPrefix = configuration.CommandPrefix;
			this.commands = commands.ToList();
			this.log = log;
		}

		public Task ProcessCommand (SocketMessage message)
		{
			log.LogTrace($"{message.Author} ({message.Channel}): {message}");

			foreach (IChatCommand command in commands.Where(x => x.DoesCommandApply(message)))
			{
				log.LogInformation($"Executing command {command}, message: {message}");
				command.InvokeAsync(message);
			}

			return Task.CompletedTask;
		}
	}
}
