using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using DiscordDkpBot.Dkp.EqDkpPlus.Xml;

namespace DiscordDkpBot.Dkp.EqDkpPlus
{
	public class EqDkpState
	{
		public RaidInfo CurrentRaid { get; set; }
		public ReadOnlyDictionary<string, int> PlayerIds { get; set; } = new ReadOnlyDictionary<string, int>(new Dictionary<string, int>());

		public ConcurrentDictionary<int, RaidInfo> Raids { get; set; } = new ConcurrentDictionary<int, RaidInfo>();
	}
}
