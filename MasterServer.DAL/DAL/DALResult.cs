using System;

namespace MasterServer.DAL
{
	// Token: 0x0200006C RID: 108
	[Serializable]
	public class DALResult<T>
	{
		// Token: 0x06000119 RID: 281 RVA: 0x0000471F File Offset: 0x00002B1F
		public DALResult(T val, DALStats stats)
		{
			this.Value = val;
			this.Stats = stats;
		}

		// Token: 0x0400011A RID: 282
		public T Value;

		// Token: 0x0400011B RID: 283
		public DALStats Stats;
	}
}
