using System;
using HK2Net;

namespace MasterServer.Matchmaking
{
	// Token: 0x020004F9 RID: 1273
	[Service]
	[Singleton]
	internal class EmptyMatchmakingPerformer : IMatchmakingPerformer
	{
		// Token: 0x14000068 RID: 104
		// (add) Token: 0x06001B77 RID: 7031 RVA: 0x0006F40C File Offset: 0x0006D80C
		// (remove) Token: 0x06001B78 RID: 7032 RVA: 0x0006F444 File Offset: 0x0006D844
		public event MatchmakingPerformerStatsDeleg OnMatchmakingPerformerStats;
	}
}
