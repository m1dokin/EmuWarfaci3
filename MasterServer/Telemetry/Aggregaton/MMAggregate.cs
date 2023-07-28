using System;

namespace MasterServer.Telemetry.Aggregaton
{
	// Token: 0x0200072B RID: 1835
	internal class MMAggregate : AggregateBase
	{
		// Token: 0x04001382 RID: 4994
		public AggregateBase.TimeTuple ExecuteTime;

		// Token: 0x04001383 RID: 4995
		public int MaxPlayers;

		// Token: 0x04001384 RID: 4996
		public int Players;

		// Token: 0x04001385 RID: 4997
		public int Executed;
	}
}
