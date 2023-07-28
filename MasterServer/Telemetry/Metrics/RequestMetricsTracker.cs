using System;
using System.Collections.Generic;
using System.Net;
using HK2Net;
using LibMetricsNet;
using MasterServer.Core;
using MasterServer.Core.Services.Metrics;
using MasterServer.Core.Web;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x02000714 RID: 1812
	[Service]
	[Singleton]
	internal class RequestMetricsTracker : BaseTracker, IRequestMetricsTracker, IMetricsProvider, IDisposable
	{
		// Token: 0x060025C9 RID: 9673 RVA: 0x0009E990 File Offset: 0x0009CD90
		public override IEnumerable<Metric> GetMetrics()
		{
			object @lock = this.m_lock;
			IEnumerable<Metric> result;
			lock (@lock)
			{
				int num = this.m_requestFailMetrics.Count + this.m_requestMetrics.Count * 3;
				Metric[] array = new Metric[num];
				int num2 = 0;
				foreach (RequestMetricsTracker.RequestMetrics requestMetrics in this.m_requestMetrics.Values)
				{
					Array.Copy(requestMetrics.Metrics.GetMetrics(), 0, array, num2, 3);
					num2 += 3;
				}
				foreach (RequestMetricsTracker.RequestFailMetrics requestFailMetrics in this.m_requestFailMetrics.Values)
				{
					array[num2++] = requestFailMetrics.ms_request_failed;
				}
				result = array;
			}
			return result;
		}

		// Token: 0x060025CA RID: 9674 RVA: 0x0009EAB8 File Offset: 0x0009CEB8
		public override void Dispose()
		{
		}

		// Token: 0x060025CB RID: 9675 RVA: 0x0009EABC File Offset: 0x0009CEBC
		private RequestMetricsTracker.RequestMetrics GetRequestMetrics(RequestDomain domain, string host, string path)
		{
			object @lock = this.m_lock;
			RequestMetricsTracker.RequestMetrics result;
			lock (@lock)
			{
				int hashCode = this.GetHashCode(domain, host, path);
				RequestMetricsTracker.RequestMetrics requestMetrics;
				if (!this.m_requestMetrics.TryGetValue(hashCode, out requestMetrics))
				{
					requestMetrics = new RequestMetricsTracker.RequestMetrics(domain, host, path, this.m_metricsService.MetricsConfig);
					this.m_requestMetrics.Add(hashCode, requestMetrics);
				}
				result = requestMetrics;
			}
			return result;
		}

		// Token: 0x060025CC RID: 9676 RVA: 0x0009EB3C File Offset: 0x0009CF3C
		private RequestMetricsTracker.RequestFailMetrics GetRequestFailMetrics(RequestDomain domain, string host, string path, HttpStatusCode statusCode)
		{
			object @lock = this.m_lock;
			RequestMetricsTracker.RequestFailMetrics result;
			lock (@lock)
			{
				RequestMetricsTracker.RequestFailMetrics requestFailMetrics;
				if (!this.m_requestFailMetrics.TryGetValue(statusCode, out requestFailMetrics))
				{
					requestFailMetrics = new RequestMetricsTracker.RequestFailMetrics(domain, host, path, statusCode, this.m_metricsService.MetricsConfig);
					this.m_requestFailMetrics.Add(statusCode, requestFailMetrics);
				}
				result = requestFailMetrics;
			}
			return result;
		}

		// Token: 0x060025CD RID: 9677 RVA: 0x0009EBB4 File Offset: 0x0009CFB4
		public void ReportHttpRequestCompleted(RequestDomain domain, string host, string path)
		{
			if (!this.m_metricsService.Enabled)
			{
				return;
			}
			RequestMetricsTracker.RequestMetrics requestMetrics = this.GetRequestMetrics(domain, host, path);
			requestMetrics.Metrics.Get<Meter>(RequestMetricsTracker.RequestMetricType.ms_request_completed).Inc(DateTime.Now);
		}

		// Token: 0x060025CE RID: 9678 RVA: 0x0009EBF8 File Offset: 0x0009CFF8
		public void ReportHttpRequestFailed(RequestDomain domain, string host, string path, HttpStatusCode statusCode)
		{
			if (!this.m_metricsService.Enabled)
			{
				return;
			}
			RequestMetricsTracker.RequestFailMetrics requestFailMetrics = this.GetRequestFailMetrics(domain, host, path, statusCode);
			requestFailMetrics.ms_request_failed.Inc(DateTime.Now);
		}

		// Token: 0x060025CF RID: 9679 RVA: 0x0009EC34 File Offset: 0x0009D034
		public void ReportHttpRequestCrashed(RequestDomain domain, string host, string path)
		{
			if (!this.m_metricsService.Enabled)
			{
				return;
			}
			RequestMetricsTracker.RequestMetrics requestMetrics = this.GetRequestMetrics(domain, host, path);
			requestMetrics.Metrics.Get<Meter>(RequestMetricsTracker.RequestMetricType.ms_request_crashed).Inc(DateTime.Now);
		}

		// Token: 0x060025D0 RID: 9680 RVA: 0x0009EC78 File Offset: 0x0009D078
		public void ReportHttpRequestTime(RequestDomain domain, string host, string path, TimeSpan time)
		{
			if (!this.m_metricsService.Enabled)
			{
				return;
			}
			RequestMetricsTracker.RequestMetrics requestMetrics = this.GetRequestMetrics(domain, host, path);
			requestMetrics.Metrics.Get<Timer>(RequestMetricsTracker.RequestMetricType.ms_request_time).TimeMilliseconds(DateTime.Now, time);
		}

		// Token: 0x060025D1 RID: 9681 RVA: 0x0009ECBD File Offset: 0x0009D0BD
		private int GetHashCode(RequestDomain domain, string host, string path)
		{
			return domain.GetHashCode() ^ host.GetHashCode() ^ path.GetHashCode();
		}

		// Token: 0x0400133C RID: 4924
		private readonly Dictionary<HttpStatusCode, RequestMetricsTracker.RequestFailMetrics> m_requestFailMetrics = new Dictionary<HttpStatusCode, RequestMetricsTracker.RequestFailMetrics>();

		// Token: 0x0400133D RID: 4925
		private readonly Dictionary<int, RequestMetricsTracker.RequestMetrics> m_requestMetrics = new Dictionary<int, RequestMetricsTracker.RequestMetrics>();

		// Token: 0x0400133E RID: 4926
		private readonly object m_lock = new object();

		// Token: 0x02000715 RID: 1813
		private enum RequestMetricType
		{
			// Token: 0x04001340 RID: 4928
			ms_request_completed,
			// Token: 0x04001341 RID: 4929
			ms_request_crashed,
			// Token: 0x04001342 RID: 4930
			ms_request_time
		}

		// Token: 0x02000716 RID: 1814
		private class RequestMetrics : IDisposable
		{
			// Token: 0x060025D2 RID: 9682 RVA: 0x0009ECDC File Offset: 0x0009D0DC
			public RequestMetrics(RequestDomain domain, string host, string path, MetricsConfig metricsConfig)
			{
				this.Metrics = new EnumBasedMetricsContainer(typeof(RequestMetricsTracker.RequestMetricType), metricsConfig);
				string[] tags = new string[]
				{
					"server",
					Resources.ServerName,
					"domain",
					domain.ToString(),
					"host",
					host,
					"path",
					path
				};
				this.Metrics.InitMeter(RequestMetricsTracker.RequestMetricType.ms_request_completed, tags);
				this.Metrics.InitMeter(RequestMetricsTracker.RequestMetricType.ms_request_crashed, tags);
				this.Metrics.InitTimer(RequestMetricsTracker.RequestMetricType.ms_request_time, tags);
			}

			// Token: 0x060025D3 RID: 9683 RVA: 0x0009ED83 File Offset: 0x0009D183
			public void Dispose()
			{
				if (this.Metrics != null)
				{
					this.Metrics.Dispose();
					this.Metrics = null;
				}
			}

			// Token: 0x04001343 RID: 4931
			public const int MetricsCount = 3;

			// Token: 0x04001344 RID: 4932
			public EnumBasedMetricsContainer Metrics;
		}

		// Token: 0x02000717 RID: 1815
		private class RequestFailMetrics : IDisposable
		{
			// Token: 0x060025D4 RID: 9684 RVA: 0x0009EDA4 File Offset: 0x0009D1A4
			public RequestFailMetrics(RequestDomain domain, string host, string path, HttpStatusCode statusCode, MetricsConfig metricsConfig)
			{
				string[] tags = new string[]
				{
					"server",
					Resources.ServerName,
					"domain",
					domain.ToString(),
					"host",
					host,
					"path",
					path,
					"code",
					statusCode.ToString()
				};
				this.ms_request_failed = new Meter("ms_request_failed", tags, metricsConfig.GetFilters("ms_request_failed"));
			}

			// Token: 0x060025D5 RID: 9685 RVA: 0x0009EE35 File Offset: 0x0009D235
			public void Dispose()
			{
				if (this.ms_request_failed != null)
				{
					this.ms_request_failed.Dispose();
					this.ms_request_failed = null;
				}
			}

			// Token: 0x04001345 RID: 4933
			public Meter ms_request_failed;
		}
	}
}
