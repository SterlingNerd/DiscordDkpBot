using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DiscordDkpBot.Dkp.EqDkp.Xml
{
	[XmlType("request")]
	public class AddRaidRequest
	{
		[XmlElement("raid_date")]
		public DateTime Date { get; set; }
		[XmlArray("raid_attendees")]
		public RaidAttendee[] Attendees { get; set; }
		[XmlElement("raid_value")]
		public decimal Value { get; set; }
		[XmlElement("raid_event_id")]
		public int EventId { get; set; }
		[XmlElement("raid_note")]
		public string Note { get; set; }
	}

	public class RaidAttendee
	{
		[XmlElement("member")]
		public int Id { get; set; }
	}

	[XmlType("response")]
	public class AddRaidResponse
	{
		[XmlElement("raid_id")]
		public int Id { get; set; }
		[XmlElement("status")]
		public int Status { get; set; }
	}
}
