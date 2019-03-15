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
	public class PlaceBidCommand : ICommand
	{
		public const string Syntax = "\"{Item_Name_With_Typos}\" {Character} {Amount} {Rank}";

		private static readonly Regex pattern = new Regex(@"""(?<Item>.+)""\s+(?<Character>\w+)(\s+(?<Bid>\d+)\s+(?<Rank>\w+)|\s+(?<Rank>\w+)\s+(?<Bid>\d+))");

		private readonly IAuctionProcessor auctionProcessor;
		private readonly ILogger<PlaceBidCommand> log;
		private readonly RankConfiguration[] ranks;

		public PlaceBidCommand (DkpBotConfiguration config, IAuctionProcessor auctionProcessor, ILogger<PlaceBidCommand> log)
		{
			this.auctionProcessor = auctionProcessor;
			this.log = log;
			ranks = config.Ranks;
		}

		public bool DoesCommandApply (IMessage message)
		{
			return message.Channel is SocketDMChannel && message.Content.Trim().StartsWith('"');
		}

		public async Task<bool> InvokeAsync (IMessage message)
		{
			try
			{
				(string item, string character, string rank, int bid) = ParseArgs(message.Content);

				await auctionProcessor.CreateBid(item, character, rank, bid, message);

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

		public (string item, string character, string rank, int bid) ParseArgs (string messageContent)
		{
			Match match = pattern.Match(messageContent);

			if (!match.Success)
			{
				throw new ArgumentException($"Could not parse bid from: '{messageContent}'");
			}

			string item = match.Groups["Item"].Value;
			string character = match.Groups["Character"].Value;
			string rank = match.Groups["Rank"].Value;
			int bid = int.Parse(match.Groups["Bid"].Value);

			log.LogTrace("Parsed bid arguments: \"{0}\" {1} ({2}) for {3} dkp.", item, character, rank, bid);

			return (item, character, rank, bid);
		}
	}
}
