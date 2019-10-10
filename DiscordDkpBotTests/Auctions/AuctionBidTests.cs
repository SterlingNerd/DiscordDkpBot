using System;
using System.Collections.Generic;
using System.Linq;

using Discord;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Configuration;
using DiscordDkpBot.Dkp.EqDkpPlus.Xml;

using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

namespace DiscordDkpBotTests.Auctions
{
	[TestFixture]
	public class AuctionBidTests
	{
		[TestCase(200, null, 250, null, ExpectedWinner.Second)]
		[TestCase(200, null, 200, null, ExpectedWinner.Tie)]
		[TestCase(200, null, 250, 100, ExpectedWinner.First)]
		[TestCase(200, 25, 250, 100, ExpectedWinner.Second)]
		[TestCase(200, 100, 250, 100, ExpectedWinner.Second)]
		[TestCase(300, 25, 250, 25, ExpectedWinner.First)]
		[TestCase(300, null, null, null, ExpectedWinner.First)]
		[TestCase(300, 25, null, null, ExpectedWinner.First)]
		[TestCase(104, null, 54, 25, ExpectedWinner.First)]
		[TestCase(54, 25, 300, 100, ExpectedWinner.Second)]
		[TestCase(14, 10, 10, null, ExpectedWinner.First)]
		[TestCase(10, null, 14, 10, ExpectedWinner.Second)]
		[TestCase(104, null, 250, 100, ExpectedWinner.First)]
		[TestCase(250, 100, 104, null, ExpectedWinner.Second)]
		public void Compare (int? bid1, int? cap1, int? bid2, int? cap2, ExpectedWinner expected)
		{
			//Arrange
			RaidInfo raid = new RaidInfo();
			Auction auction = new Auction(23423, 1, "Nuke", 2, raid, GetMessage(42));
			RankConfiguration rank1 = new RankConfiguration("rank1", cap1, 1);
			RankConfiguration rank2 = new RankConfiguration("rank2", cap2, 1);
			AuctionBid a1 = bid1 != null ? new AuctionBid(auction, "1", 1, bid1.Value, rank1, GetAuthor(45)) : null;
			AuctionBid a2 = bid2 != null ? new AuctionBid(auction, "2", 2, bid2.Value, rank2, GetAuthor(41)) : null;

			//Act
			int comparison = a1?.CompareTo(a2) ?? 0;
			ExpectedWinner actual = comparison < 0 ? ExpectedWinner.First : comparison > 0 ? ExpectedWinner.Second : ExpectedWinner.Tie;

			//Assert
			Assert.AreEqual(expected, actual);
		}

	public	enum ExpectedWinner
		{
			Tie = 0,
			First = 1,
			Second = 2
		}
		[Test]
		public void CompareToMultiples ()
		{
			//Arrange
			RaidInfo raid = new RaidInfo();
			Auction auction = new Auction(23423, 2, "Nuke", 2, raid, GetMessage(44));
			RankConfiguration main = new RankConfiguration("main", null, 1);
			RankConfiguration box = new RankConfiguration("box", 100, 1);
			RankConfiguration alt = new RankConfiguration("alt", 25, 1);

			AuctionBid mainBid = new AuctionBid(auction, "main", 1, 104, main, GetAuthor(1));
			AuctionBid boxBid1 = new AuctionBid(auction, "box1", 1, 300, box, GetAuthor(2));
			AuctionBid BoxBid2 = new AuctionBid(auction, "box2", 1, 250, box, GetAuthor(3));
			AuctionBid altBid = new AuctionBid(auction, "alt", 1, 54, alt, GetAuthor(4));

			List<AuctionBid> list = new List<AuctionBid> { altBid, boxBid1, mainBid, BoxBid2 };

			//Act
			list.Sort();

			//Assert
			Assert.AreEqual(mainBid, list[0]);
			Assert.AreEqual(boxBid1, list[1]);
			Assert.AreEqual(BoxBid2, list[2]);
			Assert.AreEqual(altBid, list[3]);

		}

		#region Test Helpers

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
