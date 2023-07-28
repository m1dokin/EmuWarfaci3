using System;
using System.Collections.Generic;

namespace MasterServer.DAL
{
	// Token: 0x02000052 RID: 82
	[Serializable]
	public struct MissionLB
	{
		// Token: 0x040000CA RID: 202
		public Guid MissionID;

		// Token: 0x040000CB RID: 203
		public List<MissionLB.StatData> StatsData;

		// Token: 0x02000053 RID: 83
		[Serializable]
		public struct Entry
		{
			// Token: 0x040000CC RID: 204
			public ulong ProfileID;

			// Token: 0x040000CD RID: 205
			public ulong UserID;

			// Token: 0x040000CE RID: 206
			public uint Performance;
		}

		// Token: 0x02000054 RID: 84
		[Serializable]
		public struct StatData
		{
			// Token: 0x040000CF RID: 207
			public uint Stat;

			// Token: 0x040000D0 RID: 208
			public List<MissionLB.Entry> Entries;
		}
	}
}
