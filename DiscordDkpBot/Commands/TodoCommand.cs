using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Discord;

using DiscordDkpBot.Configuration;

namespace DiscordDkpBot.Commands
{
	public class TodoCommand : IChannelCommand
	{
		private readonly DkpBotConfiguration config;
		public const string todo = @"```
 - attach to existing raid Id
 - list recent raids
 - get attached raid status
 - reign in dkpcheck (so it doesn't trigger on "".dkp todo"" etc)
 - add class leaderboards
 - add configuration options for messages.
 ```";
		public TodoCommand(DkpBotConfiguration config)
		{
			this.config = config;
		}
		public async Task<bool> TryInvokeAsync(IMessage message)
		{
			if (message.Content?.Equals(ChannelSyntax, StringComparison.OrdinalIgnoreCase) == true)
			{
				await message.Channel.SendMessageAsync(todo);
				return true;
			}
			return false;
		}

		public string ChannelSyntax => $"{config.CommandPrefix} todo";
	}
}
