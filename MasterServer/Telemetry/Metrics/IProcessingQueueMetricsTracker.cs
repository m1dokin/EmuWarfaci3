using System;
using HK2Net;
using MasterServer.Core.Services.Metrics;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x020006D0 RID: 1744
	[Contract]
	public interface IProcessingQueueMetricsTracker : IMetricsProvider, IDisposable
	{
		// Token: 0x060024B4 RID: 9396
		void ReportConsumed(string queueName);

		// Token: 0x060024B5 RID: 9397
		void ReportSkipped(string queueName);

		// Token: 0x060024B6 RID: 9398
		void ReportProcessingTime(string queueName, TimeSpan elapsed);
	}
}
