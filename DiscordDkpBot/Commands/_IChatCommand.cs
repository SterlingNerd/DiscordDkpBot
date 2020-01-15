using System;
using System.Threading.Tasks;

using Discord;

namespace DiscordDkpBot.Commands
{
	public interface ICommand
	{
		string CommandDescription { get; }
		Task<bool> TryInvokeAsync(IMessage message);
	}
}
