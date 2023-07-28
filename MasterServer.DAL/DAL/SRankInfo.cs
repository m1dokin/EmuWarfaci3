using System;

namespace MasterServer.DAL
{
	// Token: 0x02000086 RID: 134
	[Serializable]
	public struct SRankInfo
	{
		// Token: 0x0600018F RID: 399 RVA: 0x00004FB5 File Offset: 0x000033B5
		public SRankInfo(byte rankId, ulong points, ulong rankStart, ulong nextRankStart)
		{
			this.RankId = (int)rankId;
			this.Points = points;
			this.RankStart = rankStart;
			this.NextRankStart = nextRankStart;
		}

		// Token: 0x06000190 RID: 400 RVA: 0x00004FD4 File Offset: 0x000033D4
		public override string ToString()
		{
			return string.Format("RankId: {0}, Points: {1}, RankStart: {2}, RankNext: {3}", new object[]
			{
				this.RankId,
				this.Points,
				this.RankStart,
				this.NextRankStart
			});
		}

		// Token: 0x0400015B RID: 347
		public int RankId;

		// Token: 0x0400015C RID: 348
		public ulong Points;

		// Token: 0x0400015D RID: 349
		public ulong RankStart;

		// Token: 0x0400015E RID: 350
		public ulong NextRankStart;
	}
}
