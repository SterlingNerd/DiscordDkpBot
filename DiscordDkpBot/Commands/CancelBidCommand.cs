using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Discord;

using DiscordDkpBot.Auctions;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Commands
{
	[UsedImplicitly]
	public class CancelBidCommand : IDmCommand
	{
		private static readonly Regex pattern = new Regex(@"^""(?<Item>.+)""\s+cancel\s*$", RegexOptions.IgnoreCase);

		private readonly IAuctionProcessor auctionProcessor;
		private readonly ILogger<CancelAuctionCommand> log;
		public string CommandDescription => "Cancel a Bid";
		public string DmSyntax => "\"{Item_Name_With_Typos}\" {Character} {Amount} {Rank}";

		public CancelBidCommand(IAuctionProcessor auctionProcessor, ILogger<CancelAuctionCommand> log)
		{
			this.auctionProcessor = auctionProcessor;
			this.log = log;
		}

		public async Task<bool> TryInvokeAsync(IMessage message)
		{
			if (!(message?.Channel is IPrivateChannel))
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
	}
}
