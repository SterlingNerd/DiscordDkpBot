using System;
using System.Threading.Tasks;

using Discord;

namespace DiscordDkpBot.Auctions
{
	public interface IAuctionProcessor
	{
		Task<AuctionBid> AddOrUpdateBid (string item, string character, string rank, int bid, IMessage message);
		Task<Auction> CancelAuction (string name, IMessage message);
		Task<AuctionBid> CancelBid (string item, IMessage message);
		Task<Auction> StartAuction (int? quantity, string name, int? minutes, IMessage messageChannel);
	}
}
