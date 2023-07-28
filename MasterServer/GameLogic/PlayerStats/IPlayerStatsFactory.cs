using System;
using System.Collections.Generic;
using HK2Net;
using OLAPHypervisor;

namespace MasterServer.GameLogic.PlayerStats
{
	// Token: 0x02000534 RID: 1332
	[Contract]
	internal interface IPlayerStatsFactory
	{
		// Token: 0x06001CFC RID: 7420
		List<Measure> PvPKillDeathRatioReset(ulong profileId);

		// Token: 0x06001CFD RID: 7421
		List<Measure> PvPWinLoseRatioReset(ulong profileId);
	}
}
