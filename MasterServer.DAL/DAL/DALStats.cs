using System;

namespace MasterServer.DAL
{
	// Token: 0x0200006E RID: 110
	[Serializable]
	public class DALStats
	{
		// Token: 0x0400011E RID: 286
		public TimeSpan DBTime;

		// Token: 0x0400011F RID: 287
		public int DBQueries;

		// Token: 0x04000120 RID: 288
		public int DBDeadlocks;

		// Token: 0x04000121 RID: 289
		public int DBDeadlocksTotal;

		// Token: 0x04000122 RID: 290
		public TimeSpan ConnectionAllocTime;
	}
}
