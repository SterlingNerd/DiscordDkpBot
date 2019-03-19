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
		private readonly List<ICommand> commands;
		private readonly ILogger<CommandProcessor> log;

		public CommandProcessor (DkpBotConfiguration configuration, IEnumerable<ICommand> commands, ILogger<CommandProcessor> log)
		{
			commandPrefix = configuration.CommandPrefix;
			this.commands = commands.ToList();
			this.log = log;
		}

		public async Task ProcessCommand (SocketMessage message)
		{
			log.LogTrace($"{message.Author} ({message.Channel}): {message}");
			bool success = false;
			List<Task<bool>> commandTasks = new List<Task<bool>>(commands.Count);
			foreach (ICommand command in commands)
			{
				log.LogInformation($"Executing command {command}, message: {message}");
				commandTasks.Add(command.TryInvokeAsync(message));
			}

			await Task.WhenAll(commandTasks);

			if (message.Channel is SocketDMChannel && commandTasks.All(x => x.Result == false))
			{
				//TODO: give syntax help to the poor soul that's DM'ing us.
			}

		}
	}
}
