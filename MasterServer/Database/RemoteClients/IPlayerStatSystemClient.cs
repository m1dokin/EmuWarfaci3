using System;
using System.Collections.Generic;
using MasterServer.DAL.PlayerStats;
using OLAPHypervisor;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x02000209 RID: 521
	internal interface IPlayerStatSystemClient
	{
		// Token: 0x06000B0D RID: 2829
		void UpdatePlayerStats(ulong profileId, List<Measure> data);

		// Token: 0x06000B0E RID: 2830
		PlayerStatistics GetPlayerStats(ulong profileId);

		// Token: 0x06000B0F RID: 2831
		void ResetPlayerStats(ulong profileId);
	}
}
