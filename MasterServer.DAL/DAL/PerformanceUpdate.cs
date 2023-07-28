using System;
using System.Collections.Generic;

namespace MasterServer.DAL
{
	// Token: 0x0200004E RID: 78
	[Serializable]
	public struct PerformanceUpdate
	{
		// Token: 0x040000BF RID: 191
		public Guid MissionID;

		// Token: 0x040000C0 RID: 192
		public MissionStatus Status;

		// Token: 0x040000C1 RID: 193
		public List<PerformanceInfo> Performances;

		// Token: 0x040000C2 RID: 194
		public List<ulong> ProfilesIds;
	}
}
