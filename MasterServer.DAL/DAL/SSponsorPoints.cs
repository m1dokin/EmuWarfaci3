using System;

namespace MasterServer.DAL
{
	// Token: 0x02000093 RID: 147
	[Serializable]
	public struct SSponsorPoints
	{
		// Token: 0x060001B9 RID: 441 RVA: 0x00005674 File Offset: 0x00003A74
		public SSponsorPoints(uint sponsorID)
		{
			this.SponsorID = sponsorID;
			this.RankInfo = default(SRankInfo);
			this.RankInfo.RankId = 1;
			this.NextUnlockItemId = 0UL;
		}

		// Token: 0x060001BA RID: 442 RVA: 0x000056AB File Offset: 0x00003AAB
		public SSponsorPoints(uint id, uint points, ulong itemId, byte stageId, ulong stageStart, ulong nextStageStart)
		{
			this.SponsorID = id;
			this.RankInfo = new SRankInfo(stageId, (ulong)points, stageStart, nextStageStart);
			this.NextUnlockItemId = itemId;
		}

		// Token: 0x060001BB RID: 443 RVA: 0x000056CE File Offset: 0x00003ACE
		public override string ToString()
		{
			return string.Format("SponsorId: {0}, NextUnlockItemId: {1}, {2}", this.SponsorID, this.NextUnlockItemId, this.RankInfo);
		}

		// Token: 0x04000180 RID: 384
		public uint SponsorID;

		// Token: 0x04000181 RID: 385
		public SRankInfo RankInfo;

		// Token: 0x04000182 RID: 386
		public ulong NextUnlockItemId;
	}
}
