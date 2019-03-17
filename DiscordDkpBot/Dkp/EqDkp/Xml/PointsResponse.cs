using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DiscordDkpBot.Dkp.EqDkp.Xml
{
	[XmlType("response")]
	public class PointsResponse
	{
		[XmlElement("game")]
		public GameInfo GameInfo { get; set; }

		[XmlElement("info")]
		public PointsInfo PointsInfo { get; set; }

		[XmlArray("players")]
		public Player[] Players { get; set; }

		[XmlArray("multidkp_pools")]
		public MultiDkpPool[] DkpPools { get; set; }

		[XmlElement("eqdkp")]
		public EqDkpInfo EqDkpInfo { get; set; }

		[XmlArray("itempools")]
		public ItemPool[] ItemPools { get; set; }
	}
}
