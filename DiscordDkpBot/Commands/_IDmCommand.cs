using System;

namespace DiscordDkpBot.Commands
{
	public interface IDmCommand : ICommand
	{
		string DmSyntax { get; }
	}
}
