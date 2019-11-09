using System;
using System.Linq;
using System.Threading.Tasks;

using Discord;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Configuration;
using DiscordDkpBot.Dkp;
using DiscordDkpBot.Dkp.EqDkpPlus.Xml;
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
		public void AuctionNotFound()
		{
			//Arrange

			void Act()
			{
				target.AddOrUpdateBid("some item", "someCharacter", main.Name, 100, new Mock<IMessage>().Object).GetAwaiter().GetResult();
				//Act
			}

			//Assert
			Assert.Throws<AuctionNotFoundException>(Act);
		}

		[Test]
		public void CalculateWinners_OneItem_OneBid()
		{
			//Arrange
			RaidInfo raid = new RaidInfo();
			Auction auction = new Auction(23423, 1, "Nuke", 2, raid, GetMessage(42));

			AuctionBid mainBid = new AuctionBid(auction, "main", 1, 50, main, GetAuthor(42));

			auction.Bids.AddOrUpdate(mainBid);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			Assert.AreEqual(1, completedAuction.WinningBids.Count);

			WinningBid mainWinner = completedAuction.WinningBids.SingleOrDefault(x => x.Bid.CharacterName == mainBid.CharacterName);

			Assert.IsNotNull(mainWinner, "main should be a winner.");

			Assert.AreEqual(1, mainWinner.Price, "main should pay 1");
		}

		[Test]
		public void CalculateWinners_OneItem_ThreeBids ()
		{
			//Arrange
			RaidInfo raid = new RaidInfo();
			Auction auction = new Auction(23423, 1, "Nuke", 2, raid, GetMessage(42));

			AuctionBid mainBid = new AuctionBid(auction, "main", 1, 50, main, GetAuthor(42));
			AuctionBid main2Bid = new AuctionBid(auction, "main2", 2, 17, main, GetAuthor(43));
			AuctionBid altBid = new AuctionBid(auction, "alt", 3, 104, alt, GetAuthor(44));

			auction.Bids.AddOrUpdate(mainBid);
			auction.Bids.AddOrUpdate(main2Bid);
			auction.Bids.AddOrUpdate(altBid);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			Assert.AreEqual(1, completedAuction.WinningBids.Count);

			WinningBid mainWinner = completedAuction.WinningBids.SingleOrDefault(x => x.Bid.CharacterName == mainBid.CharacterName);

			Assert.IsNotNull(mainWinner, "main should be a winner.");

			Assert.AreEqual(26, mainWinner.Price, "main should pay 26");
		}
		[Test]
		public void CalculateWinners_OneItem_TieBids ()
		{
			//Arrange
			RaidInfo raid = new RaidInfo();
			Auction auction = new Auction(23423, 1, "Nuke", 2, raid, GetMessage(42));

			AuctionBid mainBid = new AuctionBid(auction, "main", 1, 10, main, GetAuthor(42));
			AuctionBid altBid = new AuctionBid(auction, "alt", 3, 10, alt, GetAuthor(44));

			auction.Bids.AddOrUpdate(mainBid);
			auction.Bids.AddOrUpdate(altBid);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			Assert.AreEqual(1, completedAuction.WinningBids.Count);

			
			Assert.AreEqual(10, completedAuction.WinningBids.First().Price, "Winner should pay 10.");
		}

		[Test]
		public void CalculateWinners_TwoItems_BigBoxBid ()
		{
			//Arrange
			RaidInfo raid = new RaidInfo();
			Auction auction = new Auction(23423, 2, "Nuke", 2, raid, GetMessage(42));

			AuctionBid mainBid = new AuctionBid(auction, "main", 1, 104, main, GetAuthor(43));
			AuctionBid boxBid1 = new AuctionBid(auction, "box1", 2, 300, box, GetAuthor(44));
			AuctionBid boxBid2 = new AuctionBid(auction, "box2", 3, 250, box, GetAuthor(45));
			AuctionBid altBid = new AuctionBid(auction, "alt", 4, 54, alt, GetAuthor(46));

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

			Assert.AreEqual(mainBid.CharacterName, winner1.Bid.CharacterName, "winner1 should be main.");
			Assert.AreEqual(101, winner1.Price, "main should pay 101");

			Assert.AreEqual(boxBid1.CharacterName, winner2.Bid.CharacterName, "Winner2 should be box1");
			Assert.AreEqual(753, winner2.Price, "Box should pay 753");
		}

		[Test]
		public void CalculateWinners_TwoItems_SecondPlaceTie ()
		{
			//Arrange
			RaidInfo raid = new RaidInfo();
			Auction auction = new Auction(23423, 2, "Nuke", 2, raid, GetMessage(42));

			AuctionBid mainBid = new AuctionBid(auction, "first", 1, 15, main, GetAuthor(43));
			AuctionBid boxBid1 = new AuctionBid(auction, "second1", 2, 5, main, GetAuthor(44));
			AuctionBid boxBid2 = new AuctionBid(auction, "second2", 3, 5, main, GetAuthor(45));
			AuctionBid altBid = new AuctionBid(auction, "alt", 4, 2, alt, GetAuthor(46));

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

			Assert.AreEqual(mainBid.CharacterName, winner1.Bid.CharacterName, "winner1 should be main.");
			Assert.AreEqual(5, winner1.Price, "main should pay 5");

			Assert.IsTrue(winner2.Bid.CharacterName.StartsWith("second"), "Winner2 should be a second");
			Assert.AreEqual(5, winner2.Price, "second should pay 5");
		}

		[Test]
		public void CalculateWinners_ThreeItems_SecondPlaceTie ()
		{
			//Arrange
			RaidInfo raid = new RaidInfo();
			Auction auction = new Auction(23423, 3, "Nuke", 2, raid, GetMessage(42));

			AuctionBid bid1 = new AuctionBid(auction, "first", 1, 67, main, GetAuthor(43));
			AuctionBid bid2 = new AuctionBid(auction, "second1", 2, 41, main, GetAuthor(44));
			AuctionBid bid3 = new AuctionBid(auction, "second2", 3, 41, main, GetAuthor(45));
			AuctionBid bid4 = new AuctionBid(auction, "alt", 4, 35, main, GetAuthor(46));

			auction.Bids.AddOrUpdate(bid4);
			auction.Bids.AddOrUpdate(bid2);
			auction.Bids.AddOrUpdate(bid1);
			auction.Bids.AddOrUpdate(bid3);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			Assert.AreEqual(3, completedAuction.WinningBids.Count);

			WinningBid winner1 = completedAuction.WinningBids[0];
			WinningBid winner2 = completedAuction.WinningBids[1];
			WinningBid winner3 = completedAuction.WinningBids[2];

			Assert.AreEqual(bid1.CharacterName, winner1.Bid.CharacterName, "winner1 should be first.");
			Assert.AreEqual(36, winner1.Price, "winner1 should pay 5");

			Assert.IsTrue(winner2.Bid.CharacterName.StartsWith("second"), "Winner2 should be a second");
			Assert.AreEqual(36, winner2.Price, "winner2 should pay 5");

			Assert.IsTrue(winner3.Bid.CharacterName.StartsWith("second"), "Winner3 should be a second");
			Assert.AreEqual(36, winner2.Price, "winner3 should pay 5");

			Assert.AreNotEqual(winner2.Bid.CharacterName, winner3.Bid.CharacterName, "2 and 3 should not be the same.");
		}
		[Test]
		public void CalculateWinners_TwoItems_TwoBidsBid ()
		{
			//Arrange
			RaidInfo raid = new RaidInfo();
			Auction auction = new Auction(23423, 2, "Nuke", 2, raid, GetMessage(42));

			AuctionBid bid1 = new AuctionBid(auction, "main", 1, 104, main, GetAuthor(43));
			AuctionBid bid2 = new AuctionBid(auction, "main2", 2, 45, main, GetAuthor(44));

			auction.Bids.AddOrUpdate(bid2);
			auction.Bids.AddOrUpdate(bid1);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			Assert.AreEqual(2, completedAuction.WinningBids.Count);

			WinningBid winner1 = completedAuction.WinningBids[0];
			WinningBid winner2 = completedAuction.WinningBids[1];

			Assert.AreEqual(bid1.CharacterName, winner1.Bid.CharacterName, "winner1 should be main.");
			Assert.AreEqual(1, winner1.Price, "Winner 1 should pay 1.");

			Assert.AreEqual(bid2.CharacterName, winner2.Bid.CharacterName, "Winner2 should be main2");
			Assert.AreEqual(1, winner2.Price, "Winner 2 should pay 1");
		}

		[Test]
		public void CalculateWinners_TwoItems_ManyBids ()
		{
			//Arrange
			RankConfiguration raider = new RankConfiguration("Raider", null, 1);
			RankConfiguration box = new RankConfiguration("Box", 50, 3);
			RankConfiguration alt = new RankConfiguration("Alt", 10, 1);
			RankConfiguration member = new RankConfiguration("Member", 10, 1);

			configuration.Ranks = new[] { raider, box, alt, member };
			RaidInfo raid = new RaidInfo();
			Auction auction = new Auction(23423, 2, "Nuke", 2, raid, GetMessage(42));

			AuctionBid galvanized = new AuctionBid(auction, "Galvanized", 1, 26, raider, GetAuthor(43));
			AuctionBid barogue = new AuctionBid(auction, "Barogue", 3, 14, alt, GetAuthor(45));
			AuctionBid autobahn = new AuctionBid(auction, "Autobahn", 2, 10, raider, GetAuthor(44));
			AuctionBid khaldraks = new AuctionBid(auction, "Khaldraks", 4, 10, member, GetAuthor(46));
			AuctionBid windforce = new AuctionBid(auction, "Windforce", 5, 5, alt, GetAuthor(47));
			AuctionBid glororhan = new AuctionBid(auction, "Glororhan", 6, 5, raider, GetAuthor(48));

			auction.Bids.AddOrUpdate(galvanized);
			auction.Bids.AddOrUpdate(autobahn);
			auction.Bids.AddOrUpdate(barogue);
			auction.Bids.AddOrUpdate(khaldraks);
			auction.Bids.AddOrUpdate(windforce);
			auction.Bids.AddOrUpdate(glororhan);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			Assert.AreEqual(2, completedAuction.WinningBids.Count);

			WinningBid winner1 = completedAuction.WinningBids[0];
			WinningBid winner2 = completedAuction.WinningBids[1];

			Assert.AreEqual(galvanized.CharacterName, winner1.Bid.CharacterName);
			Assert.AreEqual(11, winner1.Price);

			Assert.AreEqual(barogue.CharacterName, winner2.Bid.CharacterName);
			Assert.AreEqual(11, winner2.Price);
		}
		[Test]
		public void CalculateWinners_TwoItems_ManyRaiderBids ()
		{
			//Arrange
			RankConfiguration raider = new RankConfiguration("Raider", null, 1);

			configuration.Ranks = new[] { raider };
			RaidInfo raid = new RaidInfo();
			Auction auction = new Auction(23423, 2, "Nuke", 2, raid, GetMessage(42));

			AuctionBid blace = new AuctionBid(auction, "Blace", 1, 103, raider, GetAuthor(43));
			AuctionBid khovet = new AuctionBid(auction, "Khovet", 2, 75, raider, GetAuthor(44));
			AuctionBid glororhan = new AuctionBid(auction, "Glororhan", 3, 69, raider, GetAuthor(45));
			AuctionBid mowron = new AuctionBid(auction, "Mowron", 4, 69, raider, GetAuthor(46));
			AuctionBid kalvin = new AuctionBid(auction, "kalvin", 5, 67, raider, GetAuthor(47));
			AuctionBid galvanized = new AuctionBid(auction, "GALVANIZED", 6, 55, raider, GetAuthor(48));

			auction.Bids.AddOrUpdate(blace);
			auction.Bids.AddOrUpdate(khovet);
			auction.Bids.AddOrUpdate(glororhan);
			auction.Bids.AddOrUpdate(mowron);
			auction.Bids.AddOrUpdate(kalvin);
			auction.Bids.AddOrUpdate(galvanized);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			Assert.AreEqual(2, completedAuction.WinningBids.Count);

			WinningBid winner1 = completedAuction.WinningBids[0];
			WinningBid winner2 = completedAuction.WinningBids[1];

			Assert.AreEqual(blace.CharacterName, winner1.Bid.CharacterName);
			Assert.AreEqual(70, winner1.Price);

			Assert.AreEqual(khovet.CharacterName, winner2.Bid.CharacterName);
			Assert.AreEqual(70, winner2.Price);
		}

		[Test]
		public void CalculateWinners_TwoItems_SingleBid()
		{
			//Arrange
			RaidInfo raid = new RaidInfo();
			Auction auction = new Auction(23423, 2, "Nuke", 2, raid, GetMessage(42));

			AuctionBid mainBid = new AuctionBid(auction, "main", 1, 104, main, GetAuthor(42));

			auction.Bids.AddOrUpdate(mainBid);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			Assert.AreEqual(1, completedAuction.WinningBids.Count);
			WinningBid winner = completedAuction.WinningBids.Single();
			Assert.AreEqual(mainBid.CharacterName, winner.Bid.CharacterName);
			Assert.AreEqual(1, winner.Price);
		}

		[Test]
		public void CalculateWinners_TwoItems_ThreeBids()
		{
			//Arrange
			RaidInfo raid = new RaidInfo();
			Auction auction = new Auction(23423, 2, "Nuke", 2, raid, GetMessage(42));

			AuctionBid mainBid = new AuctionBid(auction, "main", 1, 50, main, GetAuthor(42));
			AuctionBid main2Bid = new AuctionBid(auction, "main2", 2, 17, main, GetAuthor(43));
			AuctionBid altBid = new AuctionBid(auction, "alt", 3, 104, alt, GetAuthor(44));

			auction.Bids.AddOrUpdate(mainBid);
			auction.Bids.AddOrUpdate(main2Bid);
			auction.Bids.AddOrUpdate(altBid);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			Assert.AreEqual(2, completedAuction.WinningBids.Count);

			WinningBid mainWinner = completedAuction.WinningBids.SingleOrDefault(x => x.Bid.CharacterName == mainBid.CharacterName);
			WinningBid altWinner = completedAuction.WinningBids.SingleOrDefault(x => x.Bid.CharacterName == altBid.CharacterName);

			Assert.IsNotNull(mainWinner, "main should be a winner.");
			Assert.IsNotNull(altWinner, "alt should be a winner.");

			Assert.AreEqual(26, mainWinner.Price, "main should pay 26 (alt max is 25)");
			Assert.AreEqual(18, altWinner.Price, "alt should pay 18 (1 more than main2)");
		}

		[Test]
		public void PlaceBidOverMax()
		{
			//Arrange
			const int characterId = 42;
			const string charName = "someCharacter";
			Auction auction = new Auction(42, 2, "some item", 2.0, new RaidInfo(), new Mock<IMessage>().Object);
			state.Auctions.TryAdd("some item", auction);
			dkpProcessor.Setup(x => x.GetDkp(characterId)).Returns(Task.FromResult(new PlayerPoints { PointsCurrentWithTwink = 90 }));
			dkpProcessor.Setup(x => x.GetCharacterId(charName)).Returns(Task.FromResult(characterId));

			//Act
			void Act()
			{
				target.AddOrUpdateBid("some item", charName, main.Name, 100, message.Object).GetAwaiter().GetResult();
			}

			//Assert
			Assert.Throws<InsufficientDkpException>(Act);
		}

		[Test]
		public void PlaceSeveralBidsOverMax()
		{
			//Arrange
			const int characterId = 42;
			const string charName = "someCharacter";
			Auction auction1 = new Auction(42, 2, "1", 2.0, new RaidInfo(), new Mock<IMessage>().Object);
			Auction auction2 = new Auction(42, 2, "2", 2.0, new RaidInfo(), new Mock<IMessage>().Object);
			Auction auction3 = new Auction(42, 2, "3", 2.0, new RaidInfo(), new Mock<IMessage>().Object);
			state.Auctions.TryAdd("1", auction1);
			state.Auctions.TryAdd("2", auction2);
			state.Auctions.TryAdd("3", auction3);

			dkpProcessor.Setup(x => x.GetDkp(characterId)).Returns(Task.FromResult(new PlayerPoints { PointsCurrentWithTwink = 90 }));
			dkpProcessor.Setup(x => x.GetCharacterId(charName)).Returns(Task.FromResult(characterId));

			//Act
			// These should succeed
			target.AddOrUpdateBid("1", charName, main.Name, 40, message.Object).GetAwaiter().GetResult();
			target.AddOrUpdateBid("2", charName, main.Name, 40, message.Object).GetAwaiter().GetResult();

			// This one should fail.
			void Act()
			{
				target.AddOrUpdateBid("3", charName, main.Name, 11, message.Object).GetAwaiter().GetResult();
			}

			//Assert
			Assert.Throws<InsufficientDkpException>(Act);
		}

		[Test]
		public void PlaceSeveralBidsOverMaxWithMultiplier()
		{
			//Arrange
			const int characterId = 42;
			const string charName = "someCharacter";
			Auction auction1 = new Auction(42, 2, "1", 2.0, new RaidInfo(), new Mock<IMessage>().Object);
			Auction auction2 = new Auction(42, 2, "2", 2.0, new RaidInfo(), new Mock<IMessage>().Object);
			Auction auction3 = new Auction(42, 2, "3", 2.0, new RaidInfo(), new Mock<IMessage>().Object);
			state.Auctions.TryAdd("1", auction1);
			state.Auctions.TryAdd("2", auction2);
			state.Auctions.TryAdd("3", auction3);

			dkpProcessor.Setup(x => x.GetDkp(characterId)).Returns(Task.FromResult(new PlayerPoints { PointsCurrentWithTwink = 90 }));
			dkpProcessor.Setup(x => x.GetCharacterId(charName)).Returns(Task.FromResult(characterId));

			//Act
			// These should succeed
			target.AddOrUpdateBid("1", charName, box.Name, 20, message.Object).GetAwaiter().GetResult();
			target.AddOrUpdateBid("2", charName, box.Name, 5, message.Object).GetAwaiter().GetResult();

			// This one should fail.
			void Act()
			{
				target.AddOrUpdateBid("3", charName, box.Name, 10, message.Object).GetAwaiter().GetResult();
			}

			//Assert
			Assert.Throws<InsufficientDkpException>(Act);
		}

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			state = new AuctionState();

			dkpProcessor = new Mock<IDkpProcessor>();
			itemProcessor = new Mock<IItemProcessor>();
			configuration = new DkpBotConfiguration { Ranks = new[] { alt, box, main } };

			Mock<IMessageChannel> channel = new Mock<IMessageChannel>();
			channel.Setup(x => x.SendMessageAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Embed>(), It.IsAny<RequestOptions>()))
				.Returns(Task.FromResult(new Mock<IUserMessage>().Object));
			message = new Mock<IMessage>();
			message.SetupGet(x => x.Channel).Returns(channel.Object);

			target = new AuctionProcessor(configuration, state, itemProcessor.Object, dkpProcessor.Object, new Mock<ILogger<AuctionProcessor>>().Object);
		}

		#endregion

		#region Test Helpers

		private readonly RankConfiguration alt = new RankConfiguration("alt", 25, null);
		private readonly RankConfiguration box = new RankConfiguration("box", 100, 3);
		private DkpBotConfiguration configuration;
		private Mock<IDkpProcessor> dkpProcessor;
		private Mock<IItemProcessor> itemProcessor;
		private readonly RankConfiguration main = new RankConfiguration("main", null, null);
		private Mock<IMessage> message;
		private AuctionState state;
		private AuctionProcessor target;

		private IUser GetAuthor(ulong id)
		{
			Mock<IUser> mock = new Mock<IUser>();
			mock.SetupGet(x => x.Id).Returns(id);
			return mock.Object;
		}

		private IMessage GetMessage(ulong id)
		{
			Mock<IMessage> message = new Mock<IMessage>();
			message.Setup(x => x.Author).Returns(GetAuthor(id));
			return message.Object;
		}

		#endregion
	}
}
