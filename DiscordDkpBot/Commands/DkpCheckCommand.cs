using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Discord;

using DiscordDkpBot.Configuration;
using DiscordDkpBot.Dkp;
using DiscordDkpBot.Dkp.EqDkp;
using DiscordDkpBot.Dkp.EqDkp.Xml;
using DiscordDkpBot.Extensions;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Commands
{
	public class DkpCheckCommand : ICommand
	{
		private readonly Regex pattern;
		private readonly IDkpProcessor dkpProcessor;
		private readonly ILogger<DkpCheckCommand> log;

		public DkpCheckCommand (DkpBotConfiguration config, IDkpProcessor dkpProcessor, ILogger<DkpCheckCommand> log)
		{
			pattern = new Regex($@"^\s*((?<character>\w+) dkp|{Regex.Escape(config.CommandPrefix)} (?<character>\w+))\s*$", RegexOptions.IgnoreCase);
			this.dkpProcessor = dkpProcessor;
			this.log = log;
		}

		public async Task<bool> TryInvokeAsync (IMessage message)
		{
			try
			{
				Match match = pattern.Match(message.Content);
				if (!match.Success)
				{
					return false;
				}

				string character = match.Groups["character"].Value;
				PlayerPoints dkp = await dkpProcessor.GetDkp(character);

				await message.Channel.SendMessageAsync($"{character.UppercaseFirst()} has **{dkp.PointsCurrentWithTwink}** available to spend.\n```brainfuck\nLifetime DKP for {character}: Earned {dkp.PointsEarnedWithTwink} - Spent {dkp.PointsSpentWithTwink} - Adjustments {dkp.PointsAdjustmentWithTwink}.```");
				return true;
			}
			catch (PlayerNotFoundException ex)
			{
				log.LogWarning(ex);
				await message.Channel.SendMessageAsync(ex.Message);
				return false;
			}
			catch (Exception ex)
			{
				log.LogError(ex);
				return false;
			}
		}
	}
}
