using System;
using System.Xml.Serialization;

namespace DiscordDkpBot.Dkp.EqDkpPlus.Xml
{
	[XmlType("response")]
	public class EventsResponse
	{
		[XmlElement("event")]
		public DkpEvent[] Events { get; set; }

		[XmlElement("status")]
		public int Status { get; set; }
	}
}
