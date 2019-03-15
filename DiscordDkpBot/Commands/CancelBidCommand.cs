using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Configuration;
using DiscordDkpBot.Extensions;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Commands
{
	public class CancelBidCommand : ICommand
	{
		public const string Syntax = "\"{Item_Name_With_Typos}\" {Character} {Amount} {Rank}";

		private static readonly Regex pattern = new Regex(@"""(?<Item>.+)""\s+cancel", RegexOptions.IgnoreCase);

		private readonly IAuctionProcessor auctionProcessor;
		private readonly ILogger<CancelBidCommand> log;

		public CancelBidCommand (IAuctionProcessor auctionProcessor, ILogger<CancelBidCommand> log)
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

		public async Task<bool> InvokeAsync (IMessage message)
		{
			try
			{
				string item = ParseArgs(message.Content);

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

		public string ParseArgs (string messageContent)
		{
			Match match = pattern.Match(messageContent);

			if (!match.Success)
			{
				throw new ArgumentException($"Could not parse bid from: '{messageContent}'");
			}

			string item = match.Groups["Item"].Value;

			log.LogTrace("Parsed bid arguments: \"{0}\" cancel.", item);

			return item;
		}
	}
}
