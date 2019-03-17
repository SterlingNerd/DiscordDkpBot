using System;
using System.Xml.Serialization;

namespace DiscordDkpBot.Dkp.EqDkp.Xml
{
	[XmlType ("itempool")]
	public class ItemPool
	{
		[XmlElement ("id")]
		public int Id { get; set; }

		[XmlElement ("name")]
		public string Name { get; set; }

		[XmlElement ("desc")]
		public string Description { get; set; }
	}
}
