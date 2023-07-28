using System;
using System.Collections.Generic;
using MasterServer.Core;

namespace MasterServer.GameLogic.RewardSystem.RankConfig
{
	// Token: 0x020000D0 RID: 208
	public class ChannelRankConfigBuilder
	{
		// Token: 0x0600035B RID: 859 RVA: 0x0000F60A File Offset: 0x0000DA0A
		public void SetNewbieProtectionRankClustering(NewbieProtectionRankClustering newbieProtectionRankClustering)
		{
			this.m_newbieProtectionRankClustering = newbieProtectionRankClustering;
		}

		// Token: 0x0600035C RID: 860 RVA: 0x0000F613 File Offset: 0x0000DA13
		public void SetRankRestrictions(Dictionary<Resources.ChannelType, ChannelRankRestriction> rankRestrictions)
		{
			this.m_rankRestrictions = rankRestrictions;
		}

		// Token: 0x0600035D RID: 861 RVA: 0x0000F61C File Offset: 0x0000DA1C
		public void SetExpCurve(List<ulong> expCurve)
		{
			this.m_expCurve = expCurve;
		}

		// Token: 0x0600035E RID: 862 RVA: 0x0000F625 File Offset: 0x0000DA25
		public void SetGlobalMaxRank(uint globalMaxRank)
		{
			this.m_globalMaxRank = globalMaxRank;
		}

		// Token: 0x0600035F RID: 863 RVA: 0x0000F630 File Offset: 0x0000DA30
		public ChannelRankConfig BuildForChannel(Resources.ChannelType channelType)
		{
			ChannelRankRestriction currentChannelRankRestriction = this.GetCurrentChannelRankRestriction(this.m_rankRestrictions, this.m_globalMaxRank, channelType);
			return new ChannelRankConfig
			{
				ExpCurve = this.m_expCurve,
				GlobalMaxRank = this.m_globalMaxRank,
				NewbieProtectionRankClustering = this.m_newbieProtectionRankClustering,
				RankRestriction = currentChannelRankRestriction
			};
		}

		// Token: 0x06000360 RID: 864 RVA: 0x0000F688 File Offset: 0x0000DA88
		private ChannelRankRestriction GetCurrentChannelRankRestriction(IDictionary<Resources.ChannelType, ChannelRankRestriction> rankRestrictions, uint globalMaxRank, Resources.ChannelType channelType)
		{
			ChannelRankRestriction result;
			if (!rankRestrictions.TryGetValue(channelType, out result))
			{
				result = new ChannelRankRestriction(1U, globalMaxRank);
				if (Resources.ChannelTypes.IsPvP(channelType))
				{
					Log.Warning<Resources.ChannelType>("Can't find join channel restriction for channel '{0}'. Using default restrictions", channelType);
				}
			}
			return result;
		}

		// Token: 0x0400016B RID: 363
		private NewbieProtectionRankClustering m_newbieProtectionRankClustering;

		// Token: 0x0400016C RID: 364
		private Dictionary<Resources.ChannelType, ChannelRankRestriction> m_rankRestrictions;

		// Token: 0x0400016D RID: 365
		private List<ulong> m_expCurve;

		// Token: 0x0400016E RID: 366
		private uint m_globalMaxRank;
	}
}
