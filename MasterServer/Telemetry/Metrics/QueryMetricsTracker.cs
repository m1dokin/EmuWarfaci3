using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HK2Net;
using LibMetricsNet;
using MasterServer.Core;
using MasterServer.Core.Services.Metrics;
using MasterServer.CryOnlineNET;
using MasterServer.Database;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x0200070D RID: 1805
	[Service]
	[Singleton]
	internal class QueryMetricsTracker : BaseTracker, IQueryMetricsTracker, IMetricsProvider, IDisposable
	{
		// Token: 0x060025AC RID: 9644 RVA: 0x0009DE7C File Offset: 0x0009C27C
		public QueryMetricsTracker(IQueryManager queryManager, IOnlineClient onlineClient)
		{
			this.m_queryManager = queryManager;
			this.m_onlineClient = onlineClient;
			this.m_queryManager.QueryCompleted += this.QueryManager_OnQueryCompleted;
			this.m_onlineClient.OnlineQueryStats += this.OnlineClient_OnOnlineQueryStats;
		}

		// Token: 0x060025AD RID: 9645 RVA: 0x0009DF06 File Offset: 0x0009C306
		public override void Dispose()
		{
			this.m_queryManager.QueryCompleted -= this.QueryManager_OnQueryCompleted;
			this.m_onlineClient.OnlineQueryStats -= this.OnlineClient_OnOnlineQueryStats;
		}

		// Token: 0x060025AE RID: 9646 RVA: 0x0009DF38 File Offset: 0x0009C338
		public override IEnumerable<Metric> GetMetrics()
		{
			object @lock = this.m_lock;
			IEnumerable<Metric> result;
			lock (@lock)
			{
				int num = this.m_queryMetrics.Values.Sum((List<QueryMetricsTracker.QueryMetrics> ml) => ml.Count);
				Metric[] array = new Metric[num * this.m_numMetrics];
				int num2 = 0;
				foreach (List<QueryMetricsTracker.QueryMetrics> list in this.m_queryMetrics.Values)
				{
					foreach (QueryMetricsTracker.QueryMetrics queryMetrics in list)
					{
						Array.Copy(queryMetrics.metrics.GetMetrics(), 0, array, num2++ * this.m_numMetrics, this.m_numMetrics);
					}
				}
				result = array;
			}
			return result;
		}

		// Token: 0x060025AF RID: 9647 RVA: 0x0009E070 File Offset: 0x0009C470
		private QueryMetricsTracker.QueryMetrics GetQueryMetrics(string queryName, QueryType queryType)
		{
			List<QueryMetricsTracker.QueryMetrics> list;
			if (!this.m_queryMetrics.TryGetValue(queryName, out list))
			{
				list = new List<QueryMetricsTracker.QueryMetrics>();
				this.m_queryMetrics.Add(queryName, list);
			}
			QueryMetricsTracker.QueryMetrics queryMetrics = list.Find((QueryMetricsTracker.QueryMetrics qm) => qm.QueryType == queryType);
			if (queryMetrics == null)
			{
				queryMetrics = new QueryMetricsTracker.QueryMetrics(queryName, queryType, this.m_metricsService.MetricsConfig);
				list.Add(queryMetrics);
			}
			return queryMetrics;
		}

		// Token: 0x060025B0 RID: 9648 RVA: 0x0009E0EC File Offset: 0x0009C4EC
		private void QueryManager_OnQueryCompleted(QueryContext ctx)
		{
			if (!this.m_metricsService.Enabled)
			{
				return;
			}
			object @lock = this.m_lock;
			EnumBasedMetricsContainer metrics;
			lock (@lock)
			{
				string queryName = (!this.IsOfInterest(ctx.Tag)) ? "ALL" : ctx.Tag;
				metrics = this.GetQueryMetrics(queryName, ctx.Type).metrics;
			}
			using (this.m_metricsService.Synchronizer.BatchWrite())
			{
				DateTime now = DateTime.Now;
				metrics.Get<Meter>(QueryMetricsTracker.ms_metric.ms_queries_executed).Inc(now);
				if (!ctx.Stats.Succeeded)
				{
					metrics.Get<Meter>(QueryMetricsTracker.ms_metric.ms_queries_failed).Inc(now);
				}
				metrics.Get<Meter>(QueryMetricsTracker.ms_metric.ms_query_dal_calls).Adjust(now, (long)ctx.Stats.DALCalls.Count);
				metrics.Get<Timer>(QueryMetricsTracker.ms_metric.ms_query_processing_time).TimeMilliseconds(now, ctx.Stats.ProcessingTime);
				TimeSpan delta = ctx.Stats.DALCalls.Aggregate(TimeSpan.Zero, (TimeSpan current, DALProxyStats dc) => current + dc.DALTime);
				metrics.Get<Timer>(QueryMetricsTracker.ms_metric.ms_query_dal_time).TimeMilliseconds(now, delta);
			}
		}

		// Token: 0x060025B1 RID: 9649 RVA: 0x0009E26C File Offset: 0x0009C66C
		private void OnlineClient_OnOnlineQueryStats(OnlineQueryStats stats)
		{
			if (!this.m_metricsService.Enabled)
			{
				return;
			}
			object @lock = this.m_lock;
			EnumBasedMetricsContainer metrics;
			lock (@lock)
			{
				string queryName = (!this.IsOfInterest(stats.Tag)) ? "ALL" : stats.Tag;
				metrics = this.GetQueryMetrics(queryName, stats.Type).metrics;
			}
			using (this.m_metricsService.Synchronizer.BatchWrite())
			{
				DateTime now = DateTime.Now;
				if (stats.Type == QueryType.Incoming_Request)
				{
					metrics.Get<Timer>(QueryMetricsTracker.ms_metric.ms_query_servicing_time).TimeMilliseconds(now, stats.ServicingTime);
				}
				metrics.Get<Meter>(QueryMetricsTracker.ms_metric.ms_query_traffic_up_compressed).Adjust(now, (long)((ulong)stats.OutboundCompressedSize));
				metrics.Get<Meter>(QueryMetricsTracker.ms_metric.ms_query_traffic_up_data).Adjust(now, (long)((ulong)stats.OutboundDataSize));
				metrics.Get<Meter>(QueryMetricsTracker.ms_metric.ms_query_traffic_down_compressed).Adjust(now, (long)((ulong)stats.InboundCompressedSize));
				metrics.Get<Meter>(QueryMetricsTracker.ms_metric.ms_query_traffic_down_data).Adjust(now, (long)((ulong)stats.InboundDataSize));
			}
		}

		// Token: 0x060025B2 RID: 9650 RVA: 0x0009E3B8 File Offset: 0x0009C7B8
		private bool IsOfInterest(string queryTag)
		{
			return this.m_queriesOfInterest.Any((QueryMetricsTracker.Pattern p) => p.pattern.IsMatch(queryTag));
		}

		// Token: 0x060025B3 RID: 9651 RVA: 0x0009E3EC File Offset: 0x0009C7EC
		public void AddInterest(string wildcard)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				string pattern = "^" + Regex.Escape(wildcard).Replace("\\*", ".*").Replace("\\?", ".") + "$";
				Regex pattern2 = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
				this.m_queriesOfInterest.Add(new QueryMetricsTracker.Pattern
				{
					wildcard = wildcard,
					pattern = pattern2
				});
			}
		}

		// Token: 0x060025B4 RID: 9652 RVA: 0x0009E490 File Offset: 0x0009C890
		public void RemoveInterest(int index)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_queriesOfInterest.RemoveAt(index);
				KeyValuePair<string, List<QueryMetricsTracker.QueryMetrics>>[] array = (from x in this.m_queryMetrics
				where x.Key != "ALL" && !this.IsOfInterest(x.Key)
				select x).ToArray<KeyValuePair<string, List<QueryMetricsTracker.QueryMetrics>>>();
				if (array.Length != 0)
				{
					foreach (KeyValuePair<string, List<QueryMetricsTracker.QueryMetrics>> keyValuePair in array)
					{
						this.m_queryMetrics.Remove(keyValuePair.Key);
						keyValuePair.Value.ForEach(delegate(QueryMetricsTracker.QueryMetrics qm)
						{
							qm.Dispose();
						});
					}
				}
			}
		}

		// Token: 0x060025B5 RID: 9653 RVA: 0x0009E56C File Offset: 0x0009C96C
		public List<string> GetInterests()
		{
			object @lock = this.m_lock;
			List<string> result;
			lock (@lock)
			{
				result = (from p in this.m_queriesOfInterest
				select p.wildcard).ToList<string>();
			}
			return result;
		}

		// Token: 0x04001321 RID: 4897
		private readonly IQueryManager m_queryManager;

		// Token: 0x04001322 RID: 4898
		private readonly IOnlineClient m_onlineClient;

		// Token: 0x04001323 RID: 4899
		private readonly int m_numMetrics = Enum.GetValues(typeof(QueryMetricsTracker.ms_metric)).Length;

		// Token: 0x04001324 RID: 4900
		private readonly Dictionary<string, List<QueryMetricsTracker.QueryMetrics>> m_queryMetrics = new Dictionary<string, List<QueryMetricsTracker.QueryMetrics>>();

		// Token: 0x04001325 RID: 4901
		private readonly object m_lock = new object();

		// Token: 0x04001326 RID: 4902
		private const string TOTAL_QUERY_NAME = "ALL";

		// Token: 0x04001327 RID: 4903
		private readonly List<QueryMetricsTracker.Pattern> m_queriesOfInterest = new List<QueryMetricsTracker.Pattern>();

		// Token: 0x0200070E RID: 1806
		private enum ms_metric
		{
			// Token: 0x0400132D RID: 4909
			ms_queries_executed,
			// Token: 0x0400132E RID: 4910
			ms_queries_failed,
			// Token: 0x0400132F RID: 4911
			ms_query_dal_calls,
			// Token: 0x04001330 RID: 4912
			ms_query_servicing_time,
			// Token: 0x04001331 RID: 4913
			ms_query_processing_time,
			// Token: 0x04001332 RID: 4914
			ms_query_dal_time,
			// Token: 0x04001333 RID: 4915
			ms_query_traffic_up_compressed,
			// Token: 0x04001334 RID: 4916
			ms_query_traffic_up_data,
			// Token: 0x04001335 RID: 4917
			ms_query_traffic_down_compressed,
			// Token: 0x04001336 RID: 4918
			ms_query_traffic_down_data
		}

		// Token: 0x0200070F RID: 1807
		private class QueryMetrics : IDisposable
		{
			// Token: 0x060025BB RID: 9659 RVA: 0x0009E62C File Offset: 0x0009CA2C
			public QueryMetrics(string queryName, QueryType queryType, MetricsConfig cfg)
			{
				this.QueryType = queryType;
				this.metrics = new EnumBasedMetricsContainer(typeof(QueryMetricsTracker.ms_metric), cfg);
				string[] tags = new string[]
				{
					"server",
					Resources.ServerName,
					"query",
					queryName,
					"query_type",
					queryType.ToString().ToLower()
				};
				this.metrics.InitMeter(QueryMetricsTracker.ms_metric.ms_queries_executed, tags);
				this.metrics.InitMeter(QueryMetricsTracker.ms_metric.ms_queries_failed, tags);
				this.metrics.InitMeter(QueryMetricsTracker.ms_metric.ms_query_dal_calls, tags);
				this.metrics.InitTimer(QueryMetricsTracker.ms_metric.ms_query_servicing_time, tags);
				this.metrics.InitTimer(QueryMetricsTracker.ms_metric.ms_query_processing_time, tags);
				this.metrics.InitTimer(QueryMetricsTracker.ms_metric.ms_query_dal_time, tags);
				this.metrics.InitMeter(QueryMetricsTracker.ms_metric.ms_query_traffic_up_compressed, tags);
				this.metrics.InitMeter(QueryMetricsTracker.ms_metric.ms_query_traffic_up_data, tags);
				this.metrics.InitMeter(QueryMetricsTracker.ms_metric.ms_query_traffic_down_compressed, tags);
				this.metrics.InitMeter(QueryMetricsTracker.ms_metric.ms_query_traffic_down_data, tags);
			}

			// Token: 0x1700039A RID: 922
			// (get) Token: 0x060025BC RID: 9660 RVA: 0x0009E751 File Offset: 0x0009CB51
			// (set) Token: 0x060025BD RID: 9661 RVA: 0x0009E759 File Offset: 0x0009CB59
			public QueryType QueryType { get; private set; }

			// Token: 0x060025BE RID: 9662 RVA: 0x0009E762 File Offset: 0x0009CB62
			public void Dispose()
			{
				this.metrics.Dispose();
			}

			// Token: 0x04001337 RID: 4919
			public readonly EnumBasedMetricsContainer metrics;
		}

		// Token: 0x02000710 RID: 1808
		private class Pattern
		{
			// Token: 0x04001339 RID: 4921
			public string wildcard;

			// Token: 0x0400133A RID: 4922
			public Regex pattern;
		}
	}
}
