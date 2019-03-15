using System;
using System.Threading.Tasks;

using Discord;

namespace DiscordDkpBot.Commands
{
	public interface ICommand
	{
		bool DoesCommandApply (IMessage message);
		Task<bool> InvokeAsync (IMessage message);
	}
}
