using System;
using System.Collections.Generic;
using HK2Net;

namespace MasterServer.Matchmaking
{
	// Token: 0x02000507 RID: 1287
	[Contract]
	internal interface IMatchmakingMissionsProvider
	{
		// Token: 0x170002DF RID: 735
		// (get) Token: 0x06001BCE RID: 7118
		IEnumerable<string> AutostartMissions { get; }

		// Token: 0x170002E0 RID: 736
		// (get) Token: 0x06001BCF RID: 7119
		IEnumerable<string> PvpMissions { get; }

		// Token: 0x170002E1 RID: 737
		// (get) Token: 0x06001BD0 RID: 7120
		IEnumerable<string> RatingGameMissions { get; }

		// Token: 0x06001BD1 RID: 7121
		void ReloadMapLists();
	}
}
