using System;
using System.Collections.Generic;

namespace MasterServer.DAL
{
	// Token: 0x0200006D RID: 109
	[Serializable]
	public class DALResultMulti<T>
	{
		// Token: 0x0600011A RID: 282 RVA: 0x00004735 File Offset: 0x00002B35
		public DALResultMulti(IEnumerable<T> val, DALStats stats)
		{
			this.Values = val;
			this.Stats = stats;
		}

		// Token: 0x0400011C RID: 284
		public IEnumerable<T> Values;

		// Token: 0x0400011D RID: 285
		public DALStats Stats;
	}
}
