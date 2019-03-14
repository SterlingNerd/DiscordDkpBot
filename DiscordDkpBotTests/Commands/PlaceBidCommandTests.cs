using System;
using System.Collections.Generic;
using System.Text;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Commands;
using DiscordDkpBot.Configuration;

using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

namespace DiscordDkpBotTests.Commands
{
	[TestFixture]
	public class PlaceBidCommandTests
	{
		[TestCase("\"Item\" Billy 51 Main", "Item", "Billy", "Main", 51, TestName = "Item Character Bid Rank")]
		[TestCase("\"Item\" Billy Main 51", "Item", "Billy", "Main", 51, TestName="Item Character Rank Bid")]
		[TestCase("\"Item\" Billy 51 Main         ", "Item", "Billy", "Main", 51, TestName="Trailing Spaces")]
		[TestCase("         \"Item\" Billy 51 Main", "Item", "Billy", "Main", 51, TestName="Leading Spaces")]
		[TestCase("\t\"Item\"\tBilly\t51\tMain", "Item", "Billy", "Main", 51, TestName="Tabs")]
		[TestCase("\"Item\" George 42 Box", "Item", "George", "Box", 42,TestName ="Box")]
		[TestCase("\"Item\" Biff 99 Unicorn", "Item", "Biff", "Unicorn", 99,TestName="Unicorn")]
		[TestCase("\"Item\" Billy 1 Main", "Item", "Billy", "Main", 1,TestName="1 Digit Bid")]
		[TestCase("\"Item\" Billy 351 Main", "Item", "Billy", "Main", 351, TestName = "3 Digit Bid")]
		[TestCase("\"Item\" Billy 1337 Main", "Item", "Billy", "Main", 1337, TestName = "4 Digit Bid")]
		public void ParseArgs (string input, string expectedItem, string expectedCharacter, string expectedRank, int expectedBid)
		{
			//Act
			(string item, string character, string rank, int bid) = target.ParseArgs(input);

			//Assert
			Assert.AreEqual(expectedItem, item);
			Assert.AreEqual(expectedCharacter, character);
			Assert.AreEqual(expectedRank, rank);
			Assert.AreEqual(expectedBid, bid);
		}

		#region Setup/Teardown

		[SetUp]
		public void SetUp ()
		{
			processor = new Mock<IAuctionProcessor>();
			log = new Mock<ILogger<PlaceBidCommand>>();
			target = new PlaceBidCommand(new DkpBotConfiguration(), processor.Object, log.Object);
		}

		#endregion

		#region Test Helpers

		private Mock<ILogger<PlaceBidCommand>> log;
		private Mock<IAuctionProcessor> processor;
		private PlaceBidCommand target;

		#endregion

	}
}
