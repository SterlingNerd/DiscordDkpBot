using System;

using DiscordDkpBot.Commands;
using DiscordDkpBot.Dkp;

using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

namespace DiscordDkpBotTests.Commands
{
	[TestFixture]
	public class GetEventsCommandTests
	{
		[TestCase("getevents pop", "pop")]
		[TestCase("getevents", "")]
		public void ParseArgs(string args, string expectedName)
		{
			//Act
			(bool success, string name) = target.ParseArgs(args);

			//Assert
			Assert.AreEqual(expectedName, name);
		}

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			target = new GetEventsCommand(new Mock<IDkpProcessor>().Object, new Mock<ILogger<GetEventsCommand>>().Object);
		}

		#endregion

		#region Test Helpers

		private GetEventsCommand target;

		#endregion
	}
}
