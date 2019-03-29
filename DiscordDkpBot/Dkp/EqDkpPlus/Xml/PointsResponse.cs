using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DiscordDkpBot.Dkp.EqDkpPlus.Xml
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

		[XmlElement("EqDkpPlus")]
		public EqDkpPlusInfo EqDkpPlusInfo { get; set; }

		[XmlArray("itempools")]
		public ItemPool[] ItemPools { get; set; }
	}
}
