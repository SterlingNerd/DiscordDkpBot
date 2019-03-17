using System;
using System.Xml.Serialization;

namespace DiscordDkpBot.Dkp.EqDkp.Xml
{
	public class MultiDkpPool
	{
		[XmlElement ("id")]
		public int Id { get; set; }

		[XmlElement ("name")]
		public string Name { get; set; }

		[XmlElement ("desc")]
		public string Description { get; set; }

		[XmlArray ("events")]
		public DkpEvent[] Events { get; set; }

		[XmlArray ("multidkp_pools")]
		public MultiDkpItemPool[] DkpPools { get; set; }

		[XmlArray ("itempools")]
		public ItemPool[] ItemPools { get; set; }
	}
}
