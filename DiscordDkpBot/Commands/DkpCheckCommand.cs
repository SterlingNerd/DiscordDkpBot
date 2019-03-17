using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Discord;

using DiscordDkpBot.Dkp;
using DiscordDkpBot.Dkp.EqDkp;
using DiscordDkpBot.Dkp.EqDkp.Xml;
using DiscordDkpBot.Extensions;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Commands
{
	public class DkpCheckCommand : ICommand
	{
		private static readonly Regex pattern = new Regex(@"^\s*((?<character>\w+) dkp|.dkp (?<character>\w+))\s*$");
		private readonly IDkpProcessor dkpProcessor;
		private readonly ILogger<DkpCheckCommand> log;

		public DkpCheckCommand (IDkpProcessor dkpProcessor, ILogger<DkpCheckCommand> log)
		{
			this.dkpProcessor = dkpProcessor;
			this.log = log;
		}

		public bool DoesCommandApply (IMessage message)
		{
			return pattern.Match(message.Content).Success;
		}

		public async Task<bool> InvokeAsync (IMessage message)
		{
			try
			{
				Match match = pattern.Match(message.Content);
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
