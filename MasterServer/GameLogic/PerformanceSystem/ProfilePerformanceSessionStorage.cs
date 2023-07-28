using System;
using System.Collections.Concurrent;

namespace MasterServer.GameLogic.PerformanceSystem
{
	// Token: 0x020003F1 RID: 1009
	internal class ProfilePerformanceSessionStorage
	{
		// Token: 0x04000A7E RID: 2686
		public ConcurrentDictionary<ulong, ProfilePerformanceInfo> ProfilePerformanceInfos = new ConcurrentDictionary<ulong, ProfilePerformanceInfo>();
	}
}
