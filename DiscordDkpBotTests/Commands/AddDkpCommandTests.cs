using System;

using DiscordDkpBot.Commands;
using DiscordDkpBot.Configuration;
using DiscordDkpBot.Dkp;

using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

namespace DiscordDkpBotTests.Commands
{
	[TestFixture]
	public class AddDkpCommandTests
	{
		[TestCase("@@ adddkp 170 10 foo\njimmy", 170, 10, "foo", "jimmy")]
		[TestCase("@@ AddDkp 170 10\njimmy", 170, 10, "", "jimmy")]
		[TestCase("@@ AddDkp 170 10\njimmy\nSammy", 170, 10, "", "jimmy\nSammy")]
		[TestCase("@@ AddDkp 170 10\n\tjimmy\n\tSammy", 170, 10, "", "\tjimmy\n\tSammy")]
		[TestCase("@@ ADDDKP 1 1000 foo for a comment\njimmy is cool", 1, 1000, "foo for a comment", "jimmy is cool")]
		public void ParseArgs (string content, int expectedEventId, int expectedValue, string expectedComment, string expectedLines)
		{
			//Act
			(bool success, int eventId, int value, string comment, string lines) = target.ParseArgs(content);

			//Assert
			Assert.AreEqual(expectedEventId, eventId, "Event Id must match.");
			Assert.AreEqual(expectedValue, value, "Value must match.");
			Assert.AreEqual(expectedComment, comment, "Comment must match.");
			Assert.AreEqual(expectedLines, lines, "Lines must match");
		}

		#region Setup/Teardown

		[SetUp]
		public void SetUp ()
		{
			var configuration = new DkpBotConfiguration();
			configuration.CommandPrefix = "@@";
			target = new AddDkpCommand(configuration, new Mock<IDkpProcessor>().Object, new Mock<ILogger<AddDkpCommand>>().Object);
		}

		#endregion

		#region Test Helpers

		private AddDkpCommand target;


		#endregion
	}
}
