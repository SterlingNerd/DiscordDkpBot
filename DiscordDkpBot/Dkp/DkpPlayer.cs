using System;

namespace DiscordDkpBot.Dkp
{
	public class DkpPlayer {
		public int Id { get; set; }
		public string Name { get; set; }
		public PlayerPoints Points { get; } = new();
		public int MainId { get; set; }

		public void ChargeDkp (int points)
		{
			this.Points.ChargeDkp(points);
		}

	}
}
