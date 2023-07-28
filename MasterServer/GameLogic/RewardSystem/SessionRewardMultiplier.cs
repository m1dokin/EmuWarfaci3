using System;
using System.Collections.Concurrent;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020005BE RID: 1470
	public class SessionRewardMultiplier
	{
		// Token: 0x04000F59 RID: 3929
		public readonly ConcurrentDictionary<ulong, SessionRewardMultiplier.ProfileRewardMultiplier> Multiplier = new ConcurrentDictionary<ulong, SessionRewardMultiplier.ProfileRewardMultiplier>();

		// Token: 0x020005BF RID: 1471
		public class ProfileRewardMultiplier
		{
			// Token: 0x04000F5A RID: 3930
			public SRewardMultiplier Multiplier;
		}
	}
}
