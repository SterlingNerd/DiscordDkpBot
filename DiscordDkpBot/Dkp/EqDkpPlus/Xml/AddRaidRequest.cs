using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace DiscordDkpBot.Dkp.EqDkpPlus.Xml
{
	[XmlType("request")]
	public class AddRaidRequest
	{
		[XmlElement("raid_date")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string DateString
		{
			get => Date.ToString("yyyy-MM-dd hh:mm");
			set => Date = DateTime.Parse(value);
		}
		[XmlIgnore]
		public DateTime Date { get; set; }
		[XmlArray("raid_attendees")]
		[XmlArrayItem("member")]
		public int[] Attendees { get; set; }
		[XmlElement("raid_value")]
		public decimal Value { get; set; }
		[XmlElement("raid_event_id")]
		public int EventId { get; set; }
		[XmlElement("raid_note")]
		public string Note { get; set; }

		private AddRaidRequest ()
		{
			// Required for xml serializer
		}

		public AddRaidRequest (DateTime date, int eventId, string note, int botCharacterId)
		{
			Date = date;
			EventId = eventId;
			Note = note;
			Attendees = new[] { botCharacterId };
		}
	}
}
