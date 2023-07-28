using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using LibMetricsNet;
using MasterServer.Core;
using MasterServer.Core.Services.Metrics;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x0200071E RID: 1822
	[Service]
	[Singleton]
	internal class MapVotingTracker : BaseTracker, IMapVoting, IMetricsProvider, IDisposable
	{
		// Token: 0x060025E8 RID: 9704 RVA: 0x0009F168 File Offset: 0x0009D568
		public override void Dispose()
		{
			if (this.m_mapVotingMetrics != null)
			{
				this.m_mapVotingMetrics.Dispose();
				this.m_mapVotingMetrics = null;
			}
		}

		// Token: 0x060025E9 RID: 9705 RVA: 0x0009F188 File Offset: 0x0009D588
		public override IEnumerable<Metric> GetMetrics()
		{
			object @lock = this.m_lock;
			IEnumerable<Metric> result;
			lock (@lock)
			{
				result = this.m_mapVotingMetrics.Metrics.GetMetrics().ToArray<Metric>();
			}
			return result;
		}

		// Token: 0x060025EA RID: 9706 RVA: 0x0009F1DC File Offset: 0x0009D5DC
		public override void Init(IMetricsService service)
		{
			base.Init(service);
			this.m_mapVotingMetrics = new MapVotingTracker.MapVotingMetrics(this.m_metricsService.MetricsConfig);
		}

		// Token: 0x060025EB RID: 9707 RVA: 0x0009F1FB File Offset: 0x0009D5FB
		public void ReportVotesAfterVotingFinished()
		{
			this.TryIncMeter(MapVotingTracker.MapVotingMetricType.ms_voting_missed);
		}

		// Token: 0x060025EC RID: 9708 RVA: 0x0009F204 File Offset: 0x0009D604
		private void TryIncMeter(MapVotingTracker.MapVotingMetricType type)
		{
			DateTime now = DateTime.Now;
			this.m_mapVotingMetrics.Metrics.Get<Meter>(type).Inc(now);
		}

		// Token: 0x04001352 RID: 4946
		private MapVotingTracker.MapVotingMetrics m_mapVotingMetrics;

		// Token: 0x04001353 RID: 4947
		private readonly object m_lock = new object();

		// Token: 0x0200071F RID: 1823
		private enum MapVotingMetricType
		{
			// Token: 0x04001355 RID: 4949
			ms_voting_missed
		}

		// Token: 0x02000720 RID: 1824
		private class MapVotingMetrics : IDisposable
		{
			// Token: 0x060025ED RID: 9709 RVA: 0x0009F234 File Offset: 0x0009D634
			public MapVotingMetrics(MetricsConfig config)
			{
				this.Metrics = new EnumBasedMetricsContainer(typeof(MapVotingTracker.MapVotingMetricType), config);
				string[] tags = new string[]
				{
					"server",
					Resources.ServerName
				};
				this.Metrics.InitMeter(MapVotingTracker.MapVotingMetricType.ms_voting_missed, tags);
			}

			// Token: 0x060025EE RID: 9710 RVA: 0x0009F286 File Offset: 0x0009D686
			public void Dispose()
			{
				if (this.Metrics != null)
				{
					this.Metrics.Dispose();
					this.Metrics = null;
				}
			}

			// Token: 0x04001356 RID: 4950
			public EnumBasedMetricsContainer Metrics;
		}
	}
}
