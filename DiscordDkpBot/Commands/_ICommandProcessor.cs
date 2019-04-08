using System;
using System.Threading.Tasks;

using Discord.WebSocket;

namespace DiscordDkpBot.Commands
{
	public interface ICommandProcessor
	{
		Task ProcessCommand(SocketMessage message);
	}
}
