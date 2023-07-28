using System;
using MasterServer.Core.Configuration;

namespace MasterServer.GameLogic.RatingSystem
{
	// Token: 0x020000C8 RID: 200
	public class RatingSeasonConfigParser
	{
		// Token: 0x06000338 RID: 824 RVA: 0x0000EF38 File Offset: 0x0000D338
		public RatingSeasonConfig Parse(ConfigSection ratingSeasonConfigData)
		{
			bool enabled;
			ratingSeasonConfigData.Get("enabled", out enabled);
			return new RatingSeasonConfig
			{
				Enabled = enabled
			};
		}
	}
}
