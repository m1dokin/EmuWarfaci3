using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200037F RID: 895
	public class SessionBoosters
	{
		// Token: 0x04000960 RID: 2400
		public readonly ConcurrentDictionary<ulong, SessionBoosters.ProfileBoosters> Boosters = new ConcurrentDictionary<ulong, SessionBoosters.ProfileBoosters>();

		// Token: 0x02000380 RID: 896
		public class ProfileBoosters
		{
			// Token: 0x04000961 RID: 2401
			public bool IsVip;

			// Token: 0x04000962 RID: 2402
			public Dictionary<BoosterType, float> Boosters;
		}
	}
}
