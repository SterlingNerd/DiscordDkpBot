using System;
using System.Threading.Tasks;

using Discord;

namespace DiscordDkpBot.Commands
{
	public interface ICommand
	{
		Task<bool> TryInvokeAsync (IMessage message);
	}
}
