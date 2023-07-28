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
	// Token: 0x020006EE RID: 1774
	[Service]
	[Singleton]
	internal class DalMetricsTracker : BaseTracker, IDalMetricsTracker, IMetricsProvider, IDisposable
	{
		// Token: 0x06002541 RID: 9537 RVA: 0x0009BE14 File Offset: 0x0009A214
		public DalMetricsTracker(IQueryManager queryManager)
		{
			this.m_queryManager = queryManager;
			this.m_queryManager.QueryCompleted += this.OnQueryCompleted;
		}

		// Token: 0x06002542 RID: 9538 RVA: 0x0009BE80 File Offset: 0x0009A280
		public override void Dispose()
		{
			this.m_queryManager.QueryCompleted -= this.OnQueryCompleted;
		}

		// Token: 0x06002543 RID: 9539 RVA: 0x0009BE9C File Offset: 0x0009A29C
		public override IEnumerable<Metric> GetMetrics()
		{
			object @lock = this.m_lock;
			IEnumerable<Metric> result;
			lock (@lock)
			{
				Metric[] array = new Metric[this.m_dalMetrics.Count * this.m_numMetrics];
				int num = 0;
				foreach (DalMetricsTracker.DalMetrics dalMetrics in this.m_dalMetrics.Values)
				{
					Array.Copy(dalMetrics.metrics.GetMetrics(), 0, array, num++ * this.m_numMetrics, this.m_numMetrics);
				}
				result = array;
			}
			return result;
		}

		// Token: 0x06002544 RID: 9540 RVA: 0x0009BF68 File Offset: 0x0009A368
		private void OnQueryCompleted(QueryContext ctx)
		{
			if (!this.m_metricsService.Enabled || ctx.Stats.DALCalls.Count == 0)
			{
				return;
			}
			List<EnumBasedMetricsContainer> list = new List<EnumBasedMetricsContainer>(ctx.Stats.DALCalls.Count);
			object @lock = this.m_lock;
			lock (@lock)
			{
				foreach (DALProxyStats dalproxyStats in ctx.Stats.DALCalls)
				{
					string procedure = (!this.IsOfInterest(dalproxyStats.Procedure)) ? "ALL" : dalproxyStats.Procedure;
					list.Add(this.GetDalMetrics(procedure).metrics);
				}
			}
			using (this.m_metricsService.Synchronizer.BatchWrite())
			{
				DateTime now = DateTime.Now;
				for (int num = 0; num != list.Count; num++)
				{
					DALProxyStats dalproxyStats2 = ctx.Stats.DALCalls[num];
					EnumBasedMetricsContainer enumBasedMetricsContainer = list[num];
					enumBasedMetricsContainer.Get<Meter>(DalMetricsTracker.ms_metric.ms_dal_proc_calls).Inc(now);
					enumBasedMetricsContainer.Get<Meter>(DalMetricsTracker.ms_metric.ms_dal_proc_db_queries).Adjust(now, (long)dalproxyStats2.DBQueries);
					enumBasedMetricsContainer.Get<Meter>(DalMetricsTracker.ms_metric.ms_dal_proc_db_deadlocks).Adjust(now, (long)dalproxyStats2.DBDeadlocks);
					enumBasedMetricsContainer.Get<Meter>(DalMetricsTracker.ms_metric.ms_dal_proc_db_deadlocks_total).Adjust(now, (long)dalproxyStats2.DBDeadlocksTotal);
					enumBasedMetricsContainer.Get<Meter>(DalMetricsTracker.ms_metric.ms_dal_proc_cache_hits_l1).Adjust(now, (long)dalproxyStats2.L1CacheHits);
					enumBasedMetricsContainer.Get<Meter>(DalMetricsTracker.ms_metric.ms_dal_proc_cache_misses_l1).Adjust(now, (long)dalproxyStats2.L1CacheMisses);
					enumBasedMetricsContainer.Get<Meter>(DalMetricsTracker.ms_metric.ms_dal_proc_cache_clear_l1).Adjust(now, (long)dalproxyStats2.L1CacheClear);
					enumBasedMetricsContainer.Get<Meter>(DalMetricsTracker.ms_metric.ms_dal_proc_cache_hits_l2).Adjust(now, (long)dalproxyStats2.L2CacheHits);
					enumBasedMetricsContainer.Get<Meter>(DalMetricsTracker.ms_metric.ms_dal_proc_cache_misses_l2).Adjust(now, (long)dalproxyStats2.L2CacheMisses);
					enumBasedMetricsContainer.Get<Meter>(DalMetricsTracker.ms_metric.ms_dal_proc_cache_clear_l2).Adjust(now, (long)dalproxyStats2.L2CacheClear);
					enumBasedMetricsContainer.Get<Timer>(DalMetricsTracker.ms_metric.ms_dal_proc_db_time).TimeMilliseconds(now, dalproxyStats2.DBTime);
					enumBasedMetricsContainer.Get<Timer>(DalMetricsTracker.ms_metric.ms_dal_proc_db_connect_time).TimeMilliseconds(now, dalproxyStats2.ConnectionAllocTime);
					enumBasedMetricsContainer.Get<Timer>(DalMetricsTracker.ms_metric.ms_dal_proc_cache_time).TimeMilliseconds(now, dalproxyStats2.CacheTime);
				}
			}
			try
			{
				if (list.Count < ctx.Stats.DALCalls.Count)
				{
					for (int i = list.Count; i < ctx.Stats.DALCalls.Count; i++)
					{
						DALProxyStats arg = ctx.Stats.DALCalls[i];
						Log.Warning(string.Format("There is unexpected DAL call, procedure: {0}, for query {1}", arg, ctx.Tag));
					}
				}
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		// Token: 0x06002545 RID: 9541 RVA: 0x0009C2F8 File Offset: 0x0009A6F8
		private DalMetricsTracker.DalMetrics GetDalMetrics(string procedure)
		{
			DalMetricsTracker.DalMetrics dalMetrics;
			if (!this.m_dalMetrics.TryGetValue(procedure, out dalMetrics))
			{
				dalMetrics = new DalMetricsTracker.DalMetrics(procedure, this.m_metricsService.MetricsConfig);
				this.m_dalMetrics.Add(procedure, dalMetrics);
			}
			return dalMetrics;
		}

		// Token: 0x06002546 RID: 9542 RVA: 0x0009C338 File Offset: 0x0009A738
		private bool IsOfInterest(string queryTag)
		{
			return this.m_procsOfInterest.Any((DalMetricsTracker.Pattern p) => p.pattern.IsMatch(queryTag));
		}

		// Token: 0x06002547 RID: 9543 RVA: 0x0009C36C File Offset: 0x0009A76C
		public void AddInterest(string wildcard)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				string pattern = "^" + Regex.Escape(wildcard).Replace("\\*", ".*").Replace("\\?", ".") + "$";
				Regex pattern2 = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
				this.m_procsOfInterest.Add(new DalMetricsTracker.Pattern
				{
					wildcard = wildcard,
					pattern = pattern2
				});
			}
		}

		// Token: 0x06002548 RID: 9544 RVA: 0x0009C410 File Offset: 0x0009A810
		public void RemoveInterest(int index)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_procsOfInterest.RemoveAt(index);
				KeyValuePair<string, DalMetricsTracker.DalMetrics>[] array = (from x in this.m_dalMetrics
				where x.Key != "ALL" && !this.IsOfInterest(x.Key)
				select x).ToArray<KeyValuePair<string, DalMetricsTracker.DalMetrics>>();
				if (array.Length != 0)
				{
					foreach (KeyValuePair<string, DalMetricsTracker.DalMetrics> keyValuePair in array)
					{
						this.m_dalMetrics.Remove(keyValuePair.Key);
						keyValuePair.Value.Dispose();
					}
				}
			}
		}

		// Token: 0x06002549 RID: 9545 RVA: 0x0009C4D0 File Offset: 0x0009A8D0
		public List<string> GetInterests()
		{
			object @lock = this.m_lock;
			List<string> result;
			lock (@lock)
			{
				result = (from p in this.m_procsOfInterest
				select p.wildcard).ToList<string>();
			}
			return result;
		}

		// Token: 0x040012CC RID: 4812
		private readonly IQueryManager m_queryManager;

		// Token: 0x040012CD RID: 4813
		private readonly int m_numMetrics = Enum.GetValues(typeof(DalMetricsTracker.ms_metric)).Length;

		// Token: 0x040012CE RID: 4814
		private readonly Dictionary<string, DalMetricsTracker.DalMetrics> m_dalMetrics = new Dictionary<string, DalMetricsTracker.DalMetrics>();

		// Token: 0x040012CF RID: 4815
		private readonly object m_lock = new object();

		// Token: 0x040012D0 RID: 4816
		private const string TOTAL_PROC_NAME = "ALL";

		// Token: 0x040012D1 RID: 4817
		private readonly List<DalMetricsTracker.Pattern> m_procsOfInterest = new List<DalMetricsTracker.Pattern>();

		// Token: 0x020006EF RID: 1775
		private enum ms_metric
		{
			// Token: 0x040012D4 RID: 4820
			ms_dal_proc_calls,
			// Token: 0x040012D5 RID: 4821
			ms_dal_proc_db_queries,
			// Token: 0x040012D6 RID: 4822
			ms_dal_proc_cache_hits_l1,
			// Token: 0x040012D7 RID: 4823
			ms_dal_proc_cache_misses_l1,
			// Token: 0x040012D8 RID: 4824
			ms_dal_proc_cache_clear_l1,
			// Token: 0x040012D9 RID: 4825
			ms_dal_proc_cache_hits_l2,
			// Token: 0x040012DA RID: 4826
			ms_dal_proc_cache_misses_l2,
			// Token: 0x040012DB RID: 4827
			ms_dal_proc_cache_clear_l2,
			// Token: 0x040012DC RID: 4828
			ms_dal_proc_db_time,
			// Token: 0x040012DD RID: 4829
			ms_dal_proc_db_connect_time,
			// Token: 0x040012DE RID: 4830
			ms_dal_proc_cache_time,
			// Token: 0x040012DF RID: 4831
			ms_dal_proc_db_deadlocks,
			// Token: 0x040012E0 RID: 4832
			ms_dal_proc_db_deadlocks_total
		}

		// Token: 0x020006F0 RID: 1776
		private class DalMetrics : IDisposable
		{
			// Token: 0x0600254C RID: 9548 RVA: 0x0009C570 File Offset: 0x0009A970
			public DalMetrics(string procedure, MetricsConfig cfg)
			{
				this.metrics = new EnumBasedMetricsContainer(typeof(DalMetricsTracker.ms_metric), cfg);
				string[] tags = new string[]
				{
					"server",
					Resources.ServerName,
					"procedure",
					procedure
				};
				this.metrics.InitMeter(DalMetricsTracker.ms_metric.ms_dal_proc_calls, tags);
				this.metrics.InitMeter(DalMetricsTracker.ms_metric.ms_dal_proc_db_queries, tags);
				this.metrics.InitMeter(DalMetricsTracker.ms_metric.ms_dal_proc_db_deadlocks, tags);
				this.metrics.InitMeter(DalMetricsTracker.ms_metric.ms_dal_proc_db_deadlocks_total, tags);
				this.metrics.InitMeter(DalMetricsTracker.ms_metric.ms_dal_proc_cache_hits_l1, tags);
				this.metrics.InitMeter(DalMetricsTracker.ms_metric.ms_dal_proc_cache_misses_l1, tags);
				this.metrics.InitMeter(DalMetricsTracker.ms_metric.ms_dal_proc_cache_clear_l1, tags);
				this.metrics.InitMeter(DalMetricsTracker.ms_metric.ms_dal_proc_cache_hits_l2, tags);
				this.metrics.InitMeter(DalMetricsTracker.ms_metric.ms_dal_proc_cache_misses_l2, tags);
				this.metrics.InitMeter(DalMetricsTracker.ms_metric.ms_dal_proc_cache_clear_l2, tags);
				this.metrics.InitTimer(DalMetricsTracker.ms_metric.ms_dal_proc_db_time, tags);
				this.metrics.InitTimer(DalMetricsTracker.ms_metric.ms_dal_proc_db_connect_time, tags);
				this.metrics.InitTimer(DalMetricsTracker.ms_metric.ms_dal_proc_cache_time, tags);
			}

			// Token: 0x0600254D RID: 9549 RVA: 0x0009C6AA File Offset: 0x0009AAAA
			public void Dispose()
			{
				this.metrics.Dispose();
			}

			// Token: 0x040012E1 RID: 4833
			public readonly EnumBasedMetricsContainer metrics;
		}

		// Token: 0x020006F1 RID: 1777
		private class Pattern
		{
			// Token: 0x040012E2 RID: 4834
			public string wildcard;

			// Token: 0x040012E3 RID: 4835
			public Regex pattern;
		}
	}
}
