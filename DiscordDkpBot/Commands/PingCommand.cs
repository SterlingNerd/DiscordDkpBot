using System;
using System.Threading.Tasks;

using Discord.WebSocket;

namespace DiscordDkpBot.Commands
{
	public class PingCommand : ChatCommand
	{
		public PingCommand () : base("ping")
		{
		}

		public override async Task InvokeAsync (SocketMessage message)
		{
			await message.Channel.SendMessageAsync($"pong {GetArgsString(message)}");
		}
	}
}
