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
		private readonly DiscordConfiguration config;
		private readonly ICollection<IDmCommand> dmCommands;

		private readonly string HelpMessage;
		private readonly string BiddingHelpMessage;
		private readonly ILogger<CommandProcessor> log;

		public CommandProcessor(DiscordConfiguration config, IEnumerable<IChannelCommand> channelCommands, IEnumerable<IDmCommand> dmCommands, ILogger<CommandProcessor> log)
		{
			this.channelCommands = channelCommands?.ToList() ?? new List<IChannelCommand>();
			this.dmCommands = dmCommands?.ToList() ?? new List<IDmCommand>();

			HelpMessage = GetHelpMessage(channelCommands, dmCommands);
			BiddingHelpMessage = GetBiddingHelpMessage();

			this.config = config;
			this.log = log;
		}

		public async Task ProcessCommand(SocketMessage message)
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
					// If they're actually asking for help give them everything.
					if (message.Content.Contains("help", StringComparison.CurrentCultureIgnoreCase))
					{
						commandTasks.Add(SendHelp(message.Author));
					}
					else
					{
						commandTasks.Add(SendBiddingHelp(message.Author));
					}
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

		private IEnumerable<Task<bool>> ProcessDmCommand(SocketMessage message)
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
				// True here, because it matched.
				return true;
			}
		}


		private static string GetHelpMessage(IEnumerable<IChannelCommand> channelCommands, IEnumerable<IDmCommand> dmCommands)
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendLine("**Valid Channel Commands**");

			foreach (var command in channelCommands)
			{
				builder.AppendLine($"{command.CommandDescription}: ```css\n{command.ChannelSyntax}```");
			}

			builder.AppendLine();

			builder.AppendLine("**Valid Direct Message Commands**");
			foreach (var command in dmCommands)
			{
				builder.AppendLine($"{command.CommandDescription}: ```css\n{command.DmSyntax}```");
			}

			return builder.ToString();
		}
		private static string GetBiddingHelpMessage()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendLine("**How to place a bid:**");
			builder.AppendLine();
			builder.AppendLine("> 1) Copy / paste the example from the auction:                     `\"Test\" character 69 Mainspec / MS / Offspec / OS`");
			builder.AppendLine("> 2) Replace character with your character's name:              `\"Test\" YoMomma 69 Mainspec/MS/Offspec/OS`");
			builder.AppendLine("> 3) Replace 69 with how much you want to bid:                   `\"Test\" YoMomma 420 Mainspec / MS / Offspec / OS`");
			builder.AppendLine("> 4) Replace the rank with the appropriate rank:                    `\"Test\" YoMomma 420 ms`");
			builder.AppendLine("> 5) Message your bid me directly.");
			builder.AppendLine("> 	(you can click on my name in chat and paste it into the chat bar at the bottom of the popup window)");
			builder.AppendLine("");
			builder.AppendLine("Here are some examples of properly formatted bids:");
			builder.AppendLine("```");
			builder.AppendLine("\"Caub's Bread\" Maldinaw 12 os");
			builder.AppendLine("``````");
			builder.AppendLine("\"Exercise Wheel\" Hamsterlunch 9000 mainspec");
			builder.AppendLine("```");
			builder.AppendLine("Notes:");
			builder.AppendLine("- The quotations around the item name are important and must be included.");
			builder.AppendLine("- If there are typos in the item's name, they must also be in your bid. Easiest way to ensure accuracy is to copy/paste the item name from the auction.");
			builder.AppendLine("- Choose only one role.");
			builder.AppendLine("- Nothing is case sensitive.");
			builder.AppendLine("- Use the character name recieving the item. Not an abbreviation etc.");
			builder.AppendLine("- Characters must exist in the dkp site.");
			builder.AppendLine("");
			builder.AppendLine("If you require help with other commands, say: `help`");

			return builder.ToString();
		}

		private async Task<bool> SendHelp(IUser author)
		{
			IDMChannel dm = await author.GetOrCreateDMChannelAsync();
			await dm.SendMessageAsync(HelpMessage);
			return true;
		}
		private async Task<bool> SendBiddingHelp(IUser author)
		{
			IDMChannel dm = await author.GetOrCreateDMChannelAsync();
			await dm.SendMessageAsync(BiddingHelpMessage);
			return true;
		}
	}
}
