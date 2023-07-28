using System;
using System.Collections.Generic;

namespace MasterServer.DAL
{
	// Token: 0x0200004C RID: 76
	[Serializable]
	public struct MissionPerformance
	{
		// Token: 0x040000BA RID: 186
		public Guid MissionID;

		// Token: 0x040000BB RID: 187
		public List<MissionPerformance.TopTeam> TopTeams;

		// Token: 0x0200004D RID: 77
		[Serializable]
		public struct TopTeam
		{
			// Token: 0x040000BC RID: 188
			public uint Stat;

			// Token: 0x040000BD RID: 189
			public uint Performance;

			// Token: 0x040000BE RID: 190
			public List<ulong> ProfileIds;
		}
	}
}
