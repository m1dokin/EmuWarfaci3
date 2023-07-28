using System;

namespace MasterServer.Telemetry.Aggregaton
{
	// Token: 0x02000724 RID: 1828
	internal class AggregateBase
	{
		// Token: 0x02000725 RID: 1829
		public struct TimeTuple
		{
			// Token: 0x060025FB RID: 9723 RVA: 0x0009FA81 File Offset: 0x0009DE81
			public void apply(TimeSpan ts)
			{
				this.Total += ts;
				if (this.Max < ts)
				{
					this.Max = ts;
				}
			}

			// Token: 0x04001368 RID: 4968
			public TimeSpan Total;

			// Token: 0x04001369 RID: 4969
			public TimeSpan Max;
		}
	}
}
