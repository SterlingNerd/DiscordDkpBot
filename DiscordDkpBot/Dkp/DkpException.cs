using System;
using System.Runtime.Serialization;

namespace DiscordDkpBot.Dkp
{
	public class DkpException : Exception
	{
		public DkpException()
		{
		}

		public DkpException(string message) : base(message)
		{
		}

		public DkpException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected DkpException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
