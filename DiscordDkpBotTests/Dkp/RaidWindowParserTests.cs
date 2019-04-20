using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DiscordDkpBot.Dkp;

using NUnit.Framework;

namespace DiscordDkpBotTests.Dkp
{
	[TestFixture]
	public class RaidWindowParserTests
	{

		[Test]
		public void ParseRaidWindow()
		{
			//Arrange
			
			//Act
			List<Character> result = target.Parse(TestResources.RaidParse1).ToList();
			
			//Assert
			Assert.AreEqual(53, result.Count);
		}
		[SetUp]
		public void SetUp()
		{
			target = new RaidWindowParser();
		}

		private RaidWindowParser target;
	}

}
