using System;
using System.Runtime.Serialization;

namespace DiscordDkpBot.Auctions
{
	public class AuctionNotFoundException : Exception
	{
		public AuctionNotFoundException ()
		{
		}

		public AuctionNotFoundException (string message) : base(message)
		{
		}

		public AuctionNotFoundException (string message, Exception innerException) : base(message, innerException)
		{
		}

		protected AuctionNotFoundException (SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
