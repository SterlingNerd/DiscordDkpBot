using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace DiscordDkpBot.Dkp.EqDkpPlus.Xml
{
	[XmlType("request")]
	public class AddItemRequest
	{
		[XmlArray("item_buyers")]
		[XmlArrayItem("member")]
		public int[] Buyers { get; set; }

		[XmlIgnore]
		public DateTime ItemDate { get; set; }

		[XmlElement("item_date")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string ItemDateString
		{
			get => ItemDate.ToString("YYYY-MM-DD hh:mm");
			set => ItemDate = DateTime.Parse(value);
		}

		[XmlElement("item_value")]
		public decimal Value { get; set; }

		[XmlElement("item_name")]
		public string ItemName { get; set; }

		[XmlElement("item_raid_id")]
		public int RaidId { get; set; }

		[XmlElement("item_itempool_id")]
		public int ItemPoolId { get; set; }

		public override string ToString()
		{
			return $"{ItemName} bought by characterIds '{string.Join(", ", Buyers)}', in raid '{RaidId}'";
		}

		public AddItemRequest(int buyer, DateTime itemDate, string itemName, int itemPoolId, int raidId, decimal value)
			: this(new[] { buyer }, itemDate, itemName, itemPoolId, raidId, value)
		{
		}

		public AddItemRequest(int[] buyers, DateTime itemDate, string itemName, int itemPoolId, int raidId, decimal value)
		{
			Buyers = buyers;
			ItemDate = itemDate;
			ItemName = itemName;
			ItemPoolId = itemPoolId;
			RaidId = raidId;
			Value = value;
		}

		private AddItemRequest()
		{
			//Required for xml serializer.
		}
	}
}
