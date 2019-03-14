﻿using System;
using System.Collections.Generic;
using System.Linq;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Configuration;

using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

namespace DiscordDkpBotTests.Auctions
{
	[TestFixture]
	public class AuctionBidTests
	{
		[TestCase(200, null, 250, null, 2)]
		[TestCase(200, null, 200, null, 0)]
		[TestCase(200, null, 250, 100, 1)]
		[TestCase(200, 25, 250, 100, 2)]
		[TestCase(200, 100, 250, 100, 2)]
		[TestCase(300, 25, 250, 25, 1)]
		[TestCase(300, null, null, null, 1)]
		[TestCase(300, 25, null, null, 1)]
		public void Compare (int? bid1, int? cap1, int? bid2, int? cap2, int expected)
		{
			//Arrange
			AuctionBid a1 = bid1 != null ? new AuctionBid("1", bid1.Value, cap1 ?? int.MaxValue, 1, null, null) : null;
			AuctionBid a2 = bid2 != null ? new AuctionBid("2", bid2.Value, cap2 ?? int.MaxValue, 1, null, null) : null;

			//Act
			int comparison = a1?.CompareTo(a2) ?? 0;
			int actual = comparison > 0 ? 1 : comparison < 0 ? 2 : 0;

			//Assert
			Assert.AreEqual(expected, actual);
		}

		#region Setup/Teardown

		[SetUp]
		public void SetUp ()
		{
			target = new AuctionProcessor(new DkpBotConfiguration(), new AuctionState(), new Mock<ILogger<AuctionProcessor>>().Object);
		}

		#endregion

		#region Test Helpers

		private AuctionProcessor target;

		#endregion
	}
}
