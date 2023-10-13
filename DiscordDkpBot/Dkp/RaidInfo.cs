using System;

namespace DiscordDkpBot.Dkp
{
	public class RaidInfo
	{
		public DateTime Date { get; set; }

		public int EventId { get; set; }
		public int Id { get; set; }

		public string Name { get; set; }

		public Decimal Value { get; set; }

		public RaidInfo (int id, DateTime date, int eventId, decimal value, string name)
		{
			this.Id = id;
			this.Date = date;
			this.EventId = eventId;
			this.Value = value;
			this.Name = name;
		}

		public RaidInfo () { }
	}
}
