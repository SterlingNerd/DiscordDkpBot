using System;
using System.Threading.Tasks;

using Discord.WebSocket;

namespace DiscordDkpBot.Commands
{
	public interface IChatCommand
	{
		bool DoesCommandApply (SocketMessage message);
		Task InvokeAsync (SocketMessage message);
	}
}
