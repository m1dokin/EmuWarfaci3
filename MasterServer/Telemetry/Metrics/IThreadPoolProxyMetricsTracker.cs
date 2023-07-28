using System;
using HK2Net;
using MasterServer.Core.Services.Metrics;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x02000704 RID: 1796
	[Contract]
	internal interface IThreadPoolProxyMetricsTracker : IMetricsProvider, IDisposable
	{
		// Token: 0x0600258F RID: 9615
		void Report();
	}
}
