using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Discord;

using DiscordDkpBot.Configuration;
using DiscordDkpBot.Dkp;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Commands
{
	public class AddDkpCommand : IDmCommand
	{
		private const string commandName = "AddDkp";
		private readonly DkpBotConfiguration config;
		private readonly IDkpProcessor dkpProcessor;
		private readonly ILogger<UseRaidCommand> log;
		private readonly Regex pattern;

		public string DmSyntax => $"{commandName} {{raidId (optional, default = current \"active\" raid.)}} {{newLine(s)}} {{text to parse}}";

		public AddDkpCommand(DkpBotConfiguration config, IDkpProcessor dkpProcessor, ILogger<UseRaidCommand> log)
		{
			this.config = config;
			this.dkpProcessor = dkpProcessor;
			this.log = log;

			pattern = new Regex($@"^{Regex.Escape(commandName)}\s+(?<raidId>\d+)?(\r?\n)+(?<lines>.*\r?\n?)*", RegexOptions.IgnoreCase);
		}

		public (bool success, int? raidId, string lines) ParseArgs(string messageContent)
		{
			Match match = pattern.Match(messageContent);
			if (match.Success)
			{
				int? id = null;
				if (int.TryParse(match.Groups["raidId"].Value, out int parsed))
				{
					id = parsed;
				}
				string lines = match.Groups["lines"].Value;
				return (true, id, lines);
			}
			else
			{
				return (false, 0, null);
			}
		}

		public async Task<bool> TryInvokeAsync(IMessage message)
		{
			if (!(message.Channel is IDMChannel))
			{
				return false;
			}

			(bool success, int? raidId, string lines) = ParseArgs(message.Content);

			if (!success)
			{
				return false;
			}

			await dkpProcessor.AddDkp(lines, raidId);

			return true;
		}
	}
}
