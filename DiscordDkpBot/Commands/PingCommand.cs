using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Discord;

using DiscordDkpBot.Configuration;

namespace DiscordDkpBot.Commands
{
	public class PingCommand : ICommand
	{
		private readonly Regex pattern;

		public PingCommand (DkpBotConfiguration config)
		{
			pattern = new Regex("^" + config.CommandPrefix + "ping (?<content>.*)", RegexOptions.IgnoreCase);
		}

		public async Task<bool> TryInvokeAsync (IMessage message)
		{
			Match match = pattern.Match(message.Content);

			if (match.Success)
			{
				await message.Channel.SendMessageAsync($"pong {match.Groups["content"]}");
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
