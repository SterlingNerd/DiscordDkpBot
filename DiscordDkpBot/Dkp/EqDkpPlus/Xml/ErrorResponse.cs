using System;
using System.Xml.Serialization;

namespace DiscordDkpBot.Dkp.EqDkpPlus.Xml
{
	[XmlType("response")]
	public class ErrorResponse
	{
		[XmlElement("status")]
		public int Status { get; set; }

		[XmlElement("error")]
		public string Error { get; set; }
	}
}
