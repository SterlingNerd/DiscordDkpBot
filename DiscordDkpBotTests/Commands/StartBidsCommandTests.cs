using System;

using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Commands;

using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

namespace DiscordDkpBotTests.Commands
{
	[TestFixture]
	public class StartBidsCommandTests
	{
		[TestCase ("\"YoMomma\"", null, "YoMomma", null)]
		[TestCase ("2x\"YoMomma\"", 2, "YoMomma", null)]
		[TestCase ("2x \"YoMomma\"", 2, "YoMomma", null)]
		[TestCase ("7x\t\"YoMomma\"", 7, "YoMomma", null)]
		[TestCase ("5x    \"YoMomma\"", 5, "YoMomma", null)]
		[TestCase ("\"YoMomma\" 4", null, "YoMomma", 4)]
		[TestCase ("\"YoMomma\"\t6", null, "YoMomma", 6)]
		[TestCase ("\"YoMomma\"16", null, "YoMomma", 16)]
		[TestCase ("12x\"YoSista\"\t69", 12, "YoSista", 69)]
		[TestCase ("42\"YoSista\"", 42, "YoSista", null)]
		[TestCase ("42 \"YoSista\"", 42, "YoSista", null)]
		public void Parse (string args, int? expectedQuantity, string expectedName, int? expectedMinutes)
		{
			//Arrange

			//Act
			(int? quantity, string name, int? minutes) = target.ParseArgs(args);

			//Assert
			Assert.AreEqual(expectedQuantity, quantity);
			Assert.AreEqual(expectedName, name);
			Assert.AreEqual(expectedMinutes, minutes);
		}

		#region Setup/Teardown

		[SetUp]
		public void SetUp ()
		{
			processor = new Mock<IAuctionProcessor>();
			log = new Mock<ILogger<StartBidsCommand>>();
			target = new StartBidsCommand(processor.Object, log.Object);
		}

		#endregion

		#region Test Helpers

		private Mock<ILogger<StartBidsCommand>> log;
		private Mock<IAuctionProcessor> processor;
		private StartBidsCommand target;

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
