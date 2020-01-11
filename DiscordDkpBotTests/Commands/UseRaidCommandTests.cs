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
	public class UseRaidCommandTests
	{
		[TestCase("@ UseRaid 12345", 12345)]
		[TestCase("@ useraid 42", 42)]
		public void ParseArgs(string args, int expectedRaidId)
		{
			//Act
			(bool success, int raidId) = target.ParseArgs(args);

			//Assert
			Assert.AreEqual(expectedRaidId, raidId);
		}

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			configuration = new DiscordConfiguration();
			configuration.CommandPrefix = "@";
			target = new UseRaidCommand(configuration, new Mock<IDkpProcessor>().Object, new Mock<ILogger<UseRaidCommand>>().Object);
		}

		#endregion

		#region Test Helpers

		private DiscordConfiguration configuration;
		private UseRaidCommand target;

		#endregion
	}
}
