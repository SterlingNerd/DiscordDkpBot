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
	public class CancelAuctionCommand : ICommand
	{
		public const string Syntax = "`\"ItemName\" cancel`";
		private static readonly string[] CommandTriggers = { "cancel", "cancelbid", "cancelbids", "dkp cancel", "dkp cancelbid", "dkp cancelbids" };
		private readonly IAuctionProcessor auctionProcessor;
		private readonly ILogger<ICommand> log;
		private readonly Regex pattern;

		public CancelAuctionCommand (DkpBotConfiguration configuration, IAuctionProcessor auctionProcessor, ILogger<ICommand> log)
		{
			string regex = "^" + configuration?.CommandPrefix + "?(?<trigger>" + string.Join('|', CommandTriggers) + @")?\s*(?<number>\d+)?x?\s*""(?<name>.+)""\s*(?<time>\d+)?\s*$";
			pattern = new Regex(regex, RegexOptions.IgnoreCase);
			this.auctionProcessor = auctionProcessor;
			this.log = log;
		}

		public Task<bool> TryInvokeAsync (IMessage message)
		{
			try
			{
				Match match = pattern.Match(message.Content);

				if (!match.Success)
				{
					log.LogTrace("Did not apply.");
					return Task.FromResult(false);
				}

				string name = match.Groups["name"].Value;
				log.LogTrace("Parsed cancel arguments: {0}", name);
				auctionProcessor.CancelAuction(name, message);

				return Task.FromResult(true);
			}
			catch (AuctionNotFoundException ex)
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
	}
}
