using System;
using System.Linq;

using Discord;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Configuration;
using DiscordDkpBot.Items;

using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

namespace DiscordDkpBotTests.Auctions
{
	[TestFixture]
	public class AuctionProcessorTests
	{
		[Test]
		public void CalculateWinners_TwoItems_BigBoxBid ()
		{
			//Arrange
			Auction auction = new Auction(23423, 2, "Nuke", 2, GetMessage(42));

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

			Assert.AreEqual(mainBid.Character, winner1.Bid.Character);
			Assert.AreEqual(101, winner1.Price);

			Assert.AreEqual(boxBid1.Character, winner2.Bid.Character);
			Assert.AreEqual(753, winner2.Price);
		}

		[Test]
		public void CalculateWinners_TwoItems_SingleBid ()
		{
			//Arrange
			Auction auction = new Auction(23423, 2, "Nuke", 2, GetMessage(42));

			AuctionBid mainBid = new AuctionBid(auction, "main", 104, main, GetAuthor(42));

			auction.Bids.AddOrUpdate(mainBid);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			Assert.AreEqual(1, completedAuction.WinningBids.Count);
			WinningBid winner = completedAuction.WinningBids.Single();
			Assert.AreEqual(mainBid.Character, winner.Bid.Character);
			Assert.AreEqual(1, winner.Price);
		}

		[Test]
		public void CalculateWinners_TwoItems_ThreeBids ()
		{
			//Arrange
			Auction auction = new Auction(23423, 2, "Nuke", 2, GetMessage(42));

			AuctionBid mainBid = new AuctionBid(auction, "main", 50, main, GetAuthor(42));
			AuctionBid main2Bid = new AuctionBid(auction, "main2", 17, main, GetAuthor(43));
			AuctionBid altBid = new AuctionBid(auction, "alt", 104, alt, GetAuthor(44));

			auction.Bids.AddOrUpdate(mainBid);
			auction.Bids.AddOrUpdate(main2Bid);
			auction.Bids.AddOrUpdate(altBid);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			Assert.AreEqual(2, completedAuction.WinningBids.Count);

			WinningBid mainWinner = completedAuction.WinningBids.SingleOrDefault(x => x.Bid.Character == mainBid.Character);
			WinningBid altWinner = completedAuction.WinningBids.SingleOrDefault(x => x.Bid.Character == altBid.Character);

			Assert.IsNotNull(mainWinner, "main should be a winner.");
			Assert.IsNotNull(altWinner, "alt should be a winner.");

			Assert.AreEqual(18, mainWinner.Price, "main should pay 18");
			Assert.AreEqual(18, altWinner.Price, "alt should pay 18");
		}

		[Test]
		public void CalculateWinners_OneItem_ThreeBids ()
		{
			//Arrange
			Auction auction = new Auction(23423, 1, "Nuke", 2, GetMessage(42));

			AuctionBid mainBid = new AuctionBid(auction, "main", 50, main, GetAuthor(42));
			AuctionBid main2Bid = new AuctionBid(auction, "main2", 17, main, GetAuthor(43));
			AuctionBid altBid = new AuctionBid(auction, "alt", 104, alt, GetAuthor(44));

			auction.Bids.AddOrUpdate(mainBid);
			auction.Bids.AddOrUpdate(main2Bid);
			auction.Bids.AddOrUpdate(altBid);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			Assert.AreEqual(1, completedAuction.WinningBids.Count);

			WinningBid mainWinner = completedAuction.WinningBids.SingleOrDefault(x => x.Bid.Character == mainBid.Character);

			Assert.IsNotNull(mainWinner, "main should be a winner.");

			Assert.AreEqual(26, mainWinner.Price, "main should pay 26");
		}

		[Test]
		public void CalculateWinners_OneItem_OneBid ()
		{
			//Arrange
			Auction auction = new Auction(23423, 1, "Nuke", 2, GetMessage(42));

			AuctionBid mainBid = new AuctionBid(auction, "main", 50, main, GetAuthor(42));

			auction.Bids.AddOrUpdate(mainBid);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			Assert.AreEqual(1, completedAuction.WinningBids.Count);

			WinningBid mainWinner = completedAuction.WinningBids.SingleOrDefault(x => x.Bid.Character == mainBid.Character);

			Assert.IsNotNull(mainWinner, "main should be a winner.");

			Assert.AreEqual(1, mainWinner.Price, "main should pay 1");
		}

		#region Setup/Teardown

		[SetUp]
		public void SetUp ()
		{
			target = new AuctionProcessor(new DkpBotConfiguration(), new AuctionState(), new Mock<IItemProcessor>().Object, new Mock<ILogger<AuctionProcessor>>().Object);
		}

		#endregion

		#region Test Helpers

		private readonly RankConfiguration alt = new RankConfiguration("alt", 25, null);
		private readonly RankConfiguration box = new RankConfiguration("box", 100, 3);
		private readonly RankConfiguration main = new RankConfiguration("main", null, null);

		private AuctionProcessor target;

		private IUser GetAuthor (ulong id)
		{
			Mock<IUser> mock = new Mock<IUser>();
			mock.SetupGet(x => x.Id).Returns(id);
			return mock.Object;
		}

		private IMessage GetMessage (ulong id)
		{
			Mock<IMessage> message = new Mock<IMessage>();
			message.Setup(x => x.Author).Returns(GetAuthor(id));
			return message.Object;
		}

		#endregion
	}
}
