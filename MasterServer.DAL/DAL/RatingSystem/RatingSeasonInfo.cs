using System;

namespace MasterServer.DAL.RatingSystem
{
	// Token: 0x02000090 RID: 144
	[Serializable]
	public struct RatingSeasonInfo
	{
		// Token: 0x0400017C RID: 380
		public string SeasonId;

		// Token: 0x0400017D RID: 381
		public bool IsActive;

		// Token: 0x0400017E RID: 382
		public DateTime AnnouncementEndDate;

		// Token: 0x0400017F RID: 383
		public DateTime GamesEndDate;
	}
}
