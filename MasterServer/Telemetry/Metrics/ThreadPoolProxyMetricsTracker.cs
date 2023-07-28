using System;
using System.Collections;
using System.Collections.Generic;
using HK2Net;
using LibMetricsNet;
using MasterServer.Core;
using MasterServer.Core.Services.Metrics;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x02000705 RID: 1797
	[Service]
	[Singleton]
	internal class ThreadPoolProxyMetricsTracker : BaseTracker, IThreadPoolProxyMetricsTracker, IMetricsProvider, IDisposable
	{
		// Token: 0x06002591 RID: 9617 RVA: 0x0009D7FB File Offset: 0x0009BBFB
		public override void Dispose()
		{
		}

		// Token: 0x06002592 RID: 9618 RVA: 0x0009D7FD File Offset: 0x0009BBFD
		public override void Init(IMetricsService service)
		{
			base.Init(service);
			this.m_metrics = new EnumBasedMetricsContainer(typeof(ThreadPoolProxyMetricsTracker.ms_metric), this.m_metricsService.MetricsConfig);
			this.InitMetrics();
		}

		// Token: 0x06002593 RID: 9619 RVA: 0x0009D82C File Offset: 0x0009BC2C
		public override IEnumerable<Metric> GetMetrics()
		{
			return this.m_metrics.GetMetrics();
		}

		// Token: 0x06002594 RID: 9620 RVA: 0x0009D83C File Offset: 0x0009BC3C
		private void InitMetrics()
		{
			string[] tags = new string[]
			{
				"server",
				Resources.ServerName
			};
			IEnumerator enumerator = Enum.GetValues(typeof(ThreadPoolProxyMetricsTracker.ms_metric)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object metricName = enumerator.Current;
					this.m_metrics.InitGauge(metricName, tags);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		// Token: 0x06002595 RID: 9621 RVA: 0x0009D8C4 File Offset: 0x0009BCC4
		public void Report()
		{
			if (!this.m_metricsService.Enabled)
			{
				return;
			}
			ThreadPoolProxy.Stats stats = ThreadPoolProxy.GetStats(true);
			using (this.m_metricsService.Synchronizer.BatchWrite())
			{
				DateTime now = DateTime.Now;
				this.m_metrics.Get<Gauge>(ThreadPoolProxyMetricsTracker.ms_metric.ms_thread_proxy_dispatched).Track(now, (long)stats.ItemsDispatched);
				this.m_metrics.Get<Gauge>(ThreadPoolProxyMetricsTracker.ms_metric.ms_thread_proxy_running).Track(now, (long)stats.ItemsRunning);
				this.m_metrics.Get<Gauge>(ThreadPoolProxyMetricsTracker.ms_metric.ms_thread_proxy_time_in_queue).Track(now, (long)stats.TimeInQueueTotalMs);
				this.m_metrics.Get<Gauge>(ThreadPoolProxyMetricsTracker.ms_metric.ms_thread_proxy_time_in_queue_peak).Track(now, (long)stats.TimeInQueuePeakMs);
			}
		}

		// Token: 0x0400130D RID: 4877
		private EnumBasedMetricsContainer m_metrics;

		// Token: 0x02000706 RID: 1798
		private enum ms_metric
		{
			// Token: 0x0400130F RID: 4879
			ms_thread_proxy_dispatched,
			// Token: 0x04001310 RID: 4880
			ms_thread_proxy_running,
			// Token: 0x04001311 RID: 4881
			ms_thread_proxy_time_in_queue,
			// Token: 0x04001312 RID: 4882
			ms_thread_proxy_time_in_queue_peak
		}
	}
}
