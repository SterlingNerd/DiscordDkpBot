using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DiscordDkpBot.Dkp.EqDkpPlus.Xml
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

		private AddRaidRequest()
		{
			// Required for xml serializer
		}

		public AddRaidRequest(DateTime date, int eventId, string note)
		{
			Date = date;
			EventId = eventId;
			Note = note;
		}
	}
}
