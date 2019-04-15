using System;
using System.Runtime.Serialization;

namespace DiscordDkpBot.Auctions
{
	public class InsufficientDkpException : DkpBotException
	{
		public InsufficientDkpException()
		{
		}

		public InsufficientDkpException(string message) : base(message)
		{
		}

		public InsufficientDkpException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected InsufficientDkpException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}