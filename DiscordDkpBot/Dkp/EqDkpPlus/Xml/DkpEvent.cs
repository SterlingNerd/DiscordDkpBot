using System;
using System.Xml.Serialization;

namespace DiscordDkpBot.Dkp.EqDkpPlus.Xml
{
	[XmlType("event")]
	public class DkpEvent
	{
		[XmlArray("multidkp_pools")]
		public MultiDkpPool[] DkpPools { get; set; }

		[XmlElement("name")]
		public string Name { get; set; }

		[XmlElement("value")]
		public decimal Value { get; set; }

		[XmlElement("icon")]
		public string Icon { get; set; }

		[XmlElement("id")]
		public int Id { get; set; }

		[XmlArray("itempools")]
		public ItemPool[] ItemPools { get; set; }

		public Dkp.DkpEvent ToCore ()
		{
			return new Dkp.DkpEvent(Id, Name, Value);
		}
	}
}
