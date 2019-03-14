using System;
using System.Threading.Tasks;

using Discord.WebSocket;

namespace DiscordDkpBot.Commands
{
	public abstract class ChatCommand : IChatCommand
	{
		protected ChatCommand (string commandTrigger)
		{
			if (string.IsNullOrWhiteSpace(commandTrigger))
			{
				throw new ArgumentNullException(nameof(commandTrigger));
			}

			CommandTrigger = commandTrigger;
		}

		public string CommandTrigger { get; }

		public abstract Task InvokeAsync (SocketMessage message);

		protected string GetArgsString (SocketMessage message)
		{
			return GetArgsString(message.ToString());
		}

		protected string GetArgsString (string chatMessage)
		{
			if (chatMessage == null)
			{
				return null;
			}

			string firstWord = chatMessage.IndexOf(CommandTrigger, StringComparison.OrdinalIgnoreCase) > -1
				? chatMessage.Substring(chatMessage.IndexOf(CommandTrigger, StringComparison.OrdinalIgnoreCase) + CommandTrigger.Length)
				: chatMessage;
			return firstWord;
		}
	}
}
