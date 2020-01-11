using System;
using System.Linq;
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
		private static readonly string[] CommandTriggers = { "cancelbids", "cancel", "cancelbid" };
		private readonly IAuctionProcessor auctionProcessor;
		private readonly DiscordConfiguration configuration;
		private readonly ILogger<CancelAuctionCommand> log;
		private readonly Regex pattern;
		public string ChannelSyntax => $"{configuration.CommandPrefix} {CommandTriggers.First()} \"ItemName\"";

		public CancelAuctionCommand(DiscordConfiguration configuration, IAuctionProcessor auctionProcessor, ILogger<CancelAuctionCommand> log)
		{
			string regex = "^" + Regex.Escape(configuration?.CommandPrefix) + "\\s*(?<trigger>" + string.Join('|', CommandTriggers) + @")?\s*""(?<name>.+)""\s*$";
			pattern = new Regex(regex, RegexOptions.IgnoreCase);
			this.configuration = configuration;
			this.auctionProcessor = auctionProcessor;
			this.log = log;
		}

		public (bool success, string name) ParseArgs(string messageContent)
		{
			Match match = pattern.Match(messageContent);

			if (!match.Success)
			{
				log.LogTrace("Did not match pattern.");
				return (false, null);
			}

			string name = match.Groups["name"].Value;
			log.LogDebug("Parsed cancel arguments: {0}", name);
			return (true,name);
		}

		public async Task<bool> TryInvokeAsync(IMessage message)
		{
			if (message == null)
			{
				return false;
			}
			(bool success, string name) = ParseArgs(message.Content);

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
				await auctionProcessor.CancelAuction(name, message);
				return true;
			}
		}
	}
}
