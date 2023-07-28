using System;
using HK2Net;
using MasterServer.Core.Services.Metrics;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x020006F4 RID: 1780
	[Contract]
	internal interface IGiRpcMetricsTracker : IMetricsProvider, IDisposable
	{
		// Token: 0x06002553 RID: 9555
		void ReportConsumed(string domain);

		// Token: 0x06002554 RID: 9556
		void ReportFailed(string domain);

		// Token: 0x06002555 RID: 9557
		void ReportProcessingTime(string domain, TimeSpan time);
	}
}
