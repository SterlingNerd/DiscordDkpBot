using System;
using System.Runtime.Serialization;

namespace DiscordDkpBot.Auctions
{
	public class InvalidBidException : DkpBotException
	{
		public InvalidBidException()
		{
		}

		public InvalidBidException(string message) : base(message)
		{
		}

		public InvalidBidException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidBidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
