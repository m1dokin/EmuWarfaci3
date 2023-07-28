using System;

namespace MasterServer.GameLogic.RewardSystem.RankConfig
{
	// Token: 0x020000D5 RID: 213
	public class NewbieProtectionRankClustering
	{
		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000371 RID: 881 RVA: 0x0000FAA4 File Offset: 0x0000DEA4
		// (set) Token: 0x06000372 RID: 882 RVA: 0x0000FAAC File Offset: 0x0000DEAC
		public bool RankClusteringEnabled { get; set; }

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000373 RID: 883 RVA: 0x0000FAB5 File Offset: 0x0000DEB5
		// (set) Token: 0x06000374 RID: 884 RVA: 0x0000FABD File Offset: 0x0000DEBD
		public int NewbieRank { get; set; }
	}
}
