using System;

using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Commands;
using DiscordDkpBot.Configuration;

using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

namespace DiscordDkpBotTests.Commands
{
	[TestFixture]
	public class StartBidsCommandTests
	{
		[TestCase ("@dkp startbids \"YoMomma\"", null, "YoMomma", null)]
		[TestCase ("@dkp startbids 2x\"YoMomma\"", 2, "YoMomma", null)]
		[TestCase ("@dkp startbids 2x \"YoMomma\"", 2, "YoMomma", null)]
		[TestCase ("@dkp startbids 7x\t\"YoMomma\"", 7, "YoMomma", null)]
		[TestCase ("@dkp startbids 5x    \"YoMomma\"", 5, "YoMomma", null)]
		[TestCase ("@dkp startbids \"YoMomma\" 4", null, "YoMomma", 4)]
		[TestCase ("@dkp startbids \"YoMomma\"\t6", null, "YoMomma", 6)]
		[TestCase ("@dkp startbids \"YoMomma\"16", null, "YoMomma", 16)]
		[TestCase ("@dkp startbids 12x\"YoSista\"\t69", 12, "YoSista", 69)]
		[TestCase ("@dkp startbids 42\"YoSista\"", 42, "YoSista", null)]
		[TestCase ("@dkp startbids 42 \"YoSista\"", 42, "YoSista", null)]
		[TestCase ("@dkp startbids 42 \"Yo Sista\"1", 42, "Yo Sista", 1)]
		[TestCase ("@dkp startbids 42 \"Yo's Sista\" 1", 42, "Yo's Sista", 1)]
		[TestCase ("@dkp startbids 42 \"Yo$%#5Sista\" 1", 42, "Yo$%#5Sista", 1)]
		[TestCase ("@dkp startbids 42 \"Y o S i s t a!\" 1", 42, "Y o S i s t a!", 1)]
		[TestCase ("@dkp startbids \"YoMomma\"", null, "YoMomma", null)]
		[TestCase ("@dkp startbids 2x\"YoMomma\"", 2, "YoMomma", null)]
		[TestCase ("@dkp startbids \"YoMomma\" 4", null, "YoMomma", 4)]
		[TestCase ("@dkp startbid \"YoMomma\"", null, "YoMomma", null)]
		[TestCase ("@dkp startbid 2x\"YoMomma\"", 2, "YoMomma", null)]
		[TestCase ("@dkp startbid \"YoMomma\" 4", null, "YoMomma", 4)]
		[TestCase ("@startbid \"YoMomma\"", null, "YoMomma", null)]
		[TestCase ("@startbid 2x\"YoMomma\"", 2, "YoMomma", null)]
		[TestCase ("@startbid \"YoMomma\" 4", null, "YoMomma", 4)]
		[TestCase ("@startbids \"YoMomma\"", null, "YoMomma", null)]
		[TestCase ("@startbids 2x\"YoMomma\"", 2, "YoMomma", null)]
		[TestCase ("@startbids \"YoMomma\" 4", null, "YoMomma", 4)]
		public void ParseArgs (string args, int? expectedQuantity, string expectedName, int? expectedMinutes)
		{
			//Arrange
			config.CommandPrefix = "@";
			//Act
			(bool success, int? quantity, string name, int? minutes) = target.ParseArgs(args);

			//Assert
			Assert.AreEqual(expectedQuantity, quantity);
			Assert.AreEqual(expectedName, name);
			Assert.AreEqual(expectedMinutes, minutes);
		}

		#region Setup/Teardown

		[SetUp]
		public void SetUp ()
		{
			config = new DkpBotConfiguration();
			processor = new Mock<IAuctionProcessor>();
			log = new Mock<ILogger<StartAuctionCommand>>();
			target = new StartAuctionCommand(config, processor.Object, log.Object);
		}

		#endregion

		#region Test Helpers

		private DkpBotConfiguration config;

		private Mock<ILogger<StartAuctionCommand>> log;
		private Mock<IAuctionProcessor> processor;
		private StartAuctionCommand target;

		#endregion
	}

	public class AutoMockAttribute : AutoDataAttribute
	{
		public AutoMockAttribute ()
			: base(() => new Fixture().Customize(new AutoMoqCustomization()))
		{
		}
	}
}
