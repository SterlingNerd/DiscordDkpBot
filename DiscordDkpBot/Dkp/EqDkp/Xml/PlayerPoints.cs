using System;
using System.Xml.Serialization;

namespace DiscordDkpBot.Dkp.EqDkp.Xml
{
	[XmlType ("multidkp_points")]
	public class PlayerPoints
	{
		[XmlElement ("multidkp_id")]
		public decimal DkpPoolId { get; set; }

		[XmlElement ("points_spent")]
		public decimal PointsSpent { get; set; }

		[XmlElement ("points_spent_with_twink")]
		public decimal PointsSpentWithTwink { get; set; }

		[XmlElement ("points_adjustment")]
		public decimal PointsAdjustment { get; set; }

		[XmlElement ("points_adjustment_with_twink")]
		public decimal PointsAdjustmentWithTwink { get; set; }

		[XmlElement ("points_current")]
		public decimal PointsCurrent { get; set; }

		[XmlElement ("points_current_with_twink")]
		public decimal PointsCurrentWithTwink { get; set; }

		[XmlElement ("points_earned")]
		public decimal PointsEarned { get; set; }

		[XmlElement ("points_earned_with_twink")]
		public decimal PointsEarnedWithTwink { get; set; }
	}
}
