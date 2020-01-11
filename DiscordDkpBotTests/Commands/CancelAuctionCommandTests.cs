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
	public class CancelAuctionCommandTests
	{
		[TestCase("@ Cancelbids \"YoMomma\"", "YoMomma")]
		[TestCase("@ cancelbids \"YoMomma\"", "YoMomma")]
		[TestCase("@ Cancel \"YoMomma\"", "YoMomma")]
		[TestCase("@ cancel \"YoMomma\"", "YoMomma")]
		[TestCase("@ Cancelbid \"YoMomma\"", "YoMomma")]
		[TestCase("@ cancelbid \"YoMomma\"", "YoMomma")]
		[TestCase("@ cAnCel \"YoMomma\"", "YoMomma")]
		[TestCase("@ cancel \"Momma\"", "Momma")]
		[TestCase("@ cancel \"Foo\"", "Foo")]
		[TestCase("@ cancel \"Some Item With Spaces\"", "Some Item With Spaces")]
		[TestCase("@ cancel \"Hyphenated-Loots\"", "Hyphenated-Loots")]
		public void ParseArgs(string args, string expectedName)
		{
			//Arrange
			config.CommandPrefix = "@";
			target = new CancelAuctionCommand(config, processor.Object, log.Object);

			//Act
			(bool success,  string name) = target.ParseArgs(args);

			//Assert
			Assert.AreEqual(expectedName, name);
		}

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			config = new DiscordConfiguration();
			processor = new Mock<IAuctionProcessor>();
			log = new Mock<ILogger<CancelAuctionCommand>>();
		}

		#endregion

		#region Test Helpers

		private DiscordConfiguration config;

		private Mock<ILogger<CancelAuctionCommand>> log;
		private Mock<IAuctionProcessor> processor;
		private CancelAuctionCommand target;

		#endregion
	}
}
