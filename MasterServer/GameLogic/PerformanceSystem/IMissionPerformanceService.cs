using System;
using HK2Net;
using MasterServer.DAL;

namespace MasterServer.GameLogic.PerformanceSystem
{
	// Token: 0x020003F2 RID: 1010
	[Contract]
	internal interface IMissionPerformanceService
	{
		// Token: 0x060015E4 RID: 5604
		ProfilePerformanceInfo GetProfilePerformance(ulong profile_id);

		// Token: 0x060015E5 RID: 5605
		void UpdatePlayersPerformance(PerformanceUpdate upd);
	}
}
