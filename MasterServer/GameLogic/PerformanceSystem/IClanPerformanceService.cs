using System;
using HK2Net;
using MasterServer.DAL;

namespace MasterServer.GameLogic.PerformanceSystem
{
	// Token: 0x020003EA RID: 1002
	[Contract]
	internal interface IClanPerformanceService
	{
		// Token: 0x060015BF RID: 5567
		ClanPerformanceInfo GetClanPerformance(ulong clan_id);

		// Token: 0x060015C0 RID: 5568
		bool RefreshLeaderboard();

		// Token: 0x060015C1 RID: 5569
		MasterRecord ReadMasterRecordFromDB();

		// Token: 0x060015C2 RID: 5570
		void RefreshMasterRecord();

		// Token: 0x060015C3 RID: 5571
		void ForceRefreshMasterRecord();

		// Token: 0x14000047 RID: 71
		// (add) Token: 0x060015C4 RID: 5572
		// (remove) Token: 0x060015C5 RID: 5573
		event PerformanceServiceStatsDeleg OnServiceStats;
	}
}
