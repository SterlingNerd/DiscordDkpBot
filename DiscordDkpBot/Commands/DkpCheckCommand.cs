using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Discord;

using DiscordDkpBot.Configuration;
using DiscordDkpBot.Dkp;
using DiscordDkpBot.Dkp.EqDkpPlus.Xml;
using DiscordDkpBot.Extensions;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Commands
{
	public class DkpCheckCommand : IChannelCommand, IDmCommand
	{
		private readonly Regex channelPattern;
		private readonly DiscordConfiguration config;
		private readonly IDkpProcessor dkpProcessor;
		private readonly Regex dmPattern;
		private readonly ILogger<DkpCheckCommand> log;
		public string ChannelSyntax => $"{config.CommandPrefix} {{character-name}}";
		public string CommandDescription => "Check Dkp";
		public string DmSyntax => "dkp {character-name}";

		public DkpCheckCommand(DiscordConfiguration config, IDkpProcessor dkpProcessor, ILogger<DkpCheckCommand> log)
		{
			channelPattern = new Regex($@"^\s*((?<character>\w+) dkp|{Regex.Escape(config.CommandPrefix)} (?<character>\w+))\s*$", RegexOptions.IgnoreCase);
			dmPattern = new Regex(@"^\s*((?<character>\w+) dkp|dkp (?<character>\w+))\s*$", RegexOptions.IgnoreCase);
			this.config = config;
			this.dkpProcessor = dkpProcessor;
			this.log = log;
		}

		public (bool success, string characters) ParseArgs(string args)
		{
			Match match = channelPattern.Match(args);

			if (!match.Success)
			{
				match = dmPattern.Match(args);
			}

			if (!match.Success)
			{
				return (false, null);
			}

			string character = match.Groups["character"].Value;
			log.LogDebug($"Parsed character name '{character}'.");

			return (true, character);
		}

		public async Task<bool> TryInvokeAsync(IMessage message)
		{
			if (message == null)
			{
				return false;
			}
			(bool success, string character) = ParseArgs(message.Content);

			if (!success)
			{
				return false;
			}

			PlayerPoints dkp = await dkpProcessor.GetDkp(character);

			string dkpMessage = $"{character.UppercaseFirst()} has **{dkp.PointsCurrentWithTwink}** available to spend.\n```\nLifetime DKP for {character}: Earned {dkp.PointsEarnedWithTwink} - Spent {dkp.PointsSpentWithTwink} - Adjustments {dkp.PointsAdjustmentWithTwink}.```";

			string memePath = config.MemeDkpPath + character + ".jpg";
			;

			if (config.EnableMemeDkp && File.Exists(memePath))
			{
				log.LogInformation($"{character} dkp memes!");
				await message.Channel.SendFileAsync(memePath, dkpMessage);
			}
			else
			{
				await message.Channel.SendMessageAsync(dkpMessage);
			}
			return true;
		}
	}
}
