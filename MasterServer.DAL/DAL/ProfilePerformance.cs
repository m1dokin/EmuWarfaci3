using System;
using System.Collections.Generic;

namespace MasterServer.DAL
{
	// Token: 0x0200004A RID: 74
	[Serializable]
	public struct ProfilePerformance
	{
		// Token: 0x040000B5 RID: 181
		public ulong ProfileID;

		// Token: 0x040000B6 RID: 182
		public List<ProfilePerformance.MissionPerfInfo> Missions;

		// Token: 0x0200004B RID: 75
		[Serializable]
		public struct MissionPerfInfo
		{
			// Token: 0x040000B7 RID: 183
			public Guid MissionID;

			// Token: 0x040000B8 RID: 184
			public MissionStatus Status;

			// Token: 0x040000B9 RID: 185
			public List<PerformanceInfo> Performances;
		}
	}
}
