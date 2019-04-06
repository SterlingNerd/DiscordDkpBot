using System;
using System.IO;
using System.Runtime.Versioning;
using System.Xml.Serialization;

using DiscordDkpBot.Dkp.EqDkpPlus.Xml;

using NUnit.Framework;

namespace DiscordDkpBotTests.Dkp.EqDkpPlus.Xml
{
	[TestFixture]
	public class GetRaidsResponseTests
	{
		[Test]
		public void DeserializeTest ()
		{
			//Arrange
			XmlSerializer serializer = new XmlSerializer(typeof(GetRaidsResponse));

			GetRaidsResponse response;

			//Act
			using (StringReader reader = new StringReader(TestResources.GetRaids))
			{
				response = serializer.Deserialize(reader) as GetRaidsResponse;
			}

			//Assert
			Assert.IsNotNull(response);
			Assert.AreEqual(1, response.Status);
			Assert.AreEqual(10, response.Raids.Length);
		}
	}
}
