using System;
using System.Runtime.Serialization;

namespace DiscordDkpBot
{
	public class DkpBotException : Exception
	{
		public DkpBotException()
		{
		}

		public DkpBotException(string message) : base(message)
		{
		}

		public DkpBotException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected DkpBotException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
