using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace DiscordDkpBot.Dkp.EqDkp.Xml
{
	[XmlType("player")]
	public class Player
	{
		[XmlElement ("id")]
		public int Id { get; set; }

		[XmlElement ("name")]
		public string Name { get; set; }

		[XmlElement ("active")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public int _Active { get; set; }

		[XmlElement ("main_id")]
		public int MainId { get; set; }

		[XmlElement ("main_name")]
		public string MainName { get; set; }

		[XmlElement ("class_id")]
		public int ClassId { get; set; }

		[XmlElement ("hidden")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public int _Hidden { get; set; }

		[XmlIgnore]
		public bool Active => _Active != 0;

		[XmlElement ("class_name")]
		public string ClassName { get; set; }

		[XmlArray ("points")]
		public PlayerPoints[] Points { get; set; }

		[XmlArray ("items")]
		public Item[] Items { get; set; }

		[XmlArray ("adjustments")]
		public Adjustment[] Adjustments { get; set; }

		[XmlIgnore]
		public bool Hidden => _Hidden != 0;
	}
}
