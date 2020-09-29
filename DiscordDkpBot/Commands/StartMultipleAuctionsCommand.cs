using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Discord;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Configuration;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Commands
{
	public class StartMultipleAuctionsCommand : IChannelCommand
	{
		private static readonly string[] CommandTriggers = { "startmany", "startmultiple" };
		private readonly IAuctionProcessor auctionProcessor;
		private readonly DiscordConfiguration configuration;
		private readonly ILogger<StartMultipleAuctionsCommand> log;
		private readonly Regex pattern;
		private readonly Regex itemPattern;

		public string ChannelSyntax => $"{configuration.CommandPrefix} {CommandTriggers.First()}:\nItem1\n2xItem2\nEpicness, The Most Epic of Epics\netc.\netc.";
		public string CommandDescription => "Start Multiple Item Auctions Using Default Duration (No quotes needed. One item per line)";

		public StartMultipleAuctionsCommand(DiscordConfiguration configuration, IAuctionProcessor auctionProcessor, ILogger<StartMultipleAuctionsCommand> log)
		{
			string regex = "^" + Regex.Escape(configuration.CommandPrefix) + @"\s*(?<trigger>" + string.Join('|', CommandTriggers) + @"):?\s*\n?(?<items>(?:\s*\d*x?.+)+)";
			pattern = new Regex(regex, RegexOptions.IgnoreCase);

			itemPattern = new Regex("(?:^\\s*(?<quantity>\\d+)*x?\\s*(?<name>.+?)\\s*$)+", RegexOptions.IgnoreCase | RegexOptions.Multiline);

			this.configuration = configuration;
			this.auctionProcessor = auctionProcessor;
			this.log = log;
		}

		public async Task<bool> TryInvokeAsync(IMessage message)
		{
			if (message == null)
			{
				return false;
			}
			else if (message.Channel is IPrivateChannel)
			{
				return false;
			}
			else if (message.Channel.Name != configuration.SilentAuctionsChannelName)
			{
				return false;
			}

			(bool success, Dictionary<string, int> items) = ParseArgs(message.Content);

			if (!success)
			{
				return false;
			}
			else
			{
				List<Task> auctions = new List<Task>();
				foreach (KeyValuePair<string, int> item in items)
				{
					auctions.Add(auctionProcessor.StartAuction(item.Value, item.Key, null, message));
				}
				await Task.WhenAll(auctions);

				return true;
			}

		}

		public (bool success, Dictionary<string, int> items) ParseArgs(string args)
		{
			args = args.Replace('“', '"').Replace('”', '"');
			Match match = pattern.Match(args);

			if (!match.Success || !match.Groups["items"].Success)
			{
				return (false, new Dictionary<string, int>());
			}

			string itemsString = match.Groups["items"].Value;
			IEnumerable<Match> itemsMatch = itemPattern.Matches(itemsString);

			Dictionary<string, int> items = new Dictionary<string, int>();
			foreach (Match itemMatch in itemsMatch)
			{
				string name = itemMatch.Groups["name"].Value;
				if (! int.TryParse(itemMatch.Groups["quantity"]?.Value, out int quantity))
				{
					quantity = 1;
				}

				if (!items.ContainsKey(name))
				{
					items.Add(name, 0);
				}

				items[name]+= quantity;
			}

			return (true, items);
		}
	}
}