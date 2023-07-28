using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using LibMetricsNet;
using MasterServer.Core;
using MasterServer.Core.Services.Metrics;
using MasterServer.GameLogic.VoucherSystem.VoucherProviders;
using Util.Common;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x020006DB RID: 1755
	[Service]
	[Singleton]
	internal class VoucherMetricsTracker : BaseTracker, IVoucherMetricsTracker, IMetricsProvider, IDisposable
	{
		// Token: 0x060024E1 RID: 9441 RVA: 0x0009A3D4 File Offset: 0x000987D4
		public override IEnumerable<Metric> GetMetrics()
		{
			object @lock = this.m_lock;
			IEnumerable<Metric> result;
			lock (@lock)
			{
				result = this.m_metrics.Values.SelectMany((VoucherMetricsTracker.VoucherMetrics ssm) => ssm.Metrics.GetMetrics()).ToArray<Metric>();
			}
			return result;
		}

		// Token: 0x060024E2 RID: 9442 RVA: 0x0009A448 File Offset: 0x00098848
		public override void Dispose()
		{
			this.m_metrics.ForEach(delegate(KeyValuePair<string, VoucherMetricsTracker.VoucherMetrics> m)
			{
				m.Value.Dispose();
			});
		}

		// Token: 0x060024E3 RID: 9443 RVA: 0x0009A474 File Offset: 0x00098874
		public void ReportRequest(VoucherRequest request)
		{
			if (this.m_metricsService.Enabled)
			{
				VoucherMetricsTracker.VoucherMetrics metrics = this.GetMetrics(request);
				metrics.Metrics.Get<Meter>(VoucherMetricsTracker.VoucherMetricType.ms_voucher_request).Inc(DateTime.Now);
			}
		}

		// Token: 0x060024E4 RID: 9444 RVA: 0x0009A4B4 File Offset: 0x000988B4
		public void ReportRequestTime(VoucherRequest request, TimeSpan time)
		{
			if (this.m_metricsService.Enabled)
			{
				VoucherMetricsTracker.VoucherMetrics metrics = this.GetMetrics(request);
				metrics.Metrics.Get<Timer>(VoucherMetricsTracker.VoucherMetricType.ms_voucher_request_time).TimeMilliseconds(DateTime.Now, time);
			}
		}

		// Token: 0x060024E5 RID: 9445 RVA: 0x0009A4F8 File Offset: 0x000988F8
		public void ReportRequestFailed(VoucherRequest request)
		{
			if (this.m_metricsService.Enabled)
			{
				VoucherMetricsTracker.VoucherMetrics metrics = this.GetMetrics(request);
				metrics.Metrics.Get<Meter>(VoucherMetricsTracker.VoucherMetricType.ms_voucher_request_failed).Inc(DateTime.Now);
			}
		}

		// Token: 0x060024E6 RID: 9446 RVA: 0x0009A538 File Offset: 0x00098938
		private VoucherMetricsTracker.VoucherMetrics GetMetrics(VoucherRequest request)
		{
			object @lock = this.m_lock;
			VoucherMetricsTracker.VoucherMetrics result;
			lock (@lock)
			{
				VoucherMetricsTracker.VoucherMetrics voucherMetrics;
				if (!this.m_metrics.TryGetValue(request.Name, out voucherMetrics))
				{
					voucherMetrics = new VoucherMetricsTracker.VoucherMetrics(request, this.m_metricsService.MetricsConfig);
					this.m_metrics.Add(request.Name, voucherMetrics);
				}
				result = voucherMetrics;
			}
			return result;
		}

		// Token: 0x040012A1 RID: 4769
		private readonly Dictionary<string, VoucherMetricsTracker.VoucherMetrics> m_metrics = new Dictionary<string, VoucherMetricsTracker.VoucherMetrics>();

		// Token: 0x040012A2 RID: 4770
		private readonly object m_lock = new object();

		// Token: 0x020006DC RID: 1756
		private enum VoucherMetricType
		{
			// Token: 0x040012A6 RID: 4774
			ms_voucher_request,
			// Token: 0x040012A7 RID: 4775
			ms_voucher_request_time,
			// Token: 0x040012A8 RID: 4776
			ms_voucher_request_failed
		}

		// Token: 0x020006DD RID: 1757
		private class VoucherMetrics : IDisposable
		{
			// Token: 0x060024E9 RID: 9449 RVA: 0x0009A5D0 File Offset: 0x000989D0
			public VoucherMetrics(VoucherRequest request, MetricsConfig cfg)
			{
				this.Metrics = new EnumBasedMetricsContainer(typeof(VoucherMetricsTracker.VoucherMetricType), cfg);
				string[] tags = new string[]
				{
					"request",
					request.Name,
					"server",
					Resources.ServerName
				};
				this.Metrics.InitMeter(VoucherMetricsTracker.VoucherMetricType.ms_voucher_request, tags);
				this.Metrics.InitTimer(VoucherMetricsTracker.VoucherMetricType.ms_voucher_request_time, tags);
				this.Metrics.InitMeter(VoucherMetricsTracker.VoucherMetricType.ms_voucher_request_failed, tags);
			}

			// Token: 0x17000395 RID: 917
			// (get) Token: 0x060024EA RID: 9450 RVA: 0x0009A657 File Offset: 0x00098A57
			// (set) Token: 0x060024EB RID: 9451 RVA: 0x0009A65F File Offset: 0x00098A5F
			public EnumBasedMetricsContainer Metrics { get; private set; }

			// Token: 0x060024EC RID: 9452 RVA: 0x0009A668 File Offset: 0x00098A68
			public void Dispose()
			{
				if (this.Metrics != null)
				{
					this.Metrics.Dispose();
					this.Metrics = null;
				}
			}
		}
	}
}
