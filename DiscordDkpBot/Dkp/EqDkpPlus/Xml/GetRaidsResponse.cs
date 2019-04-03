using System;
using System.Xml.Serialization;

namespace DiscordDkpBot.Dkp.EqDkpPlus.Xml
{
	[XmlType("response")]
	public class GetRaidsResponse
	{
		[XmlElement("raid")]
		public RaidInfo[] Raids { get; set; } = { };

		[XmlElement("status")]
		public int Status { get; set; }
	}
}
