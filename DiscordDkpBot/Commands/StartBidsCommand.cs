﻿using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Discord.WebSocket;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Extensions;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Commands
{
	public class StartBidsCommand : ChatCommand
	{
		public const string Syntax = "One item:\t\t\t`\"Item_Name\"`\nTwo of an item:\t\t`2x\"Item_Name\"`\nCustom duration:\t\t`\"Item_Name\" 4`";
		private static readonly Regex pattern = new Regex(@"(?<number>\d+)?x?\s*""(?<name>\w+)""\s*(?<time>\d+)?");
		private readonly IAuctionProcessor auctionProcessor;
		private readonly ILogger<StartBidsCommand> log;

		public StartBidsCommand (IAuctionProcessor auctionProcessor, ILogger<StartBidsCommand> log) : base("startbids")
		{
			this.auctionProcessor = auctionProcessor;
			this.log = log;
		}

		public override Task InvokeAsync (SocketMessage message)
		{
			string args = GetArgsString(message);

			try
			{
				(int? quantity, string name, int? minutes) = ParseArgs(args);
				Auction auction = auctionProcessor.CreateAuction(quantity, name, minutes, message.Channel, message.Author);
				message.Channel.SendMessageAsync($"**{auction}**\nBids are open for **{auction}** for **{auction.Minutes}** minutes.");
			}
			catch (AuctionAlreadyExistsException ex)
			{
				log.LogWarning(ex);
				message.Channel.SendMessageAsync(ex.Message);
			}
			catch (Exception ex)
			{
				log.LogWarning(ex);
				message.Channel.SendMessageAsync($"Yer doin it wrong!\n\nSyntax:\n{Syntax}");
			}

			return Task.CompletedTask;
		}

		public (int? number, string name, int? minutes) ParseArgs (string args)
		{
			Match match = pattern.Match(args);

			if (!match.Success)
			{
				throw new ArgumentException($"Could not parse auction from: '{args}'");
			}

			int? number = match.Groups["number"].Success ? int.Parse(match.Groups["number"].Value) : (int?) null;
			string name = match.Groups["name"].Value;
			int? minutes = match.Groups["time"].Success ? int.Parse(match.Groups["time"].Value) : (int?)null;

			log.LogTrace("Parsed auction arguments: {0}x {1} for {2} mins.", number, name, minutes);

			return (number, name, minutes);
		}
	}
}
