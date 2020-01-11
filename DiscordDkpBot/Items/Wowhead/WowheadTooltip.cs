using System;

using Newtonsoft.Json;

namespace DiscordDkpBot.Items.Wowhead
{
	/*
	* {"name":"Deprecated Orc Apprentice Shirt",
	* "quality":1,
	* "icon":"inv_shirt_01",
	* "tooltip":"<table><tr><td><!--nstart--><b class=\"q1\">Deprecated Orc Apprentice Shirt</b><!--nend--><!--ndstart--><!--ndend--><span class=\"q\"><br>Item Level <!--ilvl-->1</span><table width=\"100%\"><tr><td>Shirt</td><th><!--scstart4:-8--><span class=\"q1\"></span><!--scend--></th></tr></table><!--ebstats--><!--egstats--><!--eistats--><!--e--><!--ps--></td></tr></table><table><tr><td><!--itemEffects:0--></td></tr></table>"}
	*
	*/

	public class WowheadTooltip
	{
		[JsonProperty(PropertyName = "icon")]
		public string Icon { get; set; }

		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "quality")]
		public int Quality { get; set; }

		[JsonProperty(PropertyName = "tooltip")]
		public string TooltipHtml { get; set; }
	}
}
