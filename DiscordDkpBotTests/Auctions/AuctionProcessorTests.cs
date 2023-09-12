using System;
using System.Linq;
using System.Threading.Tasks;

using Discord;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Configuration;
using DiscordDkpBot.Dkp;
using DiscordDkpBot.Dkp.EqDkpPlus.Xml;
using DiscordDkpBot.Items;

using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

namespace DiscordDkpBotTests.Auctions
{
	[TestFixture]
	public class AuctionProcessorTests
	{

		[Test]
		public void UpdateBid_CloseToCap()
		{
			//Arrange
			Auction auction = raid.NewAuction(1);
			state.Auctions.TryAdd(auction.Name, auction);

			dkpProcessor.Setup(x => x.GetDkp(It.IsAny<int>())).Returns(Task.FromResult(new PlayerPoints { PointsCurrentWithTwink = 55 }));

			try
			{
				target.AddOrUpdateBid(auction.Name, "Jimmy", "Main", 50, new Mock<IMessage>().Object).GetAwaiter().GetResult();
			}
			catch(NullReferenceException ex)
			{
				//Don't care it'll bomb on the mock.''
			}

			//Act
			Action act = () =>
						{
							target.AddOrUpdateBid(auction.Name, "Jimmy", "Main", 55, new Mock<IMessage>().Object).GetAwaiter().GetResult();
						};

			//Assert
			act.Should().NotThrow<InsufficientDkpException>();
		}


		[Test]
		public void UpdateBid_MultipleAuctions_CloseToCap()
		{
			//Arrange
			Auction auction = new Auction(23423, 1, "Nuke", 2, raid, new Mock<IMessage>().Object);
			Auction auction2 = new Auction(23424, 1, "Horseshoe", 2, raid, new Mock<IMessage>().Object);
			state.Auctions.TryAdd(auction.Name, auction);
			state.Auctions.TryAdd(auction2.Name, auction2);

			dkpProcessor.Setup(x => x.GetDkp(It.IsAny<int>())).Returns(Task.FromResult(new PlayerPoints { PointsCurrentWithTwink = 55 }));

			try
			{
				target.AddOrUpdateBid(auction.Name, "Jimmy", "Main", 20, new Mock<IMessage>().Object).GetAwaiter().GetResult();
			}
			catch (NullReferenceException ex)
			{
				//Don't care it'll bomb on the mock.''
			}
			try
			{
				target.AddOrUpdateBid(auction.Name, "Jimmy", "Main", 30, new Mock<IMessage>().Object).GetAwaiter().GetResult();
			}
			catch (NullReferenceException ex)
			{
				//Don't care it'll bomb on the mock.''
			}

			//Act
			Action act = () =>
						{
							target.AddOrUpdateBid(auction.Name, "Jimmy", "Main", 35, new Mock<IMessage>().Object).GetAwaiter().GetResult();
						};

			//Assert
			act.Should().NotThrow<InsufficientDkpException>();
		}


		[Test]
		public void UpdateBid_MultipleAuctions_OverToCap()
		{
			//Arrange
			Auction auction = new Auction(23423, 1, "Nuke", 2, raid, new Mock<IMessage>().Object);
			Auction auction2 = new Auction(23424, 1, "Horseshoe", 2, raid, new Mock<IMessage>().Object);
			state.Auctions.TryAdd(auction.Name, auction);
			state.Auctions.TryAdd(auction2.Name, auction2);

			dkpProcessor.Setup(x => x.GetDkp(It.IsAny<int>())).Returns(Task.FromResult(new PlayerPoints { PointsCurrentWithTwink = 55 }));

			try
			{
				target.AddOrUpdateBid(auction.Name, "Jimmy", "Main", 20, new Mock<IMessage>().Object).GetAwaiter().GetResult();
			}
			catch (NullReferenceException ex)
			{
				//Don't care it'll bomb on the mock.''
			}
			try
			{
				target.AddOrUpdateBid(auction2.Name, "Jimmy", "Main", 30, new Mock<IMessage>().Object).GetAwaiter().GetResult();
			}
			catch (NullReferenceException ex)
			{
				//Don't care it'll bomb on the mock.''
			}

			//Act
			Action act = () =>
						{
							target.AddOrUpdateBid(auction2.Name, "Jimmy", "Main", 36, new Mock<IMessage>().Object).GetAwaiter().GetResult();
						};

			//Assert
			act.Should().Throw<InsufficientDkpException>();
		}

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
			Auction auction = raid.NewAuction(1);
			AuctionBid mainBid = auction.AddBid("main", 50, main);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			completedAuction.AssertNumberOfWinners(1);
			completedAuction.AssertWinner(mainBid, 1);
		}

