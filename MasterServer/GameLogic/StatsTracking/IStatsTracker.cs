using System;
using HK2Net;

namespace MasterServer.GameLogic.StatsTracking
{
	// Token: 0x020005D2 RID: 1490
	[Contract]
	public interface IStatsTracker
	{
		// Token: 0x06001FC7 RID: 8135
		void ChangeStatistics(ulong profileId, EStatsEvent eventId, object eventParams);

		// Token: 0x06001FC8 RID: 8136
		void ResetStatistics(ulong profileId, EStatsEvent eventId, object eventParams);

		// Token: 0x1400007A RID: 122
		// (add) Token: 0x06001FC9 RID: 8137
		// (remove) Token: 0x06001FCA RID: 8138
		event StatsChangedDelegate OnStatisticsChanged;

		// Token: 0x1400007B RID: 123
		// (add) Token: 0x06001FCB RID: 8139
		// (remove) Token: 0x06001FCC RID: 8140
		event StatsChangedDelegate OnStatisticsReset;
	}
}
