using System;
using System.IO;
using System.Xml.Serialization;

using DiscordDkpBot.Dkp.EqDkpPlus.Xml;

using NUnit.Framework;

namespace DiscordDkpBotTests.Dkp.EqDkp.Xml
{
	[TestFixture]
	public class AddRaidRequestTests
	{
		[Test]
		public void DeserializeTest()
		{
			//Arrange
			const string xml = @"<request>
<raid_date>2015-02-01 19:30</raid_date>
<raid_attendees><member>1</member><member>2</member></raid_attendees>
<raid_value>10</raid_value>
<raid_event_id>1</raid_event_id>
<raid_note>Notiz</raid_note>
</request>";

			XmlSerializer serializer = new XmlSerializer(typeof(AddRaidRequest));

			AddRaidRequest response;

			//Act
			using (StringReader reader = new StringReader(xml))
			{
				response = serializer.Deserialize(reader) as AddRaidRequest;
			}

			//Assert
			Assert.IsNotNull(response);
			Assert.AreEqual(new DateTime(2015, 02, 01, 19, 30, 00), response.Date);
			Assert.AreEqual(1, response.Attendees[0]);
			Assert.AreEqual(2, response.Attendees[1]);
			Assert.AreEqual(10, response.Value);
			Assert.AreEqual("Notiz", response.Note);
			Assert.AreEqual(1, response.EventId);
		}
	}
}
