using System;
using System.Collections.Generic;

namespace MasterServer.Telemetry.Aggregaton
{
	// Token: 0x02000726 RID: 1830
	internal class QueryAggregate : AggregateBase
	{
		// Token: 0x0400136A RID: 4970
		public int Executed;

		// Token: 0x0400136B RID: 4971
		public int Failed;

		// Token: 0x0400136C RID: 4972
		public AggregateBase.TimeTuple ServicingTime;

		// Token: 0x0400136D RID: 4973
		public AggregateBase.TimeTuple ProcessingTime;

		// Token: 0x0400136E RID: 4974
		public AggregateBase.TimeTuple DALTime;

		// Token: 0x0400136F RID: 4975
		public int DALCallsTotal;

		// Token: 0x04001370 RID: 4976
		public Dictionary<string, int> DALCalls = new Dictionary<string, int>();

		// Token: 0x04001371 RID: 4977
		public QueryAggregate.Traffic Download;

		// Token: 0x04001372 RID: 4978
		public QueryAggregate.Traffic Upload;

		// Token: 0x02000727 RID: 1831
		public struct Traffic
		{
			// Token: 0x04001373 RID: 4979
			public uint TotalData;

			// Token: 0x04001374 RID: 4980
			public uint CompressedData;
		}
	}
}
