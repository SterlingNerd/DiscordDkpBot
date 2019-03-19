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
	public class StartAuctionCommand : ICommand
	{
		public const string Syntax = "One item:\t\t\t`\"Item_Name\"`\nTwo of an item:\t\t`2x\"Item_Name\"`\nCustom duration:\t\t`\"Item_Name\" 4`";
		private static readonly string[] CommandTriggers = { "startbid", "startbids", "dkp startbids", "dkp startbid" };
		private readonly IAuctionProcessor auctionProcessor;
		private readonly ILogger<CancelAuctionCommand> log;
		private readonly Regex pattern;

		public StartAuctionCommand (DkpBotConfiguration configuration, IAuctionProcessor auctionProcessor, ILogger<CancelAuctionCommand> log)
		{
			string regex = "^" + configuration.CommandPrefix + "?(?<trigger>" + string.Join('|', CommandTriggers) + @")?\s*(?<number>\d+)?x?\s*""(?<name>.+)""\s*(?<time>\d+)?\s*$";
			pattern = new Regex(regex, RegexOptions.IgnoreCase);
			this.auctionProcessor = auctionProcessor;
			this.log = log;
		}

		public (bool success, int? number, string name, int? minutes) ParseArgs (string args)
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

		public async Task<bool> TryInvokeAsync (IMessage message)
		{
			try
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
					await message.Channel.SendMessageAsync("Yer doin it wrong!\n\nYou can only start auctions from a public channel, not DMs.");
					return false;
				}
				else
				{
					await auctionProcessor.StartAuction(quantity, name, minutes, message);
					return true;
				}
			}
			catch (AuctionAlreadyExistsException ex)
			{
				log.LogWarning(ex);
				await message.Channel.SendMessageAsync(ex.Message);
				return false;
			}
			catch (Exception ex)
			{
				log.LogWarning(ex);
				await message.Channel.SendMessageAsync($"Yer doin it wrong!\n\nSyntax:\n{Syntax}");
				return false;
			}
		}
	}
}
