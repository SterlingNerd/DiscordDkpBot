using System;
using System.Linq;

namespace DiscordDkpBot.Configuration
{
	public class RankConfiguration
	{
		private int maxBid;
		private int priceMultiplier;

		public int MaxBid
		{
			get => maxBid <= 0 ? int.MaxValue : maxBid;
			set => maxBid = value;
		}

		public string Name { get; set; }
		public string Aliases { get; set; }

		public int PriceMultiplier
		{
			get => priceMultiplier <= 0 ? 1 : priceMultiplier;
			set => priceMultiplier = value;
		}

		public RankConfiguration()
		{
		}

		public RankConfiguration(string name, int? maxBid, int? priceMultiplier, string aliases = null)
		{
			Name = name;
			MaxBid = maxBid ?? int.MaxValue;
			PriceMultiplier = priceMultiplier ?? 1;
			Aliases = aliases;
		}

		public override string ToString()
		{
			if (Aliases != null)
			{
				return $"{Name}/{string.Join("/", Aliases.Split(',').Select(x=>x.Trim()))}";
			}
			else
			{
				return Name;
			}
		}
	}
}
