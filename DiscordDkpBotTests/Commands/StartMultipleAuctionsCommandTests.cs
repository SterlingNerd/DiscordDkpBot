using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Discord;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Commands;
using DiscordDkpBot.Configuration;

using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

using Match = System.Text.RegularExpressions.Match;

namespace DiscordDkpBotTests.Commands
{
	[TestFixture]
	public class StartMultipleAuctionCommandTests
	{
		[TestCase("@startmany:\nFoo1\nFoo2", true, "1xFoo1|1xFoo2")]
		[TestCase("@startmultiple:\nFoo1\nFoo2", true, "1xFoo1|1xFoo2")]
		[TestCase("@startmany:\nFoo1\n3xFoo2", true, "1xFoo1|3xFoo2")]
		[TestCase("@startmany:\nFoo1\n3x Foo2", true, "1xFoo1|3xFoo2")]
		[TestCase("@startmany:\nFoo, the Barer of Baz\nFoo2", true, "1xFoo, the Barer of Baz|1xFoo2")]
		[TestCase("@startmany:Foo1\nFoo2", true, "1xFoo1|1xFoo2")]
		[TestCase("@startmany:\n  3xFoo1\nFoo2", true, "3xFoo1|1xFoo2")]
		[TestCase("@startmany:\n  Foo1\n  3x Foo2", true, "1xFoo1|3xFoo2")]
		[TestCase("@startmany:\nFoo1\t\t\n\t3x\tFoo2", true, "1xFoo1|3xFoo2")]
		[TestCase("@startmany:\nQenni's Mom\n2x Caub's Balls\nLivin's Life", true, "1xQenni's Mom|2xCaub's Balls|1xLivin's Life")]
		[TestCase("@kjhaegr asdawef", false, "")]
		public void ParseArgs(string args, bool expectedSuccess, string expectedString)
		{
			//Arrange
			Dictionary<string, int> expectedItems = ParseExpected(expectedString);

			//Act
			(bool success, Dictionary<string, int> items) = target.ParseArgs(args);

			//Assert
			success.Should().Be(expectedSuccess);
			items.Should().BeEquivalentTo(expectedItems);
		}

		[Test]
		public async Task StartsMultipleAuctions()
		{
			//Arrange
			string input = "@startmany:Foo1\nFoo2\n2xFoo3";
			Mock<IMessage> message = new Mock<IMessage>();
			Mock<IMessageChannel> channel = new Mock<IMessageChannel>();
			channel.SetupGet(x => x.Name).Returns(config.SilentAuctionsChannelName);
			message.SetupGet(x => x.Channel).Returns(channel.Object);
			message.SetupGet(x => x.Content).Returns(input);

			//Act
			await target.TryInvokeAsync(message.Object);

			//Assert
			processor.Verify(x => x.StartAuction(1, "Foo1", null, message.Object), Times.Once);
			processor.Verify(x => x.StartAuction(1, "Foo2", null, message.Object), Times.Once);
			processor.Verify(x => x.StartAuction(2, "Foo3", null, message.Object), Times.Once);
		}

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			config = new DiscordConfiguration();
			config.CommandPrefix = "@";
			processor = new Mock<IAuctionProcessor>();
			log = new Mock<ILogger<StartMultipleAuctionsCommand>>();
			target = new StartMultipleAuctionsCommand(config, processor.Object, log.Object);
		}

		#endregion

		#region Test Helpers

		private DiscordConfiguration config;
		private static readonly Regex expectedItemFormat = new Regex(@"(?<quantity>\d+)x(?<name>.*)");

		private Mock<ILogger<StartMultipleAuctionsCommand>> log;
		private Mock<IAuctionProcessor> processor;
		private StartMultipleAuctionsCommand target;

		private Dictionary<string, int> ParseExpected(string expected)
		{
			Dictionary<string, int> expectedItems = new Dictionary<string, int>();
			string[] itemStrings = expected.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

			foreach (string itemString in itemStrings)
			{
				Match match = expectedItemFormat.Match(itemString);

				if (match.Success)
				{
					expectedItems.Add(match.Groups["name"].Value, int.Parse(match.Groups["quantity"].Value));
				}
				else
				{
					throw new InvalidOperationException("Could not parse expected items!");
				}
			}

			return expectedItems;
		}

		#endregion
	}
}
