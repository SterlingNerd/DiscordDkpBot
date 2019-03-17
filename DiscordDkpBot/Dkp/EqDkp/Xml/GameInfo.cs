using System;
using System.Xml.Serialization;

namespace DiscordDkpBot.Dkp.EqDkp.Xml
{
	[XmlType ("game")]
	public class GameInfo
	{
		[XmlElement ("name")]
		public string Name { get; set; }

		[XmlElement ("version")]
		public string Version { get; set; }

		[XmlElement ("language")]
		public string Language { get; set; }

		[XmlElement ("server_name")]
		public string ServerName { get; set; }

		[XmlElement ("server_loc")]
		public string ServerLocation { get; set; }
	}
}
