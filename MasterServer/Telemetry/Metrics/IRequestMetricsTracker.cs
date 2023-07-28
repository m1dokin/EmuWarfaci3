using System;
using System.Net;
using HK2Net;
using MasterServer.Core.Services.Metrics;
using MasterServer.Core.Web;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x02000713 RID: 1811
	[Contract]
	internal interface IRequestMetricsTracker : IMetricsProvider, IDisposable
	{
		// Token: 0x060025C4 RID: 9668
		void ReportHttpRequestCompleted(RequestDomain domain, string host, string path);

		// Token: 0x060025C5 RID: 9669
		void ReportHttpRequestFailed(RequestDomain domain, string host, string path, HttpStatusCode statusCode);

		// Token: 0x060025C6 RID: 9670
		void ReportHttpRequestCrashed(RequestDomain domain, string host, string path);

		// Token: 0x060025C7 RID: 9671
		void ReportHttpRequestTime(RequestDomain domain, string host, string path, TimeSpan time);
	}
}
