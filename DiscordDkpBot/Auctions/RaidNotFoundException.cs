using System;
using System.Runtime.Serialization;

namespace DiscordDkpBot.Auctions
{
	public class RaidNotFoundException : InvalidOperationException
	{
		public RaidNotFoundException()
		{
		}

		public RaidNotFoundException(string message) : base(message)
		{
		}

		public RaidNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected RaidNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
