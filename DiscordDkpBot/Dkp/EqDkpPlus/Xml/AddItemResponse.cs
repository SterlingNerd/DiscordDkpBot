using System;
using System.Xml.Serialization;

namespace DiscordDkpBot.Dkp.EqDkpPlus.Xml
{
	[XmlType("response")]
	public class AddItemResponse
	{
		[XmlElement("item_id")]
		public int ItemId { get; set; }

		[XmlElement("status")]
		public int Status { get; set; }
	}
}
