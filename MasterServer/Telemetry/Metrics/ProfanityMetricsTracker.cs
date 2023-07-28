using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using LibMetricsNet;
using MasterServer.Core;
using MasterServer.Core.Services.Metrics;
using MasterServer.Platform.ProfanityCheck;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x02000708 RID: 1800
	[Service]
	[Singleton]
	internal class ProfanityMetricsTracker : BaseTracker, IProfanityMetricsTracker, IMetricsProvider, IDisposable
	{
		// Token: 0x0600259B RID: 9627 RVA: 0x0009D9CC File Offset: 0x0009BDCC
		public override IEnumerable<Metric> GetMetrics()
		{
			object @lock = this.m_lock;
			IEnumerable<Metric> result;
			lock (@lock)
			{
				List<Metric> list = new List<Metric>();
				foreach (ProfanityMetricsTracker.ProfanityMetrics profanityMetrics in this.m_metrics.Values)
				{
					list.AddRange(from m in profanityMetrics.Metrics.GetMetrics()
					where m != null
					select m);
				}
				list.AddRange((from resultMetrics in this.m_resultMetrics
				select resultMetrics.Metric).Cast<Metric>());
				result = list.ToArray();
			}
			return result;
		}

		// Token: 0x0600259C RID: 9628 RVA: 0x0009DACC File Offset: 0x0009BECC
		public override void Dispose()
		{
		}

		// Token: 0x0600259D RID: 9629 RVA: 0x0009DAD0 File Offset: 0x0009BED0
		public void ReportProfanityRequest(ProfanityCheckService.CheckType checkType)
		{
			if (this.m_metricsService.Enabled)
			{
				ProfanityMetricsTracker.ProfanityMetrics metrics = this.GetMetrics(checkType);
				metrics.Metrics.Get<Meter>(ProfanityMetricsTracker.ProfanityMetricType.ms_profanity_request).Inc(DateTime.Now);
			}
		}

		// Token: 0x0600259E RID: 9630 RVA: 0x0009DB10 File Offset: 0x0009BF10
		public void ReportProfanityRequestTime(ProfanityCheckService.CheckType checkType, TimeSpan time)
		{
			if (this.m_metricsService.Enabled)
			{
				ProfanityMetricsTracker.ProfanityMetrics metrics = this.GetMetrics(checkType);
				metrics.Metrics.Get<Timer>(ProfanityMetricsTracker.ProfanityMetricType.ms_profanity_request_time).TimeMilliseconds(DateTime.Now, time);
			}
		}

		// Token: 0x0600259F RID: 9631 RVA: 0x0009DB54 File Offset: 0x0009BF54
		public void ReportProfanityRequestFailed(ProfanityCheckService.CheckType checkType)
		{
			if (this.m_metricsService.Enabled)
			{
				ProfanityMetricsTracker.ProfanityMetrics metrics = this.GetMetrics(checkType);
				metrics.Metrics.Get<Meter>(ProfanityMetricsTracker.ProfanityMetricType.ms_profanity_request_failed).Inc(DateTime.Now);
			}
		}

		// Token: 0x060025A0 RID: 9632 RVA: 0x0009DB94 File Offset: 0x0009BF94
		public void ReportProfanityResult(ProfanityCheckService.CheckType checkType, ProfanityCheckResult checkResult)
		{
			if (this.m_metricsService.Enabled)
			{
				ProfanityMetricsTracker.ProfanityResultMetrics resultMetrics = this.GetResultMetrics(checkType, checkResult);
				resultMetrics.Metric.Inc(DateTime.Now);
			}
		}

		// Token: 0x060025A1 RID: 9633 RVA: 0x0009DBCC File Offset: 0x0009BFCC
		private ProfanityMetricsTracker.ProfanityMetrics GetMetrics(ProfanityCheckService.CheckType checkType)
		{
			object @lock = this.m_lock;
			ProfanityMetricsTracker.ProfanityMetrics result;
			lock (@lock)
			{
				ProfanityMetricsTracker.ProfanityMetrics profanityMetrics;
				if (!this.m_metrics.TryGetValue(checkType, out profanityMetrics))
				{
					profanityMetrics = new ProfanityMetricsTracker.ProfanityMetrics(checkType, this.m_metricsService.MetricsConfig);
					this.m_metrics.Add(checkType, profanityMetrics);
				}
				result = profanityMetrics;
			}
			return result;
		}

		// Token: 0x060025A2 RID: 9634 RVA: 0x0009DC40 File Offset: 0x0009C040
		private ProfanityMetricsTracker.ProfanityResultMetrics GetResultMetrics(ProfanityCheckService.CheckType checkType, ProfanityCheckResult checkResult)
		{
			object @lock = this.m_lock;
			ProfanityMetricsTracker.ProfanityResultMetrics result;
			lock (@lock)
			{
				ProfanityMetricsTracker.ProfanityResultMetrics profanityResultMetrics = this.m_resultMetrics.FirstOrDefault((ProfanityMetricsTracker.ProfanityResultMetrics m) => m.CheckType == checkType && m.CheckResult == checkResult);
				if (profanityResultMetrics == null)
				{
					this.m_resultMetrics.Add(profanityResultMetrics = new ProfanityMetricsTracker.ProfanityResultMetrics(checkType, checkResult, this.m_metricsService.MetricsConfig));
				}
				result = profanityResultMetrics;
			}
			return result;
		}

		// Token: 0x04001313 RID: 4883
		private readonly Dictionary<ProfanityCheckService.CheckType, ProfanityMetricsTracker.ProfanityMetrics> m_metrics = new Dictionary<ProfanityCheckService.CheckType, ProfanityMetricsTracker.ProfanityMetrics>();

		// Token: 0x04001314 RID: 4884
		private readonly List<ProfanityMetricsTracker.ProfanityResultMetrics> m_resultMetrics = new List<ProfanityMetricsTracker.ProfanityResultMetrics>();

		// Token: 0x04001315 RID: 4885
		private readonly object m_lock = new object();

		// Token: 0x02000709 RID: 1801
		private enum ProfanityMetricType
		{
			// Token: 0x04001319 RID: 4889
			ms_profanity_request,
			// Token: 0x0400131A RID: 4890
			ms_profanity_request_time,
			// Token: 0x0400131B RID: 4891
			ms_profanity_request_failed,
			// Token: 0x0400131C RID: 4892
			ms_profanity_result
		}

		// Token: 0x0200070A RID: 1802
		private class ProfanityMetrics : IDisposable
		{
			// Token: 0x060025A5 RID: 9637 RVA: 0x0009DCF4 File Offset: 0x0009C0F4
			public ProfanityMetrics(ProfanityCheckService.CheckType checkType, MetricsConfig cfg)
			{
				this.Metrics = new EnumBasedMetricsContainer(typeof(ProfanityMetricsTracker.ProfanityMetricType), cfg);
				string[] tags = new string[]
				{
					"server",
					Resources.ServerName,
					"type",
					checkType.ToString()
				};
				this.Metrics.InitMeter(ProfanityMetricsTracker.ProfanityMetricType.ms_profanity_request, tags);
				this.Metrics.InitTimer(ProfanityMetricsTracker.ProfanityMetricType.ms_profanity_request_time, tags);
				this.Metrics.InitMeter(ProfanityMetricsTracker.ProfanityMetricType.ms_profanity_request_failed, tags);
			}

			// Token: 0x060025A6 RID: 9638 RVA: 0x0009DD82 File Offset: 0x0009C182
			public void Dispose()
			{
				if (this.Metrics != null)
				{
					this.Metrics.Dispose();
					this.Metrics = null;
				}
			}

			// Token: 0x0400131D RID: 4893
			public EnumBasedMetricsContainer Metrics;
		}

		// Token: 0x0200070B RID: 1803
		private class ProfanityResultMetrics : IDisposable
		{
			// Token: 0x060025A7 RID: 9639 RVA: 0x0009DDA4 File Offset: 0x0009C1A4
			public ProfanityResultMetrics(ProfanityCheckService.CheckType checkType, ProfanityCheckResult checkResult, MetricsConfig cfg)
			{
				this.CheckType = checkType;
				this.CheckResult = checkResult;
				string[] tags = new string[]
				{
					"server",
					Resources.ServerName,
					"type",
					checkType.ToString(),
					"result",
					checkResult.ToString()
				};
				string name = ProfanityMetricsTracker.ProfanityMetricType.ms_profanity_result.ToString();
				this.Metric = new Meter(name, tags, cfg.GetFilters(name));
			}

			// Token: 0x060025A8 RID: 9640 RVA: 0x0009DE30 File Offset: 0x0009C230
			public void Dispose()
			{
				if (this.Metric != null)
				{
					this.Metric.Dispose();
					this.Metric = null;
				}
			}

			// Token: 0x0400131E RID: 4894
			public readonly ProfanityCheckService.CheckType CheckType;

			// Token: 0x0400131F RID: 4895
			public readonly ProfanityCheckResult CheckResult;

			// Token: 0x04001320 RID: 4896
			public Meter Metric;
		}
	}
}
