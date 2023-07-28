using System;
using System.Collections.Generic;
using HK2Net;
using OLAPHypervisor;

namespace MasterServer.GameLogic.PlayerStats
{
	// Token: 0x02000533 RID: 1331
	[Contract]
	internal interface IDebugPlayerStatsService
	{
		// Token: 0x06001CFA RID: 7418
		void FlushPlayerStats(ulong profileId, List<Measure> data);

		// Token: 0x06001CFB RID: 7419
		void ResetPlayerStats(ulong profileId);
	}
}
