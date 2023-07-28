using System;
using System.Collections.Generic;

namespace MasterServer.DAL
{
	// Token: 0x0200004F RID: 79
	[Serializable]
	public struct MasterRecord
	{
		// Token: 0x040000C3 RID: 195
		public DateTime MinLastUpdateUtc;

		// Token: 0x040000C4 RID: 196
		public List<MasterRecord.Record> Records;

		// Token: 0x02000050 RID: 80
		[Serializable]
		public struct StatSamples
		{
			// Token: 0x040000C5 RID: 197
			public uint Stat;

			// Token: 0x040000C6 RID: 198
			public List<KeyValuePair<float, float>> Samples;
		}

		// Token: 0x02000051 RID: 81
		[Serializable]
		public struct Record
		{
			// Token: 0x040000C7 RID: 199
			public string RecordID;

			// Token: 0x040000C8 RID: 200
			public DateTime LastUpdateUtc;

			// Token: 0x040000C9 RID: 201
			public List<MasterRecord.StatSamples> StatSamples;
		}
	}
}
