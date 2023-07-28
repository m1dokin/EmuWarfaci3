using System;
using HK2Net;
using HK2Net.Attributes.Bootstrap;

namespace MasterServer.Matchmaking
{
	// Token: 0x020004FE RID: 1278
	[Contract]
	[BootstrapExplicit]
	internal interface IMatchmakingPerformer
	{
		// Token: 0x1400006A RID: 106
		// (add) Token: 0x06001B8C RID: 7052
		// (remove) Token: 0x06001B8D RID: 7053
		event MatchmakingPerformerStatsDeleg OnMatchmakingPerformerStats;
	}
}
