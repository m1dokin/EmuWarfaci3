using System;
using System.Collections.Generic;
using MasterServer.DAL.PlayerStats;
using OLAPHypervisor;

namespace MasterServer.DAL
{
	// Token: 0x0200003A RID: 58
	public interface IPlayerStatsSystem
	{
		// Token: 0x06000091 RID: 145
		DALResultVoid UpdatePlayerStats(ulong profileId, List<Measure> data);

		// Token: 0x06000092 RID: 146
		DALResult<PlayerStatistics> GetPlayerStats(ulong profileId);

		// Token: 0x06000093 RID: 147
		DALResultVoid ResetPlayerStats(ulong profileId);
	}
}
