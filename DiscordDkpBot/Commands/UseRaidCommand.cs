using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Discord;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Configuration;
using DiscordDkpBot.Dkp;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Commands
{
	public class UseRaidCommand : ICommand
	{
		private readonly Regex pattern;
		private const string commandName = "UseRaid";
		private readonly ILogger<UseRaidCommand> log;
		private readonly DkpBotConfiguration config;
		private readonly IDkpProcessor dkpProcessor;

		public UseRaidCommand (DkpBotConfiguration config, IDkpProcessor dkpProcessor, ILogger<UseRaidCommand> log)
		{
			this.config = config;
			this.dkpProcessor = dkpProcessor;
			this.log = log;

			pattern = new Regex($@"^{Regex.Escape(config.CommandPrefix)}\s+{Regex.Escape(commandName)}\s+(?<raidId>\d+)\s*$", RegexOptions.IgnoreCase);
		}

		public (bool success, int raidId) ParseArgs (string messageContent)
		{
			Match match = pattern.Match(messageContent);
			if (match.Success)
			{
				int id = int.Parse(match.Groups["raidId"].Value);
				return (true, id);
			}
			else
			{
				return (false, 0);
			}
		}

		public async Task<bool> TryInvokeAsync (IMessage message)
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

			await dkpProcessor.UseRaid(raidId, message);

			return true;
		}
	}
}
