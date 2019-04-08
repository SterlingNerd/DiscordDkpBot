using System;

namespace DiscordDkpBot.Commands
{
	public interface IChannelCommand : ICommand
	{
		string ChannelSyntax { get; }
	}
}
