using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using DiscordDkpBot.Dkp.EqDkpPlus.Xml;

using NUnit.Framework;

namespace DiscordDkpBotTests.Dkp.EqDkpPlus.Xml
{
	[TestFixture]
	public class EventsResponseTests
	{
		[Test]
		public void DeserializeTest ()
		{
			//Arrange
			XmlSerializer serializer = new XmlSerializer(typeof(EventsResponse));

			EventsResponse response;
			//Act
			using (StringReader reader = new StringReader(TestResources.EventsXml))
			{
				response = serializer.Deserialize(reader) as EventsResponse;
			}

			//Assert
			Assert.IsNotNull(response);
			Assert.IsNotNull(response.Events);
			Assert.IsTrue(response.Events.Length > 0);
			Assert.IsNotNull(response.Events[0]);
			Assert.AreEqual(1, response.Events[0].Id);
		}
	}
}
