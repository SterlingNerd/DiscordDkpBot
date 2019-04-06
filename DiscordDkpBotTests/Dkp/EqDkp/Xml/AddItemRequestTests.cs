using System;
using System.IO;
using System.Xml.Serialization;

using DiscordDkpBot.Dkp.EqDkpPlus.Xml;

using NUnit.Framework;

namespace DiscordDkpBotTests.Dkp.EqDkpPlus.Xml
{
	[TestFixture]
	public class AddItemRequestTests
	{
		[Test]
		public void DeserializeTest()
		{
			//Arrange
			const string xml = @"<request>
<item_date>2015-02-01 19:30</item_date>
<item_buyers><member>1</member><member>2</member></item_buyers>
<item_value>10</item_value>
<item_name>Axt</item_name>
<item_raid_id>3</item_raid_id>
<item_itempool_id>1</item_itempool_id>
</request>";

			XmlSerializer serializer = new XmlSerializer(typeof(AddItemRequest));

			AddItemRequest response;

			//Act
			using (StringReader reader = new StringReader(xml))
			{
				response = serializer.Deserialize(reader) as AddItemRequest;
			}

			//Assert
			Assert.IsNotNull(response);
			Assert.AreEqual(new DateTime(2015, 02, 01, 19, 30, 00), response.ItemDate);
			Assert.AreEqual(1, response.Buyers[0]);
			Assert.AreEqual(2, response.Buyers[1]);
			Assert.AreEqual(10, response.Value);
			Assert.AreEqual("Axt", response.ItemName);
			Assert.AreEqual(3, response.RaidId);
			Assert.AreEqual(1, response.ItemPoolId);
		}
	}
}
