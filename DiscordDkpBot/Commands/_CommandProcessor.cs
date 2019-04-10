using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using DiscordDkpBot.Configuration;
using DiscordDkpBot.Extensions;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Commands
{
	[UsedImplicitly]
	public class CommandProcessor : ICommandProcessor
	{
		private readonly ICollection<IChannelCommand> channelCommands;
		private readonly string ChannelHelpMessage;
		private readonly DkpBotConfiguration config;
		private readonly ICollection<IDmCommand> dmCommands;

		private readonly string DmHelpMessage;
		private readonly ILogger<CommandProcessor> log;

		public CommandProcessor(DkpBotConfiguration config, IEnumerable<IChannelCommand> channelCommands, IEnumerable<IDmCommand> dmCommands, ILogger<CommandProcessor> log)
		{
			this.channelCommands = channelCommands?.ToList() ?? new List<IChannelCommand>();
			this.dmCommands = dmCommands?.ToList() ?? new List<IDmCommand>();

			DmHelpMessage = GetDmHelpMessage(dmCommands);
			ChannelHelpMessage = GetChannelHelpMessage(channelCommands);

			this.config = config;
			this.log = log;
		}

		public async Task ProcessCommand(SocketMessage message)
		{
			log.LogInformation($"{message.Author} ({message.Channel}): {message}");
			List<Task<bool>> commandTasks = new List<Task<bool>>();
			try
			{
				//Are they asking for help?
				if (message.Channel is IPrivateChannel)
				{
					commandTasks.AddRange(ProcessDmCommand(message));
				}
				else if (!(message.Channel is IPrivateChannel))
				{
					commandTasks.AddRange(ProcessChannelCommand(message));
				}

				await Task.WhenAll(commandTasks);

				//As an extra help, if we're dealing with PMs and nothing matched, we can give them the help message.
				if (message.Channel is IPrivateChannel && commandTasks.All(x => x.Result == false))
				{
					commandTasks.Add(SendHelp(DmHelpMessage, message.Channel));
				}
			}
			catch (Exception ex)
			{
				log.LogError(ex);
				await message.Channel.SendMessageAsync("**Look ma, I'm roadkill!** (something went wrong, check the logs.)");
			}
		}

		private IEnumerable<Task<bool>> ProcessChannelCommand(SocketMessage message)
		{
			List<Task<bool>> commandTasks = new List<Task<bool>>();
			if (Regex.Match(message.Content, $@"{Regex.Escape(config.CommandPrefix)}\s+help",  RegexOptions.IgnoreCase).Success)
			{
				commandTasks.Add(SendHelp(ChannelHelpMessage, message.Channel));
			}
			else
			{
				commandTasks.AddRange(channelCommands.Select(command => RunCommand(command, message)));
			}
			return commandTasks;
		}

		private IEnumerable<Task<bool>> ProcessDmCommand(SocketMessage message)
		{
			List<Task<bool>> commandTasks = new List<Task<bool>>();
			if (message.Content.StartsWith("help", StringComparison.OrdinalIgnoreCase))
			{
				commandTasks.Add(SendHelp(DmHelpMessage, message.Channel));
			}

			else
			{
				foreach (IDmCommand command in dmCommands)
				{
					commandTasks.Add(RunCommand(command, message));
				}
			}

			return commandTasks;
		}

		private async Task<bool> RunCommand(ICommand command, IMessage message)
		{
			try
			{
				log.LogTrace($"Executing command {command}, message: {message}");
				return await command.TryInvokeAsync(message);
			}
			catch (DkpBotException ex)
			{
				log.LogWarning(ex, $"{command.GetType()} threw an exception:");
				await message.Channel.SendMessageAsync(ex.Message);
				return false;
			}
		}


		private static string GetChannelHelpMessage (IEnumerable<IChannelCommand> channelCommands)
		{
			return string.Join(Environment.NewLine, channelCommands.Select(x => $"```\n{x.ChannelSyntax}\n```"));
		}
		private static string GetDmHelpMessage (IEnumerable<IDmCommand> channelCommands)
		{
			return string.Join(Environment.NewLine, channelCommands.Select(x => $"```\n{x.DmSyntax}\n```"));
		}

		private static async Task<bool> SendHelp(string message, IMessageChannel channel)
		{
			await channel.SendMessageAsync(message);
			return true;
		}
	}
}
