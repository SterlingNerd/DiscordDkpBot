using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace DiscordDkpBot.Dkp.EqDkpPlus.Xml
{
	[XmlType("raid")]
	public class RaidInfo
	{
		[XmlElement("event_id")]
		public int EventId { get; set; }

		[XmlElement("event_name")]
		public string EventName { get; set; }

		[XmlElement("added_by_id")]
		public int AddedById { get; set; }

		[XmlElement("added_by_name")]
		public string AddedByName { get; set; }

		[XmlIgnore]
		public DateTimeOffset Date => DateTimeOffset.UnixEpoch.AddSeconds(DateTimestamp);

		[XmlElement("id")]
		public int Id { get; set; }

		[XmlElement("date")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string DateString { get; set; }

		[XmlElement("date_timestamp")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public long DateTimestamp { get; set; }

		[XmlElement("value")]
		public decimal Value { get; set; }

		public override string ToString()
		{
			return $"({Id}) : {EventName}({EventId}) @{Date}";
		}
	}
}
