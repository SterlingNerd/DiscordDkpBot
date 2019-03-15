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
	public class StartBidsCommand : BasicChatCommand
	{
		public const string Syntax = "One item:\t\t\t`\"Item_Name\"`\nTwo of an item:\t\t`2x\"Item_Name\"`\nCustom duration:\t\t`\"Item_Name\" 4`";
		private readonly Regex pattern;
		private readonly IAuctionProcessor auctionProcessor;
		private readonly ILogger<StartBidsCommand> log;

		public StartBidsCommand (DkpBotConfiguration configuration, IAuctionProcessor auctionProcessor, ILogger<StartBidsCommand> log)
			: base(configuration.CommandPrefix, new[] { "startbid", "startbids", "dkp startbids", "dkp startbid" })
		{
			string regex = configuration.CommandPrefix + "?(?<trigger>" + string.Join('|', CommandTriggers) + @")?\s*(?<number>\d+)?x?\s*""(?<name>.+)""\s*(?<time>\d+)?";
			pattern = new Regex(regex);
			this.auctionProcessor = auctionProcessor;
			this.log = log;
		}

		public override Task<bool> InvokeAsync (IMessage message)
		{
			try
			{
				(int? quantity, string name, int? minutes) = ParseArgs(message.Content);
				auctionProcessor.StartAuction(quantity, name, minutes, message.Channel, message.Author);
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

		public (int? number, string name, int? minutes) ParseArgs (string args)
		{
			Match match = pattern.Match(args);

			if (!match.Success)
			{
				throw new ArgumentException($"Could not parse auction from: '{args}'");
			}

			int? number = match.Groups["number"].Success ? int.Parse(match.Groups["number"].Value) : (int?)null;
			string name = match.Groups["name"].Value;
			int? minutes = match.Groups["time"].Success ? int.Parse(match.Groups["time"].Value) : (int?)null;

			log.LogTrace("Parsed auction arguments: {0}x {1} for {2} mins.", number, name, minutes);

			return (number, name, minutes);
		}
	}
}
