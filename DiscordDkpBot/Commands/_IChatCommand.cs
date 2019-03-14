using System;
using System.Threading.Tasks;

using Discord.WebSocket;

namespace DiscordDkpBot.Commands
{
	public interface ICommand
	{
		bool DoesCommandApply (SocketMessage message);
		Task<bool> InvokeAsync (SocketMessage message);
	}
}
