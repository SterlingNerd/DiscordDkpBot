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
		private readonly DkpBotConfiguration config;
		private readonly ICollection<IDmCommand> dmCommands;

		private readonly string HelpMessage;
		private readonly ILogger<CommandProcessor> log;

		public CommandProcessor (DkpBotConfiguration config, IEnumerable<IChannelCommand> channelCommands, IEnumerable<IDmCommand> dmCommands, ILogger<CommandProcessor> log)
		{
			this.channelCommands = channelCommands?.ToList() ?? new List<IChannelCommand>();
			this.dmCommands = dmCommands?.ToList() ?? new List<IDmCommand>();

			HelpMessage = GetHelpMessage(channelCommands, dmCommands);

			this.config = config;
			this.log = log;
		}

		public async Task ProcessCommand (SocketMessage message)
		{
			if (!message.Content.StartsWith(config.CommandPrefix, StringComparison.CurrentCultureIgnoreCase) && !(message.Channel is IDMChannel))
			{
				// Only trigger on commands or DMs.
				return;
			}

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
					commandTasks.Add(SendHelp(message.Author));
				}
			}
			catch (Exception ex)
			{
				log.LogError(ex);
				await message.Channel.SendMessageAsync("**Look ma, I'm roadkill!** (something went wrong, check the logs.)");
			}
		}

		private IEnumerable<Task<bool>> ProcessChannelCommand (SocketMessage message)
		{
			List<Task<bool>> commandTasks = new List<Task<bool>>();
			if (Regex.Match(message.Content, $@"{Regex.Escape(config.CommandPrefix)}\s+help", RegexOptions.IgnoreCase).Success)
			{
				commandTasks.Add(SendHelp(message.Author));
			}
			else
			{
				commandTasks.AddRange(channelCommands.Select(command => RunCommand(command, message)));
			}
			return commandTasks;
		}

		private IEnumerable<Task<bool>> ProcessDmCommand (SocketMessage message)
		{
			List<Task<bool>> commandTasks = new List<Task<bool>>();
			if (message.Content.StartsWith("help", StringComparison.OrdinalIgnoreCase))
			{
				commandTasks.Add(SendHelp(message.Author));
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

		private async Task<bool> RunCommand (ICommand command, IMessage message)
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
				// True here, because it matched.
				return true;
			}
		}


		private static string GetHelpMessage (IEnumerable<IChannelCommand> channelCommands, IEnumerable<IDmCommand> dmCommands)
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendLine("**Valid Channel Commands**");
			builder.AppendLine(string.Join(Environment.NewLine, channelCommands.Select(x => $"```\n{x.ChannelSyntax}\n```")));
			builder.AppendLine("**Valid DM Commands**");
			builder.AppendLine(string.Join(Environment.NewLine, dmCommands.Select(x => $"```\n{x.DmSyntax}\n```")));
			return builder.ToString();
		}

		private async Task<bool> SendHelp (IUser author)
		{
			IDMChannel dm = await author.GetOrCreateDMChannelAsync();
			await dm.SendMessageAsync(HelpMessage);
			return true;
		}
	}
}
