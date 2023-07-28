using System;

namespace MasterServer.GameLogic.RatingSystem
{
	// Token: 0x0200009E RID: 158
	public class RatingSeasonRangeHelper
	{
		// Token: 0x0600026B RID: 619 RVA: 0x0000C716 File Offset: 0x0000AB16
		public static bool IsAnnouncement(DateTime announcementEndDate)
		{
			return DateTime.UtcNow < announcementEndDate;
		}

		// Token: 0x0600026C RID: 620 RVA: 0x0000C724 File Offset: 0x0000AB24
		public static bool IsActive(DateTime announcementEndDate, DateTime gamesEndDate)
		{
			DateTime utcNow = DateTime.UtcNow;
			return utcNow >= announcementEndDate && utcNow < gamesEndDate;
		}

		// Token: 0x0600026D RID: 621 RVA: 0x0000C74D File Offset: 0x0000AB4D
		public static bool IsFinished(DateTime gamesEndDate)
		{
			return DateTime.UtcNow >= gamesEndDate;
		}
	}
}
