using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Discord;

using DiscordDkpBot.Auctions;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Commands
{
	public class PlaceBidCommand : IDmCommand
	{
		private static readonly Regex pattern = new Regex(@"^\s*\S(?<Item>.+?)\S\s+(?<Character>\w+)\s+(?<Bid>\d+)\s*$", RegexOptions.IgnoreCase);

		private readonly IAuctionProcessor auctionProcessor;
		private readonly ILogger<PlaceBidCommand> log;
		public string CommandDescription => "Place a Bid";
		public string DmSyntax => "\"{Item_Name_With_Typos}\" {Character} {Amount}";

		public PlaceBidCommand(IAuctionProcessor auctionProcessor, ILogger<PlaceBidCommand> log)
		{
			this.auctionProcessor = auctionProcessor;
			this.log = log;
		}

		public (bool success, string item, string character, int bid) ParseArgs(string messageContent)
		{
			Match match = pattern.Match(messageContent);
			if (!match.Success)
			{
				return (false, null, null, 0);
			}

			string item = match.Groups["Item"].Value;
			string character = match.Groups["Character"].Value;
			int bid = int.Parse(match.Groups["Bid"].Value);

			log.LogTrace("Parsed bid arguments: \"{0}\" {1} ({2}) for {3} dkp.", item, character, bid);

			return (true, item, character,  bid);
		}

		public async Task<bool> TryInvokeAsync(IMessage message)
		{
			if (!(message.Channel is IDMChannel))
			{
				return false;
			}

			(bool success, string item, string character, int bid) = ParseArgs(message.Content);

			if (!success)
			{
				return false;
			}

			await auctionProcessor.AddOrUpdateBid(item, character, "main", bid, message);

			return true;
		}
	}
}
