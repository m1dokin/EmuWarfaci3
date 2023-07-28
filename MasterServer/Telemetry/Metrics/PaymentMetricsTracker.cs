using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using LibMetricsNet;
using MasterServer.Core;
using MasterServer.Core.Services.Metrics;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x02000701 RID: 1793
	[Service]
	[Singleton]
	internal class PaymentMetricsTracker : BaseTracker, IPaymentMetricsTracker, IMetricsProvider, IDisposable
	{
		// Token: 0x06002582 RID: 9602 RVA: 0x0009D538 File Offset: 0x0009B938
		public override IEnumerable<Metric> GetMetrics()
		{
			object @lock = this.m_lock;
			IEnumerable<Metric> result;
			lock (@lock)
			{
				result = this.m_paymentMetrics.Metrics.GetMetrics().ToArray<Metric>();
			}
			return result;
		}

		// Token: 0x06002583 RID: 9603 RVA: 0x0009D58C File Offset: 0x0009B98C
		public override void Init(IMetricsService service)
		{
			base.Init(service);
			this.m_paymentMetrics = new PaymentMetricsTracker.PaymentMetrics(this.m_metricsService.MetricsConfig);
		}

		// Token: 0x06002584 RID: 9604 RVA: 0x0009D5AC File Offset: 0x0009B9AC
		public override void Dispose()
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (this.m_paymentMetrics != null)
				{
					this.m_paymentMetrics.Dispose();
					this.m_paymentMetrics = null;
				}
			}
		}

		// Token: 0x06002585 RID: 9605 RVA: 0x0009D608 File Offset: 0x0009BA08
		public void ReportSpendMoneyRequest()
		{
			this.TryIncMeter(PaymentMetricsTracker.PaymentMetricType.ms_payment_spend_request);
		}

		// Token: 0x06002586 RID: 9606 RVA: 0x0009D611 File Offset: 0x0009BA11
		public void ReportSpendMoneyRequestTime(TimeSpan time)
		{
			this.TryReportTime(PaymentMetricsTracker.PaymentMetricType.ms_payment_spend_request_time, time);
		}

		// Token: 0x06002587 RID: 9607 RVA: 0x0009D61B File Offset: 0x0009BA1B
		public void ReportSpendMoneyRequestFailed()
		{
			this.TryIncMeter(PaymentMetricsTracker.PaymentMetricType.ms_payment_spend_request_failed);
		}

		// Token: 0x06002588 RID: 9608 RVA: 0x0009D624 File Offset: 0x0009BA24
		public void ReportGetMoneyRequest()
		{
			this.TryIncMeter(PaymentMetricsTracker.PaymentMetricType.ms_payment_get_request);
		}

		// Token: 0x06002589 RID: 9609 RVA: 0x0009D62D File Offset: 0x0009BA2D
		public void ReportGetMoneyRequestTime(TimeSpan time)
		{
			this.TryReportTime(PaymentMetricsTracker.PaymentMetricType.ms_payment_get_request_time, time);
		}

		// Token: 0x0600258A RID: 9610 RVA: 0x0009D637 File Offset: 0x0009BA37
		public void ReportGetMoneyRequestFailed()
		{
			this.TryIncMeter(PaymentMetricsTracker.PaymentMetricType.ms_payment_get_request_failed);
		}

		// Token: 0x0600258B RID: 9611 RVA: 0x0009D640 File Offset: 0x0009BA40
		private void TryIncMeter(PaymentMetricsTracker.PaymentMetricType metricType)
		{
			if (this.m_metricsService.Enabled)
			{
				using (this.m_metricsService.Synchronizer.BatchWrite())
				{
					DateTime now = DateTime.Now;
					this.m_paymentMetrics.Metrics.Get<Meter>(metricType).Inc(now);
				}
			}
		}

		// Token: 0x0600258C RID: 9612 RVA: 0x0009D6B4 File Offset: 0x0009BAB4
		private void TryReportTime(PaymentMetricsTracker.PaymentMetricType metricType, TimeSpan time)
		{
			if (this.m_metricsService.Enabled)
			{
				using (this.m_metricsService.Synchronizer.BatchWrite())
				{
					DateTime now = DateTime.Now;
					this.m_paymentMetrics.Metrics.Get<Timer>(metricType).TimeMilliseconds(now, time);
				}
			}
		}

		// Token: 0x04001303 RID: 4867
		private PaymentMetricsTracker.PaymentMetrics m_paymentMetrics;

		// Token: 0x04001304 RID: 4868
		private readonly object m_lock = new object();

		// Token: 0x02000702 RID: 1794
		private enum PaymentMetricType
		{
			// Token: 0x04001306 RID: 4870
			ms_payment_spend_request,
			// Token: 0x04001307 RID: 4871
			ms_payment_spend_request_time,
			// Token: 0x04001308 RID: 4872
			ms_payment_spend_request_failed,
			// Token: 0x04001309 RID: 4873
			ms_payment_get_request,
			// Token: 0x0400130A RID: 4874
			ms_payment_get_request_time,
			// Token: 0x0400130B RID: 4875
			ms_payment_get_request_failed
		}

		// Token: 0x02000703 RID: 1795
		private class PaymentMetrics : IDisposable
		{
			// Token: 0x0600258D RID: 9613 RVA: 0x0009D728 File Offset: 0x0009BB28
			public PaymentMetrics(MetricsConfig cfg)
			{
				this.Metrics = new EnumBasedMetricsContainer(typeof(PaymentMetricsTracker.PaymentMetricType), cfg);
				string[] tags = new string[]
				{
					"server",
					Resources.ServerName
				};
				this.Metrics.InitMeter(PaymentMetricsTracker.PaymentMetricType.ms_payment_spend_request, tags);
				this.Metrics.InitTimer(PaymentMetricsTracker.PaymentMetricType.ms_payment_spend_request_time, tags);
				this.Metrics.InitMeter(PaymentMetricsTracker.PaymentMetricType.ms_payment_spend_request_failed, tags);
				this.Metrics.InitMeter(PaymentMetricsTracker.PaymentMetricType.ms_payment_get_request, tags);
				this.Metrics.InitTimer(PaymentMetricsTracker.PaymentMetricType.ms_payment_get_request_time, tags);
				this.Metrics.InitMeter(PaymentMetricsTracker.PaymentMetricType.ms_payment_get_request_failed, tags);
			}

			// Token: 0x0600258E RID: 9614 RVA: 0x0009D7D4 File Offset: 0x0009BBD4
			public void Dispose()
			{
				if (this.Metrics != null)
				{
					this.Metrics.Dispose();
					this.Metrics = null;
				}
			}

			// Token: 0x0400130C RID: 4876
			public EnumBasedMetricsContainer Metrics;
		}
	}
}
