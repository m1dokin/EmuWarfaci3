using System;
using HK2Net;
using MasterServer.Core.Configuration;
using MasterServer.Core.WebRequest;
using MasterServer.Telemetry.Metrics;
using Network.Http.Builders;
using Network.Http.Filters;
using Network.Monitoring;

namespace MasterServer.Core.Web
{
	// Token: 0x0200016A RID: 362
	[Service]
	internal class MSHttpClientBuilder : HttpClientBuilder
	{
		// Token: 0x0600067E RID: 1662 RVA: 0x0001A558 File Offset: 0x00018958
		public MSHttpClientBuilder(IRequestMetricsTracker metricsTracker)
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("WebRequestPool");
			this.m_cfg = new MSWebRequestConfig(section);
			base.Tracer(new IRequestTracer[]
			{
				new MetricRequestTracer(metricsTracker),
				new LogRequestTracer()
			}).ThrowOnHttpErrors().MaxConnections(this.m_cfg.MaxPendingRequests, this.m_cfg.AllocationTimeout);
		}

		// Token: 0x04000402 RID: 1026
		private readonly MSWebRequestConfig m_cfg;
	}
}
