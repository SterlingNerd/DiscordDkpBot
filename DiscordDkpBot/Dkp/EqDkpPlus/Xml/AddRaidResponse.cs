using System;
using System.Xml.Serialization;

namespace DiscordDkpBot.Dkp.EqDkpPlus.Xml
{
	[XmlType("response")]
	public class AddRaidResponse
	{
		[XmlElement("raid_id")]
		public int Id { get; set; }

		[XmlElement("status")]
		public int Status { get; set; }
	}
}
