using System;
using System.Linq;

using Discord;

using DiscordDkpBot.Auctions;
using DiscordDkpBot.Configuration;
using DiscordDkpBot.Dkp;

using Moq;

using NUnit.Framework;

namespace DiscordDkpBotTests.Auctions
{
	public static class AuctionTestingExtensions
	{
		private static readonly Random random = new Random();

		public static AuctionBid AddBid (this Auction auction, string name, int amount, RankConfiguration rank)
		{
			AuctionBid bid = new AuctionBid(auction, name, random.Next(), amount, rank, GetAuthor(42));
			auction.Bids.AddOrUpdate(bid);
			return bid;
		}

		public static void AssertWinner (this CompletedAuction completedAuction, AuctionBid winningBid, int price)
		{
			WinningBid mainWinner = completedAuction.WinningBids.SingleOrDefault(x => ReferenceEquals(winningBid, x.Bid));
			Assert.IsNotNull(mainWinner, $"{winningBid.CharacterName} should be a winner.");
			Assert.AreEqual(price, mainWinner.Price, $"{winningBid.CharacterName} should pay {price}.");
		}

		private static IUser GetAuthor (ulong id)
		{
			Mock<IUser> mock = new Mock<IUser>();
			mock.SetupGet(x => x.Id).Returns(id);
			return mock.Object;
		}

		private static IMessage GetMessage (ulong id)
		{
			Mock<IMessage> message = new Mock<IMessage>();
			message.Setup(x => x.Author).Returns(GetAuthor(id));
			return message.Object;
		}

		public static Auction NewAuction (this RaidInfo raid, int quantity)
		{
			Auction auction = new Auction(23423, quantity, "Nuke", 2, raid, GetMessage(42));
			return auction;
		}

		public static void AssertNumberOfWinners (this CompletedAuction completedAuction, int numberOfWinners)
		{
			Assert.AreEqual(numberOfWinners, completedAuction.WinningBids.Count);

		}
	}
}
