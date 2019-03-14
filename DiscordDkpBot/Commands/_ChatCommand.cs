using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Discord.WebSocket;

namespace DiscordDkpBot.Commands
{
	public abstract class BasicChatCommand : IChatCommand
	{
		public string CommandPrefix { get; }

		public string[] CommandTriggers { get; }

		protected BasicChatCommand (string commandPrefix, string commandTrigger) : this(commandPrefix, new[] { commandTrigger })
		{
		}

		protected BasicChatCommand (string commandPrefix, IEnumerable<string> commandTriggers)
		{
			if (commandTriggers == null)
			{
				throw new ArgumentNullException(nameof(commandTriggers));
			}
			CommandPrefix = commandPrefix;
			CommandTriggers = commandTriggers.ToArray();
		}

		public bool DoesCommandApply (SocketMessage message)
		{
			return CommandTriggers.Any(trigger => message?.Content?.StartsWith(CommandPrefix + trigger) == true);
		}

		public abstract Task InvokeAsync (SocketMessage message);
	}
}
