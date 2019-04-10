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
	public class DkpCheckCommandTests
	{
		[TestCase(".dkp jimmy", "jimmy")]
		[TestCase("dkp jimmy", "jimmy")]
		public void ParseArgs (string args, string expectedName)
		{
			//Act
			(bool success, string name) = target.ParseArgs(args);

			//Assert
			Assert.AreEqual(expectedName, name);
		}

		#region Setup/Teardown

		[SetUp]
		public void SetUp ()
		{
			target = new DkpCheckCommand(new DkpBotConfiguration(), new Mock<IDkpProcessor>().Object, new Mock<ILogger<DkpCheckCommand>>().Object);
		}

		#endregion

		#region Test Helpers

		private DkpCheckCommand target;

		#endregion
	}
}
