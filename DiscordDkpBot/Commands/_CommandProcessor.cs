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
		private readonly List<ICommand> commands;
		private readonly ILogger<CommandProcessor> log;

		public CommandProcessor (IEnumerable<ICommand> commands, ILogger<CommandProcessor> log)
		{
			this.commands = commands.ToList();
			this.log = log;
		}

		public async Task ProcessCommand (SocketMessage message)
		{
			log.LogInformation($"{message.Author} ({message.Channel}): {message}");
			bool success = false;
			List<Task<bool>> commandTasks = new List<Task<bool>>(commands.Count);

			foreach (ICommand command in commands)
			{
				log.LogTrace($"Executing command {command}, message: {message}");
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
