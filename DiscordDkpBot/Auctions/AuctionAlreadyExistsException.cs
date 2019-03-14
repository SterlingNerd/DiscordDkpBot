using System;
using System.Runtime.Serialization;

namespace DiscordDkpBot.Auctions
{
	public class AuctionAlreadyExistsException : Exception
	{
		public AuctionAlreadyExistsException ()
		{
		}

		public AuctionAlreadyExistsException (string message) : base(message)
		{
		}

		public AuctionAlreadyExistsException (string message, Exception innerException) : base(message, innerException)
		{
		}

		protected AuctionAlreadyExistsException (SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
