using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using LibMetricsNet;
using MasterServer.Core;
using MasterServer.Core.Services.Metrics;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x020006D1 RID: 1745
	[Service]
	[Singleton]
	internal class ProcessingQueueMetricsTracker : BaseTracker, IProcessingQueueMetricsTracker, IMetricsProvider, IDisposable
	{
		// Token: 0x060024B8 RID: 9400 RVA: 0x00099C69 File Offset: 0x00098069
		public override IEnumerable<Metric> GetMetrics()
		{
			return this.m_metrics.Values.SelectMany((ProcessingQueueMetricsTracker.ProcessingQueueMetrics x) => x.Metrics.GetMetrics()).ToArray<Metric>();
		}

		// Token: 0x060024B9 RID: 9401 RVA: 0x00099CA0 File Offset: 0x000980A0
		public override void Dispose()
		{
			ProcessingQueueMetricsTracker.ProcessingQueueMetrics[] array = this.m_metrics.Values.ToArray<ProcessingQueueMetricsTracker.ProcessingQueueMetrics>();
			this.m_metrics.Clear();
			Array.ForEach<ProcessingQueueMetricsTracker.ProcessingQueueMetrics>(array, delegate(ProcessingQueueMetricsTracker.ProcessingQueueMetrics x)
			{
				x.Dispose();
			});
		}

		// Token: 0x060024BA RID: 9402 RVA: 0x00099CEC File Offset: 0x000980EC
		public void ReportConsumed(string queueName)
		{
			if (!this.m_metricsService.Enabled)
			{
				return;
			}
			EnumBasedMetricsContainer metrics = this.GetMetrics(queueName);
			metrics.Get<Meter>(ProcessingQueueMetricsTracker.ProcessingQueueMetricType.ms_processing_queue_consumed).Inc(DateTime.Now);
		}

		// Token: 0x060024BB RID: 9403 RVA: 0x00099D28 File Offset: 0x00098128
		public void ReportSkipped(string queueName)
		{
			if (!this.m_metricsService.Enabled)
			{
				return;
			}
			EnumBasedMetricsContainer metrics = this.GetMetrics(queueName);
			metrics.Get<Meter>(ProcessingQueueMetricsTracker.ProcessingQueueMetricType.ms_processing_queue_skipped).Inc(DateTime.Now);
		}

		// Token: 0x060024BC RID: 9404 RVA: 0x00099D64 File Offset: 0x00098164
		public void ReportProcessingTime(string queueName, TimeSpan elapsed)
		{
			if (!this.m_metricsService.Enabled)
			{
				return;
			}
			EnumBasedMetricsContainer metrics = this.GetMetrics(queueName);
			metrics.Get<Timer>(ProcessingQueueMetricsTracker.ProcessingQueueMetricType.ms_processing_queue_processing_time).TimeMilliseconds(DateTime.Now, elapsed);
		}

		// Token: 0x060024BD RID: 9405 RVA: 0x00099DA1 File Offset: 0x000981A1
		private EnumBasedMetricsContainer GetMetrics(string queueName)
		{
			return this.m_metrics.GetOrAdd(queueName, (string name) => new ProcessingQueueMetricsTracker.ProcessingQueueMetrics(name, this.m_metricsService.MetricsConfig)).Metrics;
		}

		// Token: 0x0400128A RID: 4746
		private readonly ConcurrentDictionary<string, ProcessingQueueMetricsTracker.ProcessingQueueMetrics> m_metrics = new ConcurrentDictionary<string, ProcessingQueueMetricsTracker.ProcessingQueueMetrics>();

		// Token: 0x020006D2 RID: 1746
		private enum ProcessingQueueMetricType
		{
			// Token: 0x0400128E RID: 4750
			ms_processing_queue_consumed,
			// Token: 0x0400128F RID: 4751
			ms_processing_queue_skipped,
			// Token: 0x04001290 RID: 4752
			ms_processing_queue_processing_time
		}

		// Token: 0x020006D3 RID: 1747
		private class ProcessingQueueMetrics : IDisposable
		{
			// Token: 0x060024C1 RID: 9409 RVA: 0x00099DE8 File Offset: 0x000981E8
			public ProcessingQueueMetrics(string queueName, MetricsConfig metricsConfig)
			{
				this.Metrics = new EnumBasedMetricsContainer(typeof(ProcessingQueueMetricsTracker.ProcessingQueueMetricType), metricsConfig);
				string[] tags = new string[]
				{
					"server",
					Resources.ServerName,
					"queue",
					queueName
				};
				this.Metrics.InitMeter(ProcessingQueueMetricsTracker.ProcessingQueueMetricType.ms_processing_queue_consumed, tags);
				this.Metrics.InitMeter(ProcessingQueueMetricsTracker.ProcessingQueueMetricType.ms_processing_queue_skipped, tags);
				this.Metrics.InitTimer(ProcessingQueueMetricsTracker.ProcessingQueueMetricType.ms_processing_queue_processing_time, tags);
			}

			// Token: 0x17000392 RID: 914
			// (get) Token: 0x060024C2 RID: 9410 RVA: 0x00099E6A File Offset: 0x0009826A
			// (set) Token: 0x060024C3 RID: 9411 RVA: 0x00099E72 File Offset: 0x00098272
			public EnumBasedMetricsContainer Metrics { get; private set; }

			// Token: 0x060024C4 RID: 9412 RVA: 0x00099E7B File Offset: 0x0009827B
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
