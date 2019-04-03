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
	public class StartRaidCommandTests
	{
		[TestCase(".dkp startraid 1234", true, 1234)]
		public void ParseArgs (string args, bool expectedSuccess, int expectedeventId)
		{
			//Act
			(bool success, int eventId) = target.ParseArgs(args);

			//Assert
			Assert.AreEqual(expectedSuccess, success, "Match should be successful.");
			Assert.AreEqual(expectedeventId, eventId);
		}

		#region Setup/Teardown

		[SetUp]
		public void SetUp ()
		{
			target = new StartRaidCommand(new Mock<IDkpProcessor>().Object, new DkpBotConfiguration(), new Mock<ILogger<StartRaidCommand>>().Object);
		}

		#endregion

		#region Test Helpers

		private StartRaidCommand target;

		#endregion
	}
}
