using System;
using System.Collections.Generic;

namespace MasterServer.GameLogic.RewardSystem.RankConfig
{
	// Token: 0x020000D7 RID: 215
	public class ChannelRankConfig
	{
		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000381 RID: 897 RVA: 0x0000FDDE File Offset: 0x0000E1DE
		// (set) Token: 0x06000382 RID: 898 RVA: 0x0000FDE6 File Offset: 0x0000E1E6
		public NewbieProtectionRankClustering NewbieProtectionRankClustering { get; set; }

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000383 RID: 899 RVA: 0x0000FDEF File Offset: 0x0000E1EF
		// (set) Token: 0x06000384 RID: 900 RVA: 0x0000FDF7 File Offset: 0x0000E1F7
		public ChannelRankRestriction RankRestriction { get; set; }

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000385 RID: 901 RVA: 0x0000FE00 File Offset: 0x0000E200
		// (set) Token: 0x06000386 RID: 902 RVA: 0x0000FE08 File Offset: 0x0000E208
		public List<ulong> ExpCurve { get; set; }

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000387 RID: 903 RVA: 0x0000FE11 File Offset: 0x0000E211
		// (set) Token: 0x06000388 RID: 904 RVA: 0x0000FE19 File Offset: 0x0000E219
		public uint GlobalMaxRank { get; set; }
	}
}
