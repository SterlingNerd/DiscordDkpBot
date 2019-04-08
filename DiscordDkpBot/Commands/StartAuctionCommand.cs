using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Discord;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Configuration;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Commands
{
	public class StartAuctionCommand : IChannelCommand
	{
		private static readonly string[] CommandTriggers = { "startbid", "startbids" };
		private readonly IAuctionProcessor auctionProcessor;
		private readonly ILogger<StartAuctionCommand> log;
		private readonly Regex pattern;

		public string ChannelSyntax => "One item:\t\t\t`\"Item_Name\"`\nTwo of an item:\t\t`2x\"Item_Name\"`\nCustom duration:\t\t`\"Item_Name\" 4`";

		public StartAuctionCommand(DkpBotConfiguration configuration, IAuctionProcessor auctionProcessor, ILogger<StartAuctionCommand> log)
		{
			string regex = "^" + Regex.Escape(configuration.CommandPrefix) + @"\s*(?<trigger>" + string.Join('|', CommandTriggers) + @")?\s+(?<number>\d+)?x?\s*""(?<name>.+)""\s*(?<time>\d+)?\s*$";
			pattern = new Regex(regex, RegexOptions.IgnoreCase);
			this.auctionProcessor = auctionProcessor;
			this.log = log;
		}

		public (bool success, int? number, string name, int? minutes) ParseArgs(string args)
		{
			Match match = pattern.Match(args);

			if (!match.Success)
			{
				return (false, null, null, null);
			}

			int? number = match.Groups["number"].Success ? int.Parse(match.Groups["number"].Value) : (int?)null;
			string name = match.Groups["name"].Value;
			int? minutes = match.Groups["time"].Success ? int.Parse(match.Groups["time"].Value) : (int?)null;

			log.LogTrace("Parsed auction arguments: {0}x {1} for {2} mins.", number, name, minutes);

			return (true, number, name, minutes);
		}

		public async Task<bool> TryInvokeAsync(IMessage message)
		{
			if (message == null)
			{
				return false;
			}
			(bool success, int? quantity, string name, int? minutes) = ParseArgs(message.Content);

			if (!success)
			{
				return false;
			}
			else if (message.Channel is IPrivateChannel)
			{
				return false;
			}
			else
			{
				await auctionProcessor.StartAuction(quantity, name, minutes, message);
				return true;
			}
		}
	}
}
