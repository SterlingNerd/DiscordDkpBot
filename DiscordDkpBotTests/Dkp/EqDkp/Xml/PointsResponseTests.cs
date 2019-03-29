using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using DiscordDkpBot.Dkp.EqDkpPlus.Xml;

using NUnit.Framework;

namespace DiscordDkpBotTests.Dkp.EqDkpPlus.Xml
{
	[TestFixture]
	public class PointsResponseTests
	{
		[Test]
		public void DeserializeTest ()
		{
			//Arrange
			XmlSerializer serializer = new XmlSerializer(typeof(PointsResponse));

			PointsResponse response;
			//Act
			using (StringReader reader = new StringReader(TestResources.EqDkpPlus_Points))
			{
				response = serializer.Deserialize(reader) as PointsResponse;
			}

			//Assert
			Assert.IsNotNull(response);
			Assert.True(((ICollection)response.Players).Count > 1, "Must be more than 1 player.");
			Assert.IsNotNull(response.DkpPools);
			Assert.IsTrue(response.DkpPools.Length > 0);
			Assert.IsNotNull(response.DkpPools[0]);
			Assert.IsNotNull(response.DkpPools[0].Events);
			Assert.IsTrue(response.DkpPools[0].Events.Length > 0);
			Assert.IsNotNull(response.DkpPools[0].Events[0]);
			Assert.AreEqual(1, response.DkpPools[0].Events[0].Id);


		}

		[Test]
		public void PlayerDeserialize ()
		{
			//Arrange
			const string xml = @"<player>
      <id>553</id>
      <name>2</name>
      <active>0</active>
      <hidden>0</hidden>
      <main_id>553</main_id>
      <main_name>2</main_name>
      <class_id>0</class_id>
      <class_name>Unknown</class_name>
      <points>
        <multidkp_points>
          <multidkp_id>1</multidkp_id>
          <points_current>0.00</points_current>
          <points_current_with_twink>0.00</points_current_with_twink>
          <points_earned>0.00</points_earned>
          <points_earned_with_twink>0.00</points_earned_with_twink>
          <points_spent>0.00</points_spent>
          <points_spent_with_twink>0.00</points_spent_with_twink>
          <points_adjustment>0.00</points_adjustment>
          <points_adjustment_with_twink>0.00</points_adjustment_with_twink>
        </multidkp_points>
      </points>
      <items/>
      <adjustments/>
    </player>";

			XmlSerializer serializer = new XmlSerializer(typeof(Player));

			//Act
			Player r = serializer.Deserialize(new StringReader(xml)) as Player;

			//Assert
			Assert.IsNotNull(r);
		}

		[Test]
		public void PlayerPointsDeserialize ()
		{
			//Arrange

			XmlSerializer serializer = new XmlSerializer(typeof(PointsResponse));

			//Act
			PointsResponse r = serializer.Deserialize(new StringReader(TestResources.MaggDkp)) as PointsResponse;
			PlayerPoints points = r?.Players.FirstOrDefault()?.Points?.FirstOrDefault();
			//Assert
			Assert.IsNotNull(points);
			Assert.AreEqual(959, points.PointsCurrent);
			Assert.AreEqual(547, points.PointsCurrentWithTwink);
			Assert.AreEqual(4602, points.PointsEarned);
			Assert.AreEqual(8554, points.PointsEarnedWithTwink);
			Assert.AreEqual(3793, points.PointsSpent);
			Assert.AreEqual(7957, points.PointsSpentWithTwink);
			Assert.AreEqual(150, points.PointsAdjustment);
			Assert.AreEqual(-50, points.PointsAdjustmentWithTwink);
		}
	}
}
