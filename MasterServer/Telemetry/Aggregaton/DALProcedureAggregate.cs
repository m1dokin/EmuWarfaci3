using System;

namespace MasterServer.Telemetry.Aggregaton
{
	// Token: 0x02000729 RID: 1833
	internal class DALProcedureAggregate : AggregateBase
	{
		// Token: 0x04001376 RID: 4982
		public int Executed;

		// Token: 0x04001377 RID: 4983
		public int DatabaseQueries;

		// Token: 0x04001378 RID: 4984
		public AggregateBase.TimeTuple DatabaseTime;

		// Token: 0x04001379 RID: 4985
		public AggregateBase.TimeTuple ConnectingTime;

		// Token: 0x0400137A RID: 4986
		public AggregateBase.TimeTuple CacheTime;

		// Token: 0x0400137B RID: 4987
		public int L1CacheMisses;

		// Token: 0x0400137C RID: 4988
		public int L1CacheHits;

		// Token: 0x0400137D RID: 4989
		public int L1CacheClear;

		// Token: 0x0400137E RID: 4990
		public int L2CacheMisses;

		// Token: 0x0400137F RID: 4991
		public int L2CacheHits;

		// Token: 0x04001380 RID: 4992
		public int L2CacheClear;
	}
}
