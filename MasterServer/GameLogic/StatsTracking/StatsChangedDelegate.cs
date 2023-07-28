using System;
using System.Collections.Generic;

namespace MasterServer.GameLogic.StatsTracking
{
	// Token: 0x020005D1 RID: 1489
	// (Invoke) Token: 0x06001FC4 RID: 8132
	public delegate void StatsChangedDelegate(ulong profileId, EStatsEvent eventId, List<IStatsFilter> filters, int value);
}
