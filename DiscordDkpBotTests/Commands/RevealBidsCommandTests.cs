using System;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Commands;
using DiscordDkpBot.Configuration;

using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

namespace DiscordDkpBotTests.Commands
{
	[TestFixture]
	public class RevealBidsCommandTests
	{
		[TestCase (".dkp reveal 123456", true, 123456)]
		public void ParseArgsTest (string input, bool expectedSuccess, int expectedId)
		{
			//Arrange

			//Act
			(bool success, int auctionId) = target.ParseArgs(input);

			//Assert
			Assert.AreEqual(expectedSuccess, success);
			Assert.AreEqual(expectedId, auctionId);
		}

		#region Setup/Teardown

		[SetUp]
		public void SetUp ()
		{
			configuration = new DiscordConfiguration();
			configuration.CommandPrefix = ".dkp ";
			state = new AuctionState();
			target = new RevealBidsCommand(configuration, state, new Mock<ILogger<RevealBidsCommand>>().Object);
		}

		#endregion

		#region Test Helpers

		private DiscordConfiguration configuration;
		private AuctionState state;

		private RevealBidsCommand target;

		#endregion
	}
}
