using System;

using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;

using Discord;
using Discord.Rest;
using Discord.WebSocket;

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
		[TestCase(".dkp startbids \"Foo\"", ExpectedResult = true)]
		[TestCase(".dkp startbid \"Foo\"", ExpectedResult = true)]
		[TestCase(".startbids \"Foo\"", ExpectedResult = true)]
		[TestCase(".startbid \"Foo\"", ExpectedResult = true)]
		[TestCase(".dkp startbids 2x\"Foo\"", ExpectedResult = true)]
		[TestCase(".dkp startbid 2x\"Foo\"", ExpectedResult = true)]
		[TestCase(".startbids 2x\"Foo\"", ExpectedResult = true)]
		[TestCase(".startbid 2x\"Foo\"", ExpectedResult = true)]
		[TestCase(".dkp startbids \"Foo\" 5", ExpectedResult = true)]
		[TestCase(".dkp startbid \"Foo\" 5", ExpectedResult = true)]
		[TestCase(".startbids \"Foo\" 4", ExpectedResult = true)]
		[TestCase(".startbid \"Foo\"3 ", ExpectedResult = true)]
		[TestCase(".dkp startbids 5x \"Foo\" 2", ExpectedResult = true)]
		[TestCase(".dkp startbid 4 \"Foo\" 2", ExpectedResult = true)]
		[TestCase(".startbids 3 \"Foo\" 2", ExpectedResult = true)]
		[TestCase(".startbid 2 \"Foo\" 2", ExpectedResult = true)]
		[TestCase(".start 2 \"Foo\" 2", ExpectedResult = false)]
		[TestCase(".jimmy 2 \"Foo\" 2", ExpectedResult = false)]
		[TestCase("2x \"Foo\" 2", ExpectedResult = false)]
		public bool DoesCommandApply (string input)
		{
			// Arrange
			var channel = new Mock<IMessageChannel>();


			Mock<IMessage> message = new Mock<IMessage>();
			message.SetupGet(x => x.Content).Returns(input);
			message.SetupGet(x => x.Channel).Returns(channel.Object);

			// Act
			return target.DoesCommandApply(message.Object);
		}


		[TestCase("@dkp startbids \"YoMomma\"", null, "YoMomma", null)]
		[TestCase("@dkp startbids 2x\"YoMomma\"", 2, "YoMomma", null)]
		[TestCase("@dkp startbids 2x \"YoMomma\"", 2, "YoMomma", null)]
		[TestCase("@dkp startbids 7x\t\"YoMomma\"", 7, "YoMomma", null)]
		[TestCase("@dkp startbids 5x    \"YoMomma\"", 5, "YoMomma", null)]
		[TestCase("@dkp startbids \"YoMomma\" 4", null, "YoMomma", 4)]
		[TestCase("@dkp startbids \"YoMomma\"\t6", null, "YoMomma", 6)]
		[TestCase("@dkp startbids \"YoMomma\"16", null, "YoMomma", 16)]
		[TestCase("@dkp startbids 12x\"YoSista\"\t69", 12, "YoSista", 69)]
		[TestCase("@dkp startbids 42\"YoSista\"", 42, "YoSista", null)]
		[TestCase("@dkp startbids 42 \"YoSista\"", 42, "YoSista", null)]
		[TestCase("@dkp startbids 42 \"Yo Sista\"1", 42, "Yo Sista", 1)]
		[TestCase("@dkp startbids 42 \"Yo's Sista\" 1", 42, "Yo's Sista", 1)]
		[TestCase("@dkp startbids 42 \"Yo$%#5Sista\" 1", 42, "Yo$%#5Sista", 1)]
		[TestCase("@dkp startbids 42 \"Y o S i s t a!\" 1", 42, "Y o S i s t a!", 1)]
		[TestCase("@dkp startbids \"YoMomma\"", null, "YoMomma", null)]
		[TestCase("@dkp startbids 2x\"YoMomma\"", 2, "YoMomma", null)]
		[TestCase("@dkp startbids \"YoMomma\" 4", null, "YoMomma", 4)]
		[TestCase("@dkp startbid \"YoMomma\"", null, "YoMomma", null)]
		[TestCase("@dkp startbid 2x\"YoMomma\"", 2, "YoMomma", null)]
		[TestCase("@dkp startbid \"YoMomma\" 4", null, "YoMomma", 4)]
		[TestCase("@startbid \"YoMomma\"", null, "YoMomma", null)]
		[TestCase("@startbid 2x\"YoMomma\"", 2, "YoMomma", null)]
		[TestCase("@startbid \"YoMomma\" 4", null, "YoMomma", 4)]
		[TestCase("@startbids \"YoMomma\"", null, "YoMomma", null)]
		[TestCase("@startbids 2x\"YoMomma\"", 2, "YoMomma", null)]
		[TestCase("@startbids \"YoMomma\" 4", null, "YoMomma", 4)]
		public void ParseArgs (string args, int? expectedQuantity, string expectedName, int? expectedMinutes)
		{
			//Arrange
			config.CommandPrefix = "@";
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
			config = new DkpBotConfiguration();
			processor = new Mock<IAuctionProcessor>();
			log = new Mock<ILogger<StartAuctionCommand>>();
			target = new StartAuctionCommand(config, processor.Object, log.Object);
		}

		#endregion

		#region Test Helpers

		private Mock<ILogger<StartAuctionCommand>> log;
		private Mock<IAuctionProcessor> processor;
		private StartAuctionCommand target;
		private DkpBotConfiguration config;

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
