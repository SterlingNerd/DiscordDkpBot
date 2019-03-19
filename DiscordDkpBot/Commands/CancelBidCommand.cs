using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Extensions;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Commands
{
	public class CancelBidCommand : ICommand
	{
		public const string Syntax = "\"{Item_Name_With_Typos}\" {Character} {Amount} {Rank}";

		private static readonly Regex pattern = new Regex(@"^""(?<Item>.+)""\s+cancel\s*$", RegexOptions.IgnoreCase);

		private readonly IAuctionProcessor auctionProcessor;
		private readonly ILogger<CancelAuctionCommand> log;

		public CancelBidCommand (IAuctionProcessor auctionProcessor, ILogger<CancelAuctionCommand> log)
		{
			this.auctionProcessor = auctionProcessor;
			this.log = log;
		}

		public bool DoesCommandApply (IMessage message)
		{
			return message.Channel is SocketDMChannel
				&& message.Content.Trim().StartsWith('"')
				&& message.Content.Contains("cancel", StringComparison.OrdinalIgnoreCase);
		}

		public async Task<bool> TryInvokeAsync (IMessage message)
		{
			try
			{
				if (!(message?.Channel is SocketDMChannel))
				{
					return false;
				}

				Match match = pattern.Match(message.Content);

				if (!match.Success)
				{
					return false;
				}

				string item = match.Groups["Item"].Value;

				log.LogTrace("Parsed bid arguments: \"{0}\" cancel.", item);

				await auctionProcessor.CancelBid(item, message);

				return true;
			}
			catch (AuctionNotFoundException ex)
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
