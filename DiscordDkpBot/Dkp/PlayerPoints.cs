using System;

namespace DiscordDkpBot.Dkp
{
	public class PlayerPoints
	{
		public decimal PointsAdjustment { get; set; }
		public decimal PointsAdjustmentWithTwink { get; set; }
		public decimal PointsCurrent { get; set; }
		public decimal PointsCurrentWithTwink { get; set; }
		public decimal PointsEarned { get; set; }
		public decimal PointsEarnedWithTwink { get; set; }
		public decimal PointsSpent { get; set; }
		public decimal PointsSpentWithTwink { get; set; }

		public PlayerPoints () { }

		public PlayerPoints (decimal pointsEarned, decimal pointsEarnedWithTwink, decimal pointsSpent, decimal pointsSpentWithTwink, decimal pointsAdjustment, decimal pointsAdjustmentWithTwink, decimal pointsCurrent, decimal pointsCurrentWithTwink)
		{
			this.PointsSpent = pointsSpent;
			this.PointsSpentWithTwink = pointsSpentWithTwink;
			this.PointsAdjustment = pointsAdjustment;
			this.PointsAdjustmentWithTwink = pointsAdjustmentWithTwink;
			this.PointsCurrent = pointsCurrent;
			this.PointsCurrentWithTwink = pointsCurrentWithTwink;
			this.PointsEarned = pointsEarned;
			this.PointsEarnedWithTwink = pointsEarnedWithTwink;
		}

		public void ChargeDkp (int points)
		{
			this.PointsSpent += points;
			this.PointsCurrent -= points;
		}
	}
}