		[Test]
		public void CalculateWinners_TwoItems_BoxesOverMain()
		{
			//Arrange
			Auction auction = raid.NewAuction(2);
			AuctionBid mainBid = auction.AddBid("main", 35, main);
			AuctionBid boxBid1 = auction.AddBid("box1", 51, box);
			AuctionBid boxBid2 = auction.AddBid("box2", 80, box);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			completedAuction.AssertNumberOfWinners(2);
			completedAuction.AssertWinner(boxBid1, 108);
			completedAuction.AssertWinner(boxBid2, 108);
		}

		[Test]
		public void CalculateWinners_OneItem_ThreeBids()
		{
			//Arrange

			Auction auction = raid.NewAuction(1);

			AuctionBid mainBid = auction.AddBid("main", 50, main);
			auction.AddBid("main2", 17, main);
			auction.AddBid("alt", 104, alt);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			completedAuction.AssertNumberOfWinners(1);
			completedAuction.AssertWinner(mainBid, 26);
		}

		[Test]
		public void CalculateWinners_OneItem_ThreeBidsOutOfOrder()
		{
			//Arrange

			Auction auction = raid.NewAuction(1);

			auction.AddBid("main2", 17, main);
			auction.AddBid("alt", 104, alt);
			AuctionBid mainBid = auction.AddBid("main", 50, main);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			completedAuction.AssertNumberOfWinners(1);
			completedAuction.AssertWinner(mainBid, 26);
		}

		[Test]
		public void CalculateWinners_OneItem_TieBids()
		{
			//Arrange

			Auction auction = raid.NewAuction(1);
			auction.AddBid("main", 10, main);
			auction.AddBid("alt", 10, alt);


			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			completedAuction.AssertNumberOfWinners(1);
			Assert.AreEqual(10, completedAuction.WinningBids.First().Price, "Winner should pay 10.");
		}

		[Test]
		public void CalculateWinners_OneItem_TieBidsOverCap()
		{
			//Arrange

			Auction auction = raid.NewAuction(1);

			AuctionBid mainBid = auction.AddBid("main", 126, main);
			auction.AddBid("alt", 126, alt);


			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			completedAuction.AssertNumberOfWinners(1);
			completedAuction.AssertWinner(mainBid, 26);
		}

