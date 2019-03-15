using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Discord;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Configuration;
using DiscordDkpBot.Extensions;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Commands
{
	public class CancelAuctionCommand : BasicChatCommand
	{
		public const string Syntax = "One item:\t\t\t`\"Item_Name\"`\nTwo of an item:\t\t`2x\"Item_Name\"`\nCustom duration:\t\t`\"Item_Name\" 4`";
		private readonly Regex pattern;
		private readonly IAuctionProcessor auctionProcessor;
		private readonly ILogger<CancelAuctionCommand> log;

		public CancelAuctionCommand (DkpBotConfiguration configuration, IAuctionProcessor auctionProcessor, ILogger<CancelAuctionCommand> log)
			: base(configuration.CommandPrefix, new[] { "cancel", "cancelbid", "cancelbids", "dkp cancel", "dkp cancelbid", "dkp cancelbids" })
		{
			string regex = configuration.CommandPrefix + "?(?<trigger>" + string.Join('|', CommandTriggers) + @")?\s*(?<number>\d+)?x?\s*""(?<name>.+)""\s*(?<time>\d+)?";
			pattern = new Regex(regex, RegexOptions.IgnoreCase);
			this.auctionProcessor = auctionProcessor;
			this.log = log;
		}

		public override Task<bool> InvokeAsync (IMessage message)
		{
			try
			{
				string name = ParseArgs(message.Content);
				auctionProcessor.CancelAuction(name, message);
				return Task.FromResult(true);
			}
			catch (AuctionAlreadyExistsException ex)
			{
				log.LogWarning(ex);
				message.Channel.SendMessageAsync(ex.Message);
				return Task.FromResult(false);
			}
			catch (Exception ex)
			{
				log.LogWarning(ex);
				message.Channel.SendMessageAsync($"Yer doin it wrong!\n\nSyntax:\n{Syntax}");
				return Task.FromResult(false);
			}
		}

		public string ParseArgs (string args)
		{
			Match match = pattern.Match(args);

			if (!match.Success)
			{
				throw new ArgumentException($"Could not parse auction from: '{args}'");
			}

			string name = match.Groups["name"].Value;

			log.LogTrace("Parsed cancel arguments: {0}", name);

			return name;
		}
	}
}
