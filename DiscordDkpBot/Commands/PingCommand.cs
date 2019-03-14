using System;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using DiscordDkpBot.Configuration;
using DiscordDkpBot.Extensions;

namespace DiscordDkpBot.Commands
{
	public class PingCommand : BasicChatCommand
	{
		public PingCommand (DkpBotConfiguration config) : base(config.CommandPrefix, "ping")
		{
		}

		public override async Task<bool> InvokeAsync (SocketMessage message)
		{
			await message.Channel.SendMessageAsync($"pong {message.Content.RemoveFirstWord()}");
			return true;
		}
	}
}
