using System;
using System.Threading.Tasks;

using Discord.WebSocket;

namespace DiscordDkpBot.Commands
{
	public interface IChatCommand
	{
		string CommandTrigger { get; }
		Task InvokeAsync (SocketMessage message);
	}
}
