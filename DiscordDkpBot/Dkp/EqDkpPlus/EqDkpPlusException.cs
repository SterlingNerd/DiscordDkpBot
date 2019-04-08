using System;

using DiscordDkpBot.Dkp.EqDkpPlus.Xml;

namespace DiscordDkpBot.Dkp.EqDkpPlus
{
	public class EqDkpPlusException : DkpBotException
	{
		public EqDkpPlusException(ErrorResponse errorResponse) : base(errorResponse.Error)
		{
		}
	}
}
