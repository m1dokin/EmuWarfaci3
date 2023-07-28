using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using LibMetricsNet;
using MasterServer.Core;
using MasterServer.Core.Services.Metrics;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x020006F9 RID: 1785
	[Service]
	[Singleton]
	internal class LogMetricsTracker : BaseTracker, ILogMetricsTracker, IMetricsProvider, IDisposable
	{
		// Token: 0x06002566 RID: 9574 RVA: 0x0009CB7F File Offset: 0x0009AF7F
		public LogMetricsTracker(ILogService logService)
		{
			logService.OnEvent += this.ReportLogMessageSent;
		}

		// Token: 0x06002567 RID: 9575 RVA: 0x0009CBA4 File Offset: 0x0009AFA4
		public override IEnumerable<Metric> GetMetrics()
		{
			object @lock = this.m_lock;
			IEnumerable<Metric> result;
			lock (@lock)
			{
				result = this.m_logMetrics.Metrics.GetMetrics().ToArray<Metric>();
			}
			return result;
		}

		// Token: 0x06002568 RID: 9576 RVA: 0x0009CBF8 File Offset: 0x0009AFF8
		public override void Init(IMetricsService service)
		{
			base.Init(service);
			this.m_logMetrics = new LogMetricsTracker.LogMetrics(this.m_metricsService.MetricsConfig);
		}

		// Token: 0x06002569 RID: 9577 RVA: 0x0009CC18 File Offset: 0x0009B018
		public override void Dispose()
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (this.m_logMetrics != null)
				{
					this.m_logMetrics.Dispose();
					this.m_logMetrics = null;
				}
			}
		}

		// Token: 0x0600256A RID: 9578 RVA: 0x0009CC74 File Offset: 0x0009B074
		public void ReportLogMessageSent(string logCategory)
		{
			this.TryIncMeter(LogMetricsTracker.LogMetricType.ms_log_sent_messages);
		}

		// Token: 0x0600256B RID: 9579 RVA: 0x0009CC80 File Offset: 0x0009B080
		private void TryIncMeter(LogMetricsTracker.LogMetricType metricType)
		{
			if (this.m_metricsService.Enabled)
			{
				DateTime now = DateTime.Now;
				this.m_logMetrics.Metrics.Get<Meter>(metricType).Inc(now);
			}
		}

		// Token: 0x040012EE RID: 4846
		private LogMetricsTracker.LogMetrics m_logMetrics;

		// Token: 0x040012EF RID: 4847
		private readonly object m_lock = new object();

		// Token: 0x020006FA RID: 1786
		private enum LogMetricType
		{
			// Token: 0x040012F1 RID: 4849
			ms_log_sent_messages
		}

		// Token: 0x020006FB RID: 1787
		private class LogMetrics : IDisposable
		{
			// Token: 0x0600256C RID: 9580 RVA: 0x0009CCC0 File Offset: 0x0009B0C0
			public LogMetrics(MetricsConfig cfg)
			{
				this.Metrics = new EnumBasedMetricsContainer(typeof(LogMetricsTracker.LogMetricType), cfg);
				string[] tags = new string[]
				{
					"server",
					Resources.ServerName
				};
				this.Metrics.InitMeter(LogMetricsTracker.LogMetricType.ms_log_sent_messages, tags);
			}

			// Token: 0x0600256D RID: 9581 RVA: 0x0009CD12 File Offset: 0x0009B112
			public void Dispose()
			{
				if (this.Metrics != null)
				{
					this.Metrics.Dispose();
					this.Metrics = null;
				}
			}

			// Token: 0x040012F2 RID: 4850
			public EnumBasedMetricsContainer Metrics;
		}
	}
}
