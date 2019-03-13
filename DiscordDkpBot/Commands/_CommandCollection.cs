using System;
using System.Collections.Generic;

namespace DiscordDkpBot.Commands
{
	public class CommandCollection : Dictionary<string, IChatCommand>
	{
		public CommandCollection () : base(StringComparer.OrdinalIgnoreCase)
		{
		}

		public CommandCollection (params IChatCommand[] chatCommands) : this()
		{
			foreach (IChatCommand chatCommand in chatCommands)
			{
				if (chatCommand != null)
				{
					Add(chatCommand.CommandTrigger, chatCommand);
				}
			}
		}
	}
}
