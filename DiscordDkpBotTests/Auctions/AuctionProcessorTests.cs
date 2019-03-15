using System;
using System.Linq;

using Discord;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Configuration;

using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

namespace DiscordDkpBotTests.Auctions
{
	[TestFixture]
	public class AuctionProcessorTests
	{
		[Test]
		public void CalculateWinners_TwoItems_BidCapSpread ()
		{
			//Arrange
			Auction auction = new Auction(23423, 2, "Nuke", 2, null);
			RankConfiguration main = new RankConfiguration("main", null, null);
			RankConfiguration box = new RankConfiguration("box", 100, 3);
			RankConfiguration alt = new RankConfiguration("alt", 25, null);

			AuctionBid mainBid = new AuctionBid(auction, "main", 104, main, GetAuthor(43));
			AuctionBid boxBid1 = new AuctionBid(auction, "box1", 300, box, GetAuthor(44));
			AuctionBid boxBid2 = new AuctionBid(auction, "box2", 250, box, GetAuthor(45));
			AuctionBid altBid = new AuctionBid(auction, "alt", 54, alt, GetAuthor(46));

			auction.Bids.AddOrUpdate(altBid);
			auction.Bids.AddOrUpdate(boxBid1);
			auction.Bids.AddOrUpdate(mainBid);
			auction.Bids.AddOrUpdate(boxBid2);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			Assert.AreEqual(2, completedAuction.WinningBids.Count);

			WinningBid winner1 = completedAuction.WinningBids[0];
			WinningBid winner2 = completedAuction.WinningBids[1];

			Assert.AreEqual(mainBid, winner1.Bid);
			Assert.AreEqual(101, winner1.Price);

			Assert.AreEqual(boxBid1, winner2.Bid);
			Assert.AreEqual(753, winner2.Price);
		}

		[Test]
		public void CalculateWinners_TwoItems_SingleBid ()
		{
			//Arrange
			Auction auction = new Auction(23423, 2, "Nuke", 2, GetAuthor(42));
			RankConfiguration main = new RankConfiguration("main", null, null);

			AuctionBid mainBid = new AuctionBid(auction, "main", 104, main, GetAuthor(42));

			auction.Bids.AddOrUpdate(mainBid);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			Assert.AreEqual(1, completedAuction.WinningBids.Count);
			WinningBid winner = completedAuction.WinningBids.Single();
			Assert.AreEqual(mainBid, winner.Bid);
			Assert.AreEqual(1, winner.Price);
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

		private IUser GetAuthor (ulong id)
		{
			Mock<IUser> mock = new Mock<IUser>();
			mock.SetupGet(x => x.Id).Returns(id);
			return mock.Object;
		}

		#endregion
	}
}
