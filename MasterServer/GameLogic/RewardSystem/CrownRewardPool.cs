using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x0200059A RID: 1434
	internal class CrownRewardPool
	{
		// Token: 0x06001EDC RID: 7900 RVA: 0x0007D394 File Offset: 0x0007B794
		public CrownRewardPool()
		{
			this.m_rewardThreshold = new CrownRewardThreshold();
		}

		// Token: 0x06001EDD RID: 7901 RVA: 0x0007D3A8 File Offset: 0x0007B7A8
		public void CalculateTreshold(MissionContext missionContext)
		{
			CrownRewardThreshold crownRewardThreshold = null;
			foreach (SubLevel subLevel in missionContext.subLevels)
			{
				if (crownRewardThreshold == null)
				{
					crownRewardThreshold = subLevel.crownRewardPool.m_rewardThreshold;
				}
				else
				{
					crownRewardThreshold += subLevel.crownRewardPool.m_rewardThreshold;
				}
			}
			this.m_rewardThreshold = (crownRewardThreshold ?? new CrownRewardThreshold());
		}

		// Token: 0x06001EDE RID: 7902 RVA: 0x0007D43C File Offset: 0x0007B83C
		public bool TryGetThreshold(CrownRewardThreshold.PerformanceCategory performanceCategory, out LeagueThresholdBasic threshold)
		{
			return this.m_rewardThreshold.TryGetThreshold(performanceCategory, out threshold);
		}

		// Token: 0x06001EDF RID: 7903 RVA: 0x0007D44B File Offset: 0x0007B84B
		public void Dump()
		{
			Log.Info(this.m_rewardThreshold.ToString());
		}

		// Token: 0x06001EE0 RID: 7904 RVA: 0x0007D45D File Offset: 0x0007B85D
		public bool IsValid()
		{
			return this.m_rewardThreshold.IsValid();
		}

		// Token: 0x06001EE1 RID: 7905 RVA: 0x0007D46A File Offset: 0x0007B86A
		public bool TryParse(XmlTextReader reader)
		{
			return this.m_rewardThreshold.TryParse(reader);
		}

		// Token: 0x04000F0D RID: 3853
		private CrownRewardThreshold m_rewardThreshold;
	}
}
