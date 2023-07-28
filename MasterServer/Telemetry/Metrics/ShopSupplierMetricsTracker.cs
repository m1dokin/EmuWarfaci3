using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HK2Net;
using LibMetricsNet;
using MasterServer.Core;
using MasterServer.Core.Services.Metrics;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x0200071A RID: 1818
	[Service]
	[Singleton]
	internal class ShopSupplierMetricsTracker : BaseTracker, IShopSupplierMetricsTracker, IMetricsProvider, IDisposable
	{
		// Token: 0x060025DC RID: 9692 RVA: 0x0009EEB0 File Offset: 0x0009D2B0
		public override IEnumerable<Metric> GetMetrics()
		{
			object @lock = this.m_lock;
			IEnumerable<Metric> result;
			lock (@lock)
			{
				result = this.m_metrics.Values.SelectMany((ShopSupplierMetricsTracker.ShopSupplierMetrics ssm) => ssm.Metrics.GetMetrics()).ToArray<Metric>();
			}
			return result;
		}

		// Token: 0x060025DD RID: 9693 RVA: 0x0009EF24 File Offset: 0x0009D324
		public override void Dispose()
		{
		}

		// Token: 0x060025DE RID: 9694 RVA: 0x0009EF28 File Offset: 0x0009D328
		public void ReportRequest(ShopSupplierRequest request, int shopSupplierId)
		{
			if (this.m_metricsService.Enabled)
			{
				ShopSupplierMetricsTracker.ShopSupplierMetrics metrics = this.GetMetrics(request, shopSupplierId);
				metrics.Metrics.Get<Meter>(ShopSupplierMetricsTracker.ShopSupplierMetricType.ms_external_shop_request).Inc(DateTime.Now);
			}
		}

		// Token: 0x060025DF RID: 9695 RVA: 0x0009EF6C File Offset: 0x0009D36C
		public void ReportRequestTime(ShopSupplierRequest request, int shopSupplierId, TimeSpan time)
		{
			if (this.m_metricsService.Enabled)
			{
				ShopSupplierMetricsTracker.ShopSupplierMetrics metrics = this.GetMetrics(request, shopSupplierId);
				metrics.Metrics.Get<Timer>(ShopSupplierMetricsTracker.ShopSupplierMetricType.ms_external_shop_request_time).TimeMilliseconds(DateTime.Now, time);
			}
		}

		// Token: 0x060025E0 RID: 9696 RVA: 0x0009EFB0 File Offset: 0x0009D3B0
		public void ReportRequestFailed(ShopSupplierRequest request, int shopSupplierId)
		{
			if (this.m_metricsService.Enabled)
			{
				ShopSupplierMetricsTracker.ShopSupplierMetrics metrics = this.GetMetrics(request, shopSupplierId);
				metrics.Metrics.Get<Meter>(ShopSupplierMetricsTracker.ShopSupplierMetricType.ms_external_shop_request_failed).Inc(DateTime.Now);
			}
		}

		// Token: 0x060025E1 RID: 9697 RVA: 0x0009EFF4 File Offset: 0x0009D3F4
		private ShopSupplierMetricsTracker.ShopSupplierMetrics GetMetrics(ShopSupplierRequest request, int shopSupplierId)
		{
			object @lock = this.m_lock;
			ShopSupplierMetricsTracker.ShopSupplierMetrics result;
			lock (@lock)
			{
				int hashCode = this.GetHashCode(request, shopSupplierId);
				ShopSupplierMetricsTracker.ShopSupplierMetrics shopSupplierMetrics;
				if (!this.m_metrics.TryGetValue(hashCode, out shopSupplierMetrics))
				{
					shopSupplierMetrics = new ShopSupplierMetricsTracker.ShopSupplierMetrics(request, shopSupplierId, this.m_metricsService.MetricsConfig);
					this.m_metrics.Add(hashCode, shopSupplierMetrics);
				}
				result = shopSupplierMetrics;
			}
			return result;
		}

		// Token: 0x060025E2 RID: 9698 RVA: 0x0009F074 File Offset: 0x0009D474
		private int GetHashCode(ShopSupplierRequest request, int shopSupplierId)
		{
			return request.GetHashCode() ^ shopSupplierId.GetHashCode();
		}

		// Token: 0x0400134A RID: 4938
		private readonly Dictionary<int, ShopSupplierMetricsTracker.ShopSupplierMetrics> m_metrics = new Dictionary<int, ShopSupplierMetricsTracker.ShopSupplierMetrics>();

		// Token: 0x0400134B RID: 4939
		private readonly object m_lock = new object();

		// Token: 0x0200071B RID: 1819
		private enum ShopSupplierMetricType
		{
			// Token: 0x0400134E RID: 4942
			ms_external_shop_request,
			// Token: 0x0400134F RID: 4943
			ms_external_shop_request_time,
			// Token: 0x04001350 RID: 4944
			ms_external_shop_request_failed
		}

		// Token: 0x0200071C RID: 1820
		private class ShopSupplierMetrics : IDisposable
		{
			// Token: 0x060025E4 RID: 9700 RVA: 0x0009F098 File Offset: 0x0009D498
			public ShopSupplierMetrics(ShopSupplierRequest request, int shopSupplierId, MetricsConfig cfg)
			{
				this.Metrics = new EnumBasedMetricsContainer(typeof(ShopSupplierMetricsTracker.ShopSupplierMetricType), cfg);
				string[] tags = new string[]
				{
					"request",
					request.Name,
					"supplier_id",
					shopSupplierId.ToString(CultureInfo.InvariantCulture),
					"server",
					Resources.ServerName
				};
				this.Metrics.InitMeter(ShopSupplierMetricsTracker.ShopSupplierMetricType.ms_external_shop_request, tags);
				this.Metrics.InitTimer(ShopSupplierMetricsTracker.ShopSupplierMetricType.ms_external_shop_request_time, tags);
				this.Metrics.InitMeter(ShopSupplierMetricsTracker.ShopSupplierMetricType.ms_external_shop_request_failed, tags);
			}

			// Token: 0x060025E5 RID: 9701 RVA: 0x0009F136 File Offset: 0x0009D536
			public void Dispose()
			{
				if (this.Metrics != null)
				{
					this.Metrics.Dispose();
					this.Metrics = null;
				}
			}

			// Token: 0x04001351 RID: 4945
			public EnumBasedMetricsContainer Metrics;
		}
	}
}
