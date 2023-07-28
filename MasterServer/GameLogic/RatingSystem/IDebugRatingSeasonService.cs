using System;
using HK2Net;

namespace MasterServer.GameLogic.RatingSystem
{
	// Token: 0x020000DE RID: 222
	[Contract]
	internal interface IDebugRatingSeasonService
	{
		// Token: 0x0600039C RID: 924
		void SetupSeason(string seasonId, DateTime announcementEndDate, DateTime gamesEndDate, bool startSeason);
	}
}
