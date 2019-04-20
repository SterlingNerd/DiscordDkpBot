﻿using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Discord;

using DiscordDkpBot.Configuration;
using DiscordDkpBot.Dkp;
using DiscordDkpBot.Dkp.EqDkpPlus.Xml;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Commands
{
	public class StartRaidCommand //: IChannelCommand
	{
		private static readonly string[] CommandTriggers = { "startraid" };
		private readonly DkpBotConfiguration configuration;
		private readonly IDkpProcessor dkpProcessor;
		private readonly ILogger<StartRaidCommand> log;
		private readonly Regex pattern;

		public string ChannelSyntax => $"{configuration.CommandPrefix} {CommandTriggers.FirstOrDefault()} {{Raid Event Id}}";

		public StartRaidCommand(IDkpProcessor dkpProcessor, DkpBotConfiguration configuration, ILogger<StartRaidCommand> log)
		{
			string regex = "^" + Regex.Escape(configuration.CommandPrefix) + $@"\s+(?<trigger>{string.Join('|', CommandTriggers)})\s+(?<eventId>\d+)\s*$";
			pattern = new Regex(regex, RegexOptions.IgnoreCase);

			this.dkpProcessor = dkpProcessor;
			this.configuration = configuration;
			this.log = log;
		}

		public (bool success, int eventId) ParseArgs(string args)
		{
			Match match = pattern.Match(args);

			if (!match.Success)
			{
				return (false, 0);
			}

			int eventId = int.Parse(match.Groups["eventId"].Value);

			log.LogTrace("Parsed event arguments: {0}", eventId);

			return (true, eventId);
		}

		public async Task<bool> TryInvokeAsync(IMessage message)
		{
			if (message == null)
			{
				return false;
			}

			(bool success, int eventId) = ParseArgs(message.Content);

			if (!success)
			{
				log.LogTrace("Did not match.");
				return false;
			}
			else if (message.Channel is IPrivateChannel)
			{
				log.LogTrace("Must be public channel");
				return false;
			}
			else
			{
				RaidInfo raid = await dkpProcessor.StartRaid(eventId, message.Author.Username);
				log.LogDebug($"Created Raid: {raid}.");

				StringBuilder builder = new StringBuilder();

				if (configuration.EqDkpPlus.AddRaidUri.Contains("test=true"))
				{
					builder.AppendLine("**[Test Mode]** (didn't actually add a raid. Here's the most recent one.)");
				}
				builder.AppendLine("Started Raid:");

				builder.AppendLine("```json");
				builder.AppendLine($"{{{raid.Id} : \"{raid.EventName}\"}}");
				builder.AppendLine("```");

				await message.Channel.SendMessageAsync(builder.ToString());
				return true;
			}
		}
	}
}
