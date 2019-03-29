using System;
using System.Xml.Serialization;

namespace DiscordDkpBot.Dkp.EqDkpPlus.Xml
{
	[XmlType ("item")]
	public class Item
	{
		[XmlElement ("game_id")]
		public string GameId { get; set; }

		[XmlElement ("name")]
		public string Name { get; set; }

		[XmlElement ("value")]
		public decimal Value { get; set; }

		[XmlElement ("itempool_id")]
		public int ItemPoolId { get; set; }
	}
}
