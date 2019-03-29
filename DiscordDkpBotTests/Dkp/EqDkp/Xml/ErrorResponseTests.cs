using System;
using System.IO;
using System.Xml.Serialization;

using DiscordDkpBot.Dkp.EqDkpPlus.Xml;

using NUnit.Framework;

namespace DiscordDkpBotTests.Dkp.EqDkpPlus.Xml
{
	[TestFixture]
	public class ErrorResponseTests
	{
		[Test]
		public void DeserializeTest()
		{
			//Arrange
			const string xml = "<response>\r\n\t<status>0</status>\r\n\t<error>access denied</error>\r\n</response>";
			XmlSerializer serializer = new XmlSerializer(typeof(ErrorResponse));

			ErrorResponse response;

			//Act
			using (StringReader reader = new StringReader(xml))
			{
				response = serializer.Deserialize(reader) as ErrorResponse;
			}

			//Assert
			Assert.IsNotNull(response);
			Assert.AreEqual(0, response.Status);
			Assert.AreEqual("access denied", response.Error);
		}
	}
}
