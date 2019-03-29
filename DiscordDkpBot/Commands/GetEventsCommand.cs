using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Discord;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Configuration;
using DiscordDkpBot.Dkp;
using DiscordDkpBot.Dkp.EqDkpPlus.Xml;
using DiscordDkpBot.Extensions;

using Microsoft.Extensions.Logging;

namespace DiscordDkpBot.Commands
{
	public class GetEventsCommand : ICommand
	{
		private static readonly string[] CommandTriggers = { "getevent", "getevents" };
		private readonly IDkpProcessor dkpProcessor;
		private readonly ILogger<GetEventsCommand> log;
		private readonly Regex pattern;

		public GetEventsCommand(IDkpProcessor dkpProcessor, ILogger<GetEventsCommand> log)
		{
			string regex = $@"^\s*(?<trigger>{string.Join('|', CommandTriggers)})($|\s+(?<name>.*)$)";
			pattern = new Regex(regex, RegexOptions.IgnoreCase);

			this.dkpProcessor = dkpProcessor;
			this.log = log;
		}

		public (bool success, string name) ParseArgs(string args)
		{
			Match match = pattern.Match(args);

			if (!match.Success)
			{
				return (false, null);
			}

			string name = match.Groups["name"].Value;

			log.LogTrace("Parsed event arguments: {0}", name);

			return (true, name);
		}

		public async Task<bool> TryInvokeAsync(IMessage message)
		{
			try
			{
				if (message == null)
				{
					return false;
				}
				(bool success, string name) = ParseArgs(message.Content);

				if (!success)
				{
					return false;
				}
				else if (!(message.Channel is IPrivateChannel))
				{
					return false;
				}
				else
				{
					List<DkpEvent> events = (await dkpProcessor.GetEvents(name)).ToList();

					log.LogDebug("{0} Events Found.", events.Count);

					StringBuilder builder = new StringBuilder();
					if (events.Any())
					{

						builder.AppendLine("```json");
						foreach (DkpEvent e in events)
						{
							builder.AppendLine($"{{{e.Id} : \"{e.Name}\"}}");
						}
						builder.AppendLine("```");
					}
					else if (!string.IsNullOrWhiteSpace(name))
					{
						builder.AppendLine("There are no events that match your query.");
					}
					else
					{
						builder.AppendLine("You have no favorite events. Add some in the configs!");
					}

					await message.Channel.SendMessageAsync(builder.ToString());
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
				await message.Channel.SendMessageAsync($"Yer doin it wrong!");
				return false;
			}
		}
	}
}
