using System;
using System.Xml.Serialization;

namespace DiscordDkpBot.Dkp.EqDkpPlus.Xml
{
	public class RaidAttendee
	{
		[XmlElement("member")]
		public int Id { get; set; }
	}
}
