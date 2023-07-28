using System;
using System.Collections.Generic;
using System.Xml;
using HK2Net;
using MasterServer.Core.Configs;
using MasterServer.Core.Services;
using MasterServer.GameLogic.Configs;
using MasterServer.GameLogic.CustomRules.Rules.RatingSeason;

namespace MasterServer.GameLogic.RatingSystem
{
	// Token: 0x020000C6 RID: 198
	[Service]
	[Singleton]
	internal class RatingSeasonClientConfigProvider : ServiceModule, IConfigProvider
	{
		// Token: 0x06000331 RID: 817 RVA: 0x0000EE3B File Offset: 0x0000D23B
		public RatingSeasonClientConfigProvider(IRatingSeasonService ratingSeasonService, IConfigProvider<RatingSeasonConfig> ratingSeasonConfigProvider)
		{
			this.m_ratingSeasonService = ratingSeasonService;
			this.m_ratingSeasonConfigProvider = ratingSeasonConfigProvider;
		}

		// Token: 0x06000332 RID: 818 RVA: 0x0000EE54 File Offset: 0x0000D254
		public int GetHash()
		{
			RatingSeason ratingSeason = this.m_ratingSeasonService.GetRatingSeason();
			return ratingSeason.GetHashCode();
		}

		// Token: 0x06000333 RID: 819 RVA: 0x0000EE74 File Offset: 0x0000D274
		public IEnumerable<XmlElement> GetConfig(XmlDocument doc)
		{
			RatingSeason ratingSeason = this.m_ratingSeasonService.GetRatingSeason();
			bool enabled = this.m_ratingSeasonConfigProvider.Get().Enabled;
			List<XmlElement> list = new List<XmlElement>();
			XmlElement xmlElement = doc.CreateElement("ratingseason");
			xmlElement.SetAttribute("enabled", (!enabled) ? "0" : "1");
			xmlElement.SetAttribute("announcement_end_date", ratingSeason.AnnouncementEndDate.ToString("yyyy-MM-ddTHH:mm"));
			xmlElement.SetAttribute("games_end_date", ratingSeason.GamesEndDate.ToString("yyyy-MM-ddTHH:mm"));
			list.Add(xmlElement);
			return list;
		}

		// Token: 0x0400015C RID: 348
		private readonly IRatingSeasonService m_ratingSeasonService;

		// Token: 0x0400015D RID: 349
		private readonly IConfigProvider<RatingSeasonConfig> m_ratingSeasonConfigProvider;
	}
}
