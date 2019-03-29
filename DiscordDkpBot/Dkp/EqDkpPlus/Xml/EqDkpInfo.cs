using System;
using System.Xml.Serialization;

namespace DiscordDkpBot.Dkp.EqDkpPlus.Xml
{
	[XmlType ("EqDkpPlus")]
	public class EqDkpPlusInfo
	{
		[XmlElement ("dkp_name")]
		public string DkpName { get; set; }

		[XmlElement ("version")]
		public string Version { get; set; }

		[XmlElement ("layout")]
		public string Layout { get; set; }

		[XmlElement ("base_layout")]
		public string BaseLayout { get; set; }

		[XmlElement ("name")]
		public string Name { get; set; }

		[XmlElement ("guild")]
		public string Guild { get; set; }
	}
}
