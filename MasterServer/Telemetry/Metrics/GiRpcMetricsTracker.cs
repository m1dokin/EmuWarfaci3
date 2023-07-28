using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using LibMetricsNet;
using MasterServer.Core;
using MasterServer.Core.Services.Metrics;
using Util.Common;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x020006F5 RID: 1781
	[Service]
	[Singleton]
	internal class GiRpcMetricsTracker : BaseTracker, IGiRpcMetricsTracker, IMetricsProvider, IDisposable
	{
		// Token: 0x06002557 RID: 9559 RVA: 0x0009C8B4 File Offset: 0x0009ACB4
		public override void Dispose()
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_metricsMap.Values.ForEach(delegate(GiRpcMetricsTracker.GiRpcMetrics v)
				{
					v.Dispose();
				});
				this.m_metricsMap.Clear();
			}
		}

		// Token: 0x06002558 RID: 9560 RVA: 0x0009C92C File Offset: 0x0009AD2C
		public override IEnumerable<Metric> GetMetrics()
		{
			object @lock = this.m_lock;
			IEnumerable<Metric> result;
			lock (@lock)
			{
				result = this.m_metricsMap.Values.SelectMany((GiRpcMetricsTracker.GiRpcMetrics v) => v.Metrics.GetMetrics()).Cast<Metric>().ToArray<Metric>();
			}
			return result;
		}

		// Token: 0x06002559 RID: 9561 RVA: 0x0009C9A4 File Offset: 0x0009ADA4
		public void ReportConsumed(string domain)
		{
			this.TryIncMeter(domain, GiRpcMetricsTracker.GiMetricsType.ms_gi_consumed);
		}

		// Token: 0x0600255A RID: 9562 RVA: 0x0009C9AE File Offset: 0x0009ADAE
		public void ReportFailed(string domain)
		{
			this.TryIncMeter(domain, GiRpcMetricsTracker.GiMetricsType.ms_gi_failed);
		}

		// Token: 0x0600255B RID: 9563 RVA: 0x0009C9B8 File Offset: 0x0009ADB8
		public void ReportProcessingTime(string domain, TimeSpan time)
		{
			this.TryReportTime(domain, GiRpcMetricsTracker.GiMetricsType.ms_gi_processing_time, time);
		}

		// Token: 0x0600255C RID: 9564 RVA: 0x0009C9C4 File Offset: 0x0009ADC4
		private TMetric GetMetricForDomain<TMetric>(string domain, GiRpcMetricsTracker.GiMetricsType metricType) where TMetric : Metric
		{
			object @lock = this.m_lock;
			TMetric result;
			lock (@lock)
			{
				GiRpcMetricsTracker.GiRpcMetrics giRpcMetrics;
				if (!this.m_metricsMap.TryGetValue(domain, out giRpcMetrics))
				{
					this.m_metricsMap[domain] = new GiRpcMetricsTracker.GiRpcMetrics(domain, this.m_metricsService.MetricsConfig);
				}
				result = this.m_metricsMap[domain].Metrics.Get<TMetric>(metricType);
			}
			return result;
		}

		// Token: 0x0600255D RID: 9565 RVA: 0x0009CA50 File Offset: 0x0009AE50
		private void TryReportTime(string domain, GiRpcMetricsTracker.GiMetricsType metricType, TimeSpan time)
		{
			if (!this.m_metricsService.Enabled)
			{
				return;
			}
			DateTime now = DateTime.Now;
			this.GetMetricForDomain<Timer>(domain, metricType).TimeMilliseconds(now, time);
		}

		// Token: 0x0600255E RID: 9566 RVA: 0x0009CA84 File Offset: 0x0009AE84
		private void TryIncMeter(string domain, GiRpcMetricsTracker.GiMetricsType metricType)
		{
			if (!this.m_metricsService.Enabled)
			{
				return;
			}
			DateTime now = DateTime.Now;
			this.GetMetricForDomain<Meter>(domain, metricType).Inc(now);
		}

		// Token: 0x040012E5 RID: 4837
		private readonly Dictionary<string, GiRpcMetricsTracker.GiRpcMetrics> m_metricsMap = new Dictionary<string, GiRpcMetricsTracker.GiRpcMetrics>();

		// Token: 0x040012E6 RID: 4838
		private readonly object m_lock = new object();

		// Token: 0x020006F6 RID: 1782
		private enum GiMetricsType
		{
			// Token: 0x040012EA RID: 4842
			ms_gi_consumed,
			// Token: 0x040012EB RID: 4843
			ms_gi_failed,
			// Token: 0x040012EC RID: 4844
			ms_gi_processing_time
		}

		// Token: 0x020006F7 RID: 1783
		private class GiRpcMetrics : IDisposable
		{
			// Token: 0x06002561 RID: 9569 RVA: 0x0009CACC File Offset: 0x0009AECC
			public GiRpcMetrics(string domain, MetricsConfig metricsConfig)
			{
				this.Metrics = new EnumBasedMetricsContainer(typeof(GiRpcMetricsTracker.GiMetricsType), metricsConfig);
				string[] tags = new string[]
				{
					"server",
					Resources.ServerName,
					"domain",
					domain
				};
				this.Metrics.InitMeter(GiRpcMetricsTracker.GiMetricsType.ms_gi_consumed, tags);
				this.Metrics.InitMeter(GiRpcMetricsTracker.GiMetricsType.ms_gi_failed, tags);
				this.Metrics.InitTimer(GiRpcMetricsTracker.GiMetricsType.ms_gi_processing_time, tags);
			}

			// Token: 0x17000399 RID: 921
			// (get) Token: 0x06002562 RID: 9570 RVA: 0x0009CB4E File Offset: 0x0009AF4E
			// (set) Token: 0x06002563 RID: 9571 RVA: 0x0009CB56 File Offset: 0x0009AF56
			public EnumBasedMetricsContainer Metrics { get; private set; }

			// Token: 0x06002564 RID: 9572 RVA: 0x0009CB5F File Offset: 0x0009AF5F
			public void Dispose()
			{
				if (this.Metrics == null)
				{
					return;
				}
				this.Metrics.Dispose();
				this.Metrics = null;
			}
		}
	}
}
