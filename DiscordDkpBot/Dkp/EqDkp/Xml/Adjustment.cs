using System;
using System.Xml.Serialization;

namespace DiscordDkpBot.Dkp.EqDkp.Xml
{
	public class Adjustment
	{
		private static readonly DateTime epoch = DateTime.SpecifyKind(new DateTime(1970, 01, 01), DateTimeKind.Utc);

		[XmlElement ("reason")]
		public string Reason { get; set; }

		[XmlElement ("value")]
		public decimal Value { get; set; }

		[XmlElement ("timestamp")]
		public int Timestamp { get; set; }

		[XmlElement ("event_id")]
		public int EventId { get; set; }

		public DateTime TimestampLocal => TimestampUtc.ToLocalTime();
		public DateTime TimestampUtc => DateTime.UnixEpoch.AddSeconds(Timestamp);
	}
}
