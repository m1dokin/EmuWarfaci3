using System;

namespace MasterServer.DAL.RatingSystem
{
	// Token: 0x0200008C RID: 140
	[Serializable]
	public struct RatingInfo
	{
		// Token: 0x060001A9 RID: 425 RVA: 0x000055A1 File Offset: 0x000039A1
		public bool IsEmpty()
		{
			return this.RatingPoints == 0U && string.IsNullOrEmpty(this.SeasonId);
		}

		// Token: 0x04000177 RID: 375
		public ulong ProfileId;

		// Token: 0x04000178 RID: 376
		public uint RatingPoints;

		// Token: 0x04000179 RID: 377
		public uint WinStreak;

		// Token: 0x0400017A RID: 378
		public string SeasonId;
	}
}
