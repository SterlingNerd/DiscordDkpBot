using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Discord;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Configuration;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Commands
{
	public class CancelAuctionCommand : IChannelCommand
	{
		private static readonly string[] CommandTriggers = { "cancel", "cancelbid", "cancelbids" };
		private readonly IAuctionProcessor auctionProcessor;
		private readonly ILogger<CancelAuctionCommand> log;
		private readonly Regex pattern;
		public string ChannelSyntax => "`\"ItemName\" cancel`";

		public CancelAuctionCommand(DkpBotConfiguration configuration, IAuctionProcessor auctionProcessor, ILogger<CancelAuctionCommand> log)
		{
			string regex = "^" + Regex.Escape(configuration?.CommandPrefix) + "?(?<trigger>" + string.Join('|', CommandTriggers) + @")?\s*(?<number>\d+)?x?\s*""(?<name>.+)""\s*(?<time>\d+)?\s*$";
			pattern = new Regex(regex, RegexOptions.IgnoreCase);
			this.auctionProcessor = auctionProcessor;
			this.log = log;
		}

		public async Task<bool> TryInvokeAsync(IMessage message)
		{
			Match match = pattern.Match(message.Content);

			if (!match.Success)
			{
				log.LogTrace("Did not match pattern.");
				return false;
			}

			string name = match.Groups["name"].Value;
			log.LogDebug("Parsed cancel arguments: {0}", name);

			await auctionProcessor.CancelAuction(name, message);
			return true;
		}
	}
}
