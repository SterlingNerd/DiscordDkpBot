using System;
using System.Linq;
using System.Threading.Tasks;

using AutoFixture;
using AutoFixture.AutoMoq;

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
		public void CalculateWinners_OneItem_OneBid ()
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

			Assert.AreEqual(mainBid.CharacterName, winner1.Bid.CharacterName);
			Assert.AreEqual(101, winner1.Price);

			Assert.AreEqual(boxBid1.CharacterName, winner2.Bid.CharacterName);
			Assert.AreEqual(753, winner2.Price);
		}

		[Test]
		public void CalculateWinners_TwoItems_SingleBid ()
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
		public void CalculateWinners_TwoItems_ThreeBids ()
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

			Assert.AreEqual(18, mainWinner.Price, "main should pay 18");
			Assert.AreEqual(18, altWinner.Price, "alt should pay 18");
		}
		[Test]
		public async Task AuctionNotFound ()
		{
			//Arrange

			void Act ()
			{
				target.AddOrUpdateBid("some item", "someCharacter", main.Name, 100, new Mock<IMessage>().Object).GetAwaiter().GetResult();
				//Act
			}

			//Assert
			Assert.Throws<AuctionNotFoundException>(Act);
		}


		[Test]
		public void PlaceBidOverMax ()
		{
			//Arrange
			const int characterId = 42;
			const string charName = "someCharacter";
			Auction auction = new Auction(42, 2, "some item", 2.0, new RaidInfo(), new Mock<IMessage>().Object);
			state.Auctions.TryAdd("some item", auction);
			dkpProcessor.Setup(x => x.GetDkp(characterId)).Returns(Task.FromResult(new PlayerPoints { PointsCurrentWithTwink = 90 }));
			dkpProcessor.Setup(x => x.GetCharacterId(charName)).Returns(Task.FromResult(characterId));
		
			//Act
			void Act ()
			{
				target.AddOrUpdateBid("some item", charName, main.Name, 100, message.Object).GetAwaiter().GetResult();
			}

			//Assert
			Assert.Throws<InsufficientDkpException>(Act);
		}

		[Test]
		public void PlaceSeveralBidsOverMax ()
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
			void Act ()
			{
				target.AddOrUpdateBid("3", charName, main.Name, 11, message.Object).GetAwaiter().GetResult();
			}

			//Assert
			Assert.Throws<InsufficientDkpException>(Act);
		}

		#region Setup/Teardown

		[SetUp]
		public void SetUp ()
		{
			dkpProcessor = new Mock<IDkpProcessor>();
			itemProcessor = new Mock<IItemProcessor>();
			configuration = new DkpBotConfiguration();
			configuration.Ranks = new[] { alt, box, main };

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
		private Mock<IDkpProcessor> dkpProcessor;
		private Mock<IItemProcessor> itemProcessor;
		private readonly RankConfiguration main = new RankConfiguration("main", null, null);
		private readonly AuctionState state = new AuctionState();
		private AuctionProcessor target;
		private DkpBotConfiguration configuration;
		private Mock<IMessage> message;

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
