using System;
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
	public class UseRaidCommand : IChannelCommand
	{
		private const string commandName = "UseRaid";
		private readonly DkpBotConfiguration config;
		private readonly IDkpProcessor dkpProcessor;
		private readonly ILogger<UseRaidCommand> log;
		private readonly Regex pattern;

		public string ChannelSyntax => $"{config.CommandPrefix} {commandName} {{raid-id}}";

		public UseRaidCommand(DkpBotConfiguration config, IDkpProcessor dkpProcessor, ILogger<UseRaidCommand> log)
		{
			this.config = config;
			this.dkpProcessor = dkpProcessor;
			this.log = log;

			pattern = new Regex($@"^{Regex.Escape(config.CommandPrefix)}\s+{Regex.Escape(commandName)}\s+(?<raidId>\d+)\s*$", RegexOptions.IgnoreCase);
		}

		public (bool success, int raidId) ParseArgs(string messageContent)
		{
			Match match = pattern.Match(messageContent);
			if (match.Success)
			{
				int id = int.Parse(match.Groups["raidId"].Value);
				log.LogDebug("Parsed Id {0}", id);
				return (true, id);
			}
			else
			{
				return (false, 0);
			}
		}

		public async Task<bool> TryInvokeAsync(IMessage message)
		{
			if (!(message.Channel is IDMChannel))
			{
				return false;
			}

			(bool success, int raidId) = ParseArgs(message.Content);

			if (!success)
			{
				return false;
			}

			RaidInfo raid = await dkpProcessor.UseRaid(raidId);

			StringBuilder builder = new StringBuilder();
			builder.AppendLine("Using Raid:");
			builder.AppendLine("```json");
			builder.AppendLine($"{{{raid.Id} : \"{raid.EventName}\"}}");
			builder.AppendLine("```");

			await message.Channel.SendMessageAsync(builder.ToString());

			return true;
		}
	}
}