		[Test]
		public void CalculateWinners_ThreeItem_Boxes()
		{
			//Arrange
			Auction auction = raid.NewAuction(3);
			AuctionBid main1Bid = auction.AddBid("main1", 93, main);
			AuctionBid box1Bid = auction.AddBid("box1", 101, box);
			AuctionBid box2Bid = auction.AddBid("box2", 60, box);
			auction.AddBid("main2", 30, main);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);
			//Assert
			completedAuction.AssertNumberOfWinners(3);
			completedAuction.AssertWinner(main1Bid, 31);
			completedAuction.AssertWinner(box1Bid, 93);
			completedAuction.AssertWinner(box2Bid, 93);
		}

		[Test]
		public void CalculateWinners_ThreeItems_SecondPlaceTie()
		{
			//Arrange

			Auction auction = raid.NewAuction(3);

			AuctionBid bid1 = auction.AddBid("first", 67, main);
			AuctionBid bid2 = auction.AddBid("second1", 41, main);
			AuctionBid bid3 = auction.AddBid("second2", 41, main);
			auction.AddBid("alt", 35, alt);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			completedAuction.AssertNumberOfWinners(3);
			completedAuction.AssertWinner(bid1, 26);
			completedAuction.AssertWinner(bid2, 26);
			completedAuction.AssertWinner(bid3, 26);
		}

		[Test]
		public void CalculateWinners_TwoItems_BigBoxBid()
		{
			//Arrange

			Auction auction = raid.NewAuction(2);

			AuctionBid mainBid = auction.AddBid("main", 104, main);
			AuctionBid boxBid1 = auction.AddBid("box1", 300, box);
			auction.AddBid("box2", 250, box);
			auction.AddBid("alt", 54, alt);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			completedAuction.AssertNumberOfWinners(2);
			completedAuction.AssertWinner(mainBid, 101);
			completedAuction.AssertWinner(boxBid1, 753);
		}

		[Test]
		public void CalculateWinners_TwoItemsAndCustomRanks_ManyBids()
		{
			//Arrange
			RankConfiguration raiderConfig = new RankConfiguration("Raider", null, 1);
			RankConfiguration boxConfig = new RankConfiguration("Box", 50, 3);
			RankConfiguration altConfig = new RankConfiguration("Alt", 10, 1);
			RankConfiguration memberConfig = new RankConfiguration("Member", 10, 1);

			configuration.Ranks = new[] { raiderConfig, boxConfig, altConfig, memberConfig };

			Auction auction = raid.NewAuction(2);

			AuctionBid bid1 = auction.AddBid("Galvanized", 26, raiderConfig);
			AuctionBid bid2 = auction.AddBid("Barogue", 14, altConfig);
			auction.AddBid("Autobahn", 10, raiderConfig);
			auction.AddBid("Khaldraks", 10, memberConfig);
			auction.AddBid("Windforce", 5, altConfig);
			auction.AddBid("Glororhan", 5, raiderConfig);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			completedAuction.AssertNumberOfWinners(2);
			completedAuction.AssertWinner(bid1, 11);
			completedAuction.AssertWinner(bid2, 11);
		}

		[Test]
		public void CalculateWinners_TwoItems_ManyRaiderBids()
		{
			//Arrange
			RankConfiguration raider = new RankConfiguration("Raider", null, 1);

			configuration.Ranks = new[] { raider };

			Auction auction = raid.NewAuction(2);

			AuctionBid bid1 = auction.AddBid("Blace", 103, raider);
			AuctionBid bid2 = auction.AddBid("Khovet", 75, raider);
			auction.AddBid("Glororhan", 69, raider);
			auction.AddBid("Mowron", 69, raider);
			auction.AddBid("kalvin", 67, raider);
			auction.AddBid("GALVANIZED", 55, raider);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			completedAuction.AssertNumberOfWinners(2);
			completedAuction.AssertWinner(bid1, 70);
			completedAuction.AssertWinner(bid2, 70);
		}

		[Test]
		public void CalculateWinners_TwoItems_SecondPlaceTie()
		{
			//Arrange

			Auction auction = raid.NewAuction(2);

			AuctionBid mainBid = auction.AddBid("first", 15, main);
			auction.AddBid("second1", 5, main);
			auction.AddBid("second2", 5, main);
			auction.AddBid("alt", 2, alt);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			completedAuction.AssertNumberOfWinners(2);
			completedAuction.AssertWinner(mainBid, 5);

			WinningBid winner2 = completedAuction.WinningBids[1];
			Assert.IsTrue(winner2.Bid.CharacterName.StartsWith("second"), "Winner2 should be a second");
			Assert.AreEqual(5, winner2.Price, "second should pay 5");
		}

		[Test]
		public void CalculateWinners_TwoItems_SingleBid()
		{
			//Arrange

			Auction auction = raid.NewAuction(2);

			AuctionBid mainBid = auction.AddBid("main", 104, main);

			auction.Bids.AddOrUpdate(mainBid);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			completedAuction.AssertNumberOfWinners(1);
			completedAuction.AssertWinner(mainBid, 1);
		}

		[Test]
		public void CalculateWinners_TwoItems_ThreeBids()
		{
			//Arrange

			Auction auction = raid.NewAuction(2);

			AuctionBid mainBid = auction.AddBid("main", 50, main);
			auction.AddBid("main2", 17, main);
			AuctionBid altBid = auction.AddBid("alt", 104, alt);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			completedAuction.AssertNumberOfWinners(2);
			completedAuction.AssertWinner(mainBid, 18);
			completedAuction.AssertWinner(altBid, 18);
		}

		[Test]
		public void CalculateWinners_TwoItems_TwoBidsBid()
		{
			//Arrange

			Auction auction = raid.NewAuction(2);

			AuctionBid bid1 = auction.AddBid("main", 104, main);
			AuctionBid bid2 = auction.AddBid("main2", 45, main);

			//Act
			CompletedAuction completedAuction = target.CalculateWinners(auction);

			//Assert
			completedAuction.AssertNumberOfWinners(2);
			completedAuction.AssertWinner(bid1, 1);
			completedAuction.AssertWinner(bid2, 1);

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
			raid = new RaidInfo();

			Mock<IMessageChannel> channel = new Mock<IMessageChannel>();
			channel.Setup(x => x.SendMessageAsync(
												It.IsAny<string>(), 
												It.IsAny<bool>(), 
												It.IsAny<Embed>(), 
												It.IsAny<RequestOptions>(), 
												It.IsAny<AllowedMentions>(),
												It.IsAny<MessageReference>(),
												It.IsAny<MessageComponent>(),
												It.IsAny<ISticker[]>(),
												It.IsAny<Embed[]>(),
												It.IsAny<MessageFlags>()
												))
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
		private RaidInfo raid;

		#endregion
	}
}
