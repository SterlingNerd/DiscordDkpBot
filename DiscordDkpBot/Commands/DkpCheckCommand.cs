using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Discord;

using DiscordDkpBot.Configuration;
using DiscordDkpBot.Dkp;
using DiscordDkpBot.Dkp.EqDkpPlus.Xml;
using DiscordDkpBot.Extensions;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Commands
{
	[UsedImplicitly]
	public class DkpCheckCommand : IChannelCommand, IDmCommand
	{
		private readonly DkpBotConfiguration config;
		private readonly IDkpProcessor dkpProcessor;
		private readonly ILogger<DkpCheckCommand> log;
		private readonly Regex pattern;

		public string ChannelSyntax => $"{config.CommandPrefix} dkp {{character-name}}";
		public string DmSyntax => "dkp {character-name}";

		public DkpCheckCommand(DkpBotConfiguration config, IDkpProcessor dkpProcessor, ILogger<DkpCheckCommand> log)
		{
			pattern = new Regex($@"^\s*((?<character>\w+) dkp|{Regex.Escape(config.CommandPrefix)} (?<character>\w+))\s*$", RegexOptions.IgnoreCase);
			this.config = config;
			this.dkpProcessor = dkpProcessor;
			this.log = log;
		}

		public async Task<bool> TryInvokeAsync(IMessage message)
		{
			Match match = pattern.Match(message.Content);
			if (!match.Success)
			{
				return false;
			}

			string character = match.Groups["character"].Value;
			log.LogDebug($"Parsed character name '{character}'.");
			PlayerPoints dkp = await dkpProcessor.GetDkp(character);

			string dkpMessage = $"{character.UppercaseFirst()} has **{dkp.PointsCurrentWithTwink}** available to spend.\n```brainfuck\nLifetime DKP for {character}: Earned {dkp.PointsEarnedWithTwink} - Spent {dkp.PointsSpentWithTwink} - Adjustments {dkp.PointsAdjustmentWithTwink}.```";
			if (character.Equals("magg", StringComparison.OrdinalIgnoreCase))
			{
				log.LogInformation("magg dkp memes!");
				await message.Channel.SendFileAsync("./resources/dkpmagg.jpg", dkpMessage);
			}
			else
			{
				await message.Channel.SendMessageAsync(dkpMessage);
			}
			return true;
		}
	}
}
