using System;
using MasterServer.Core.Configuration;

namespace MasterServer.GameLogic.RewardSystem.RankConfig.Deserializers
{
	// Token: 0x020000D2 RID: 210
	public class NewbieProtectionRankClusteringDeserializer
	{
		// Token: 0x0600036B RID: 875 RVA: 0x0000F8F4 File Offset: 0x0000DCF4
		public NewbieProtectionRankClustering Deserialize(ConfigSection newbieProtectionRankClustering)
		{
			NewbieProtectionRankClustering newbieProtectionRankClustering2 = new NewbieProtectionRankClustering();
			bool rankClusteringEnabled;
			newbieProtectionRankClustering.Get("enabled", out rankClusteringEnabled);
			int newbieRank;
			newbieProtectionRankClustering.Get("newbie_rank", out newbieRank);
			newbieProtectionRankClustering2.RankClusteringEnabled = rankClusteringEnabled;
			newbieProtectionRankClustering2.NewbieRank = newbieRank;
			return newbieProtectionRankClustering2;
		}
	}
}
