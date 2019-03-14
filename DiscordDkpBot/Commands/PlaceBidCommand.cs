using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Discord.WebSocket;

namespace DiscordDkpBot.Commands
{
	public class PlaceBidCommand : IChatCommand
	{
		public bool DoesCommandApply (SocketMessage message)
		{
			return false;
		}

		public Task InvokeAsync (SocketMessage message)
		{
			throw new NotImplementedException();
		}
	}
}
