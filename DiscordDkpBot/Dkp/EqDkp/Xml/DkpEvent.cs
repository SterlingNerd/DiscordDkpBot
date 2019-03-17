using System;
using System.Xml.Serialization;

namespace DiscordDkpBot.Dkp.EqDkp.Xml
{
	[XmlType ("event")]
	public class DkpEvent
	{
		[XmlElement ("id")]
		public int Id { get; set; }

		[XmlElement ("name")]
		public string Name { get; set; }

		[XmlElement ("value")]
		public decimal Value { get; set; }
	}
}
