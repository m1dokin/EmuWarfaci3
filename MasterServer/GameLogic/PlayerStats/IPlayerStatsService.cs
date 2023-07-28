using System;
using System.Collections.Generic;
using HK2Net;
using OLAPHypervisor;

namespace MasterServer.GameLogic.PlayerStats
{
	// Token: 0x02000532 RID: 1330
	[Contract]
	internal interface IPlayerStatsService
	{
		// Token: 0x06001CF5 RID: 7413
		void InitPlayerStats(ulong profileId);

		// Token: 0x06001CF6 RID: 7414
		void UpdatePlayerStats(List<Measure> data);

		// Token: 0x06001CF7 RID: 7415
		void FlushPlayerStats(List<Measure> data);

		// Token: 0x06001CF8 RID: 7416
		List<Measure> GetPlayerStats(ulong profileId);

		// Token: 0x06001CF9 RID: 7417
		List<Measure> GetPlayerStatsAggregated(ulong profileId);
	}
}
