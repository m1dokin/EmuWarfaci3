using System;

namespace MasterServer.DAL
{
	// Token: 0x0200006B RID: 107
	[Serializable]
	public class DALResultVoid
	{
		// Token: 0x06000118 RID: 280 RVA: 0x00004710 File Offset: 0x00002B10
		public DALResultVoid(DALStats stats)
		{
			this.Stats = stats;
		}

		// Token: 0x04000119 RID: 281
		public DALStats Stats;
	}
}
