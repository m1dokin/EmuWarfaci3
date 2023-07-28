using System;

namespace MasterServer.GameLogic.RewardSystem.RankConfig
{
	// Token: 0x020000CF RID: 207
	public class ChannelRankRestriction
	{
		// Token: 0x06000355 RID: 853 RVA: 0x0000F5CA File Offset: 0x0000D9CA
		public ChannelRankRestriction(uint channelMinRank, uint channelMaxRank)
		{
			this.ChannelMinRank = channelMinRank;
			this.ChannelMaxRank = channelMaxRank;
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000356 RID: 854 RVA: 0x0000F5E0 File Offset: 0x0000D9E0
		// (set) Token: 0x06000357 RID: 855 RVA: 0x0000F5E8 File Offset: 0x0000D9E8
		public uint ChannelMinRank { get; private set; }

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000358 RID: 856 RVA: 0x0000F5F1 File Offset: 0x0000D9F1
		// (set) Token: 0x06000359 RID: 857 RVA: 0x0000F5F9 File Offset: 0x0000D9F9
		public uint ChannelMaxRank { get; private set; }
	}
}
