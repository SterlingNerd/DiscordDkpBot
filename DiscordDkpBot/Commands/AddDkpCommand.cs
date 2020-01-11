using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Discord;

using DiscordDkpBot.Configuration;
using DiscordDkpBot.Dkp;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Commands
{
	public class AddDkpCommand : IChannelCommand
	{
		private const string commandName = "AddDkp";
		private readonly DiscordConfiguration config;
		private readonly IDkpProcessor dkpProcessor;
		private readonly ILogger<AddDkpCommand> log;
		private readonly Regex pattern;

		public string ChannelSyntax => $"{config.CommandPrefix} {commandName} {{eventId}} {{value}} {{comment (optional)}}{{newLine(s)}}\n{{text to parse}}";

		public AddDkpCommand (DiscordConfiguration config, IDkpProcessor dkpProcessor, ILogger<AddDkpCommand> log)
		{
			this.config = config;
			this.dkpProcessor = dkpProcessor;
			this.log = log;

			pattern = new Regex(
				$@"^{Regex.Escape(config.CommandPrefix)}\s+{Regex.Escape(commandName)}\s+"
				+ @"(?<eventId>\d+)\s+"
				+ @"(?<value>\d+)[ \t]*"
				+"(?<comment>.*)?(?:\r\n|\n|$)"
				+"(?<lines>(?:.*(?:\r\n|\n|$))*)",
				RegexOptions.IgnoreCase);
		}

		public (bool success, int eventId, int value, string lines, string comment) ParseArgs (string messageContent)
		{
			Match match = pattern.Match(messageContent);
			if (match.Success)
			{
				string eventString = match.Groups["eventId"].Value;
				int eventId = int.Parse(eventString);
				string valueString = match.Groups["value"].Value;
				int value = int.Parse(valueString);
				string comment = match.Groups["comment"].Value;
				string lines = match.Groups["lines"].Value;
				return (true, eventId, value, comment, lines);
			}
			else
			{
				return (false, 0, 0, null, null);
			}
		}

		public async Task<bool> TryInvokeAsync (IMessage message)
		{
			if (!(message.Channel is IDMChannel))
			{
				return false;
			}

			(bool success, int eventId, int value, string comment, string lines) = ParseArgs(message.Content);

			if (!success)
			{
				return false;
			}

			await dkpProcessor.AddDkp(eventId, value, comment, lines);

			return true;
		}
	}
}
