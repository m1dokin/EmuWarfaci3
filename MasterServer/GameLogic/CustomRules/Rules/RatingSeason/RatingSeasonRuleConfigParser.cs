using System;
using System.Xml;
using MasterServer.DAL.RatingSystem;

namespace MasterServer.GameLogic.CustomRules.Rules.RatingSeason
{
	// Token: 0x020000A8 RID: 168
	public class RatingSeasonRuleConfigParser
	{
		// Token: 0x060002A2 RID: 674 RVA: 0x0000D1F0 File Offset: 0x0000B5F0
		public RatingSeasonRuleConfig Parse(XmlElement config)
		{
			string attribute = config.GetAttribute("season_id_template");
			string attribute2 = config.GetAttribute("announcement_end_date");
			DateTime announcementEndDate = RatingSeasonDateParser.Parse(attribute2);
			string attribute3 = config.GetAttribute("games_end_date");
			DateTime gamesEndDate = RatingSeasonDateParser.Parse(attribute3);
			return new RatingSeasonRuleConfig
			{
				SeasonIdTemplate = attribute,
				AnnouncementEndDate = announcementEndDate,
				GamesEndDate = gamesEndDate
			};
		}
	}
}
