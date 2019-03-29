using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace DiscordDkpBot.Dkp.EqDkpPlus.Xml
{
	public class PointsInfo
	{
		[XmlElement ("with_twink")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public int _WithTwink { get; set; }

		[XmlElement ("date")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public string _dateString { get; set; }

		[XmlElement ("timestamp")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public int _timestamp { get; set; }

		[XmlIgnore]
		public DateTime TimestampUtc => DateTime.UnixEpoch.AddSeconds(_timestamp);

		[XmlElement ("total_players")]
		public int TotalPlayers { get; set; }

		[XmlElement ("total_items")]
		public int TotalItems { get; set; }

		public bool WithTwink => _WithTwink != 0;
	}
}
