using System;
using System.Collections.Generic;
using MasterServer.Core;
using MasterServer.Core.Diagnostics.Process;
using MasterServer.CryOnlineNET;
using MasterServer.Matchmaking;
using MasterServer.Telemetry.Aggregaton;
using OLAPHypervisor;

namespace MasterServer.Telemetry
{
	// Token: 0x0200072C RID: 1836
	internal class ProcessTracker : IDisposable
	{
		// Token: 0x06002606 RID: 9734 RVA: 0x0009FE64 File Offset: 0x0009E264
		public ProcessTracker(TelemetryService service)
		{
			this.m_service = service;
			ProcessInformation.GetInfo();
			this.m_threadPoolStats = ThreadPoolProxy.GetStats(true);
			string schedule = Resources.ModuleSettings.Get("MonitoringSchedule");
			TimeSpan jitter = TimeSpan.Parse(Resources.ModuleSettings.Get("MonitoringScheduleJitter"));
			this.m_service.DeferredStream.RegisterMeasureCallback(schedule, jitter, new MeasureCallback(this.Report));
			IQueryManager service2 = ServicesManager.GetService<IQueryManager>();
			service2.QueryCompleted += this.QueryManager_OnQueryCompleted;
			IOnlineClient service3 = ServicesManager.GetService<IOnlineClient>();
			service3.OnlineQueryStats += this.OnlineClient_OnOnlineQueryStats;
			IMatchmakingPerformer service4 = ServicesManager.GetService<IMatchmakingPerformer>();
			service4.OnMatchmakingPerformerStats += this.OnMatchmakingPerformerStats;
		}

		// Token: 0x06002607 RID: 9735 RVA: 0x0009FF40 File Offset: 0x0009E340
		public void Dispose()
		{
			this.m_service.DeferredStream.UnregisterMeasureCallback(new MeasureCallback(this.Report));
			IQueryManager service = ServicesManager.GetService<IQueryManager>();
			service.QueryCompleted -= this.QueryManager_OnQueryCompleted;
			IOnlineClient service2 = ServicesManager.GetService<IOnlineClient>();
			service2.OnlineQueryStats -= this.OnlineClient_OnOnlineQueryStats;
			IMatchmakingPerformer service3 = ServicesManager.GetService<IMatchmakingPerformer>();
			service3.OnMatchmakingPerformerStats -= this.OnMatchmakingPerformerStats;
		}

		// Token: 0x06002608 RID: 9736 RVA: 0x0009FFB4 File Offset: 0x0009E3B4
		private void Report(List<Measure> outMeasures, DateTime forDate)
		{
			if (!this.m_service.CheckMode(TelemetryMode.Monitoring))
			{
				return;
			}
			string format = Resources.ModuleSettings.Get("MonitoringStatsTimeFormat");
			string value = forDate.ToString(format);
			int count = outMeasures.Count;
			ProcessInformation.Info info = ProcessInformation.GetInfo();
			ThreadPoolProxy.Stats stats = ThreadPoolProxy.GetStats(false);
			Dictionary<string, QueryAggregate> dictionary;
			Dictionary<string, DALProcedureAggregate> dictionary2;
			MMAggregate mmAggregate;
			lock (this)
			{
				dictionary = this.m_queryAggregates.SwapAggregates();
				dictionary2 = this.m_procedureAggregates.SwapAggregates();
				mmAggregate = this.m_mmAggregate;
				this.m_mmAggregate = new MMAggregate();
			}
			outMeasures.Add(this.m_service.MakeMeasure((long)info.TotalProcessorTime, new object[]
			{
				"stat",
				"srv_processor_time_total",
				"server",
				Resources.ServerName,
				"host",
				Resources.Hostname
			}));
			outMeasures.Add(this.m_service.MakeMeasure((long)info.UserProcessorTime, new object[]
			{
				"stat",
				"srv_processor_time_user",
				"server",
				Resources.ServerName,
				"host",
				Resources.Hostname
			}));
			outMeasures.Add(this.m_service.MakeMeasure((long)info.KernelProcessorTime, new object[]
			{
				"stat",
				"srv_processor_time_kernel",
				"server",
				Resources.ServerName,
				"host",
				Resources.Hostname
			}));
			outMeasures.Add(this.m_service.MakeMeasure(info.VirtualMemory / 1024L, new object[]
			{
				"stat",
				"srv_memory_virtual_total",
				"server",
				Resources.ServerName,
				"host",
				Resources.Hostname
			}));
			outMeasures.Add(this.m_service.MakeMeasure(info.WorkingSetMemory / 1024L, new object[]
			{
				"stat",
				"srv_memory_working_set",
				"server",
				Resources.ServerName,
				"host",
				Resources.Hostname
			}));
			outMeasures.Add(this.m_service.MakeMeasure((long)info.HandleCount, new object[]
			{
				"stat",
				"srv_handle_count",
				"server",
				Resources.ServerName,
				"host",
				Resources.Hostname
			}));
			outMeasures.Add(this.m_service.MakeMeasure((long)info.OsThreads, new object[]
			{
				"stat",
				"srv_thread_count",
				"server",
				Resources.ServerName,
				"host",
				Resources.Hostname
			}));
			outMeasures.Add(this.m_service.MakeMeasure((long)info.ThreadPoolMaxThreads, new object[]
			{
				"stat",
				"srv_thread_pool_max",
				"server",
				Resources.ServerName,
				"host",
				Resources.Hostname
			}));
			outMeasures.Add(this.m_service.MakeMeasure((long)info.ThreadPoolActiveThreads, new object[]
			{
				"stat",
				"srv_thread_pool_active",
				"server",
				Resources.ServerName,
				"host",
				Resources.Hostname
			}));
			int num = stats.ItemsDispatched - this.m_threadPoolStats.ItemsDispatched;
			if (num != 0)
			{
				outMeasures.Add(this.m_service.MakeMeasure((long)((stats.TimeInQueueTotalMs - this.m_threadPoolStats.TimeInQueueTotalMs) / num), new object[]
				{
					"stat",
					"srv_thread_pool_avg_queue_time",
					"server",
					Resources.ServerName,
					"host",
					Resources.Hostname
				}));
				outMeasures.Add(this.m_service.MakeMeasure((long)stats.TimeInQueuePeakMs, new object[]
				{
					"stat",
					"srv_thread_pool_max_queue_time",
					"server",
					Resources.ServerName,
					"host",
					Resources.Hostname
				}));
			}
			this.m_threadPoolStats = stats;
			foreach (KeyValuePair<string, QueryAggregate> keyValuePair in dictionary)
			{
				string key = keyValuePair.Key;
				QueryAggregate value2 = keyValuePair.Value;
				if (value2.Executed != 0)
				{
					outMeasures.Add(this.m_service.MakeMeasure((long)value2.Executed, new object[]
					{
						"stat",
						"srv_queries_executed",
						"query",
						key
					}));
					outMeasures.Add(this.m_service.MakeMeasure((long)value2.Failed, new object[]
					{
						"stat",
						"srv_queries_failed",
						"query",
						key
					}));
					TimeSpan timeSpan = this.Max(value2.ServicingTime.Total, value2.ProcessingTime.Total);
					outMeasures.Add(this.m_service.MakeMeasure((long)(timeSpan.TotalMilliseconds / (double)value2.Executed), new object[]
					{
						"stat",
						"srv_query_servicing_time",
						"query",
						key
					}));
					TimeSpan timeSpan2 = this.Max(value2.ServicingTime.Max, value2.ProcessingTime.Max);
					outMeasures.Add(this.m_service.MakeMeasure((long)timeSpan2.TotalMilliseconds, new object[]
					{
						"stat",
						"srv_query_servicing_time_peak",
						"query",
						key
					}));
					outMeasures.Add(this.m_service.MakeMeasure((long)(value2.ProcessingTime.Total.TotalMilliseconds / (double)value2.Executed), new object[]
					{
						"stat",
						"srv_query_processing_time",
						"query",
						key
					}));
					outMeasures.Add(this.m_service.MakeMeasure((long)value2.ProcessingTime.Max.TotalMilliseconds, new object[]
					{
						"stat",
						"srv_query_processing_time_peak",
						"query",
						key
					}));
					outMeasures.Add(this.m_service.MakeMeasure((long)(value2.DALTime.Total.TotalMilliseconds / (double)value2.Executed), new object[]
					{
						"stat",
						"srv_query_dal_time",
						"query",
						key
					}));
					outMeasures.Add(this.m_service.MakeMeasure((long)value2.DALTime.Max.TotalMilliseconds, new object[]
					{
						"stat",
						"srv_query_dal_time_peak",
						"query",
						key
					}));
					outMeasures.Add(this.m_service.MakeMeasure((long)(value2.DALCallsTotal / value2.Executed), new object[]
					{
						"stat",
						"srv_query_dal_queries",
						"query",
						key
					}));
					outMeasures.Add(this.m_service.MakeMeasure((long)((ulong)value2.Upload.CompressedData / (ulong)((long)value2.Executed)), new object[]
					{
						"stat",
						"srv_query_traffic_up_compressed",
						"query",
						key
					}));
					outMeasures.Add(this.m_service.MakeMeasure((long)((ulong)value2.Upload.TotalData / (ulong)((long)value2.Executed)), new object[]
					{
						"stat",
						"srv_query_traffic_up_data",
						"query",
						key
					}));
					outMeasures.Add(this.m_service.MakeMeasure((long)((ulong)value2.Download.CompressedData / (ulong)((long)value2.Executed)), new object[]
					{
						"stat",
						"srv_query_traffic_down_compressed",
						"query",
						key
					}));
					outMeasures.Add(this.m_service.MakeMeasure((long)((ulong)value2.Download.TotalData / (ulong)((long)value2.Executed)), new object[]
					{
						"stat",
						"srv_query_traffic_down_data",
						"query",
						key
					}));
					outMeasures.Add(this.m_service.MakeMeasure((long)((ulong)value2.Upload.CompressedData), new object[]
					{
						"stat",
						"srv_query_traffic_up_rate",
						"query",
						key
					}));
					outMeasures.Add(this.m_service.MakeMeasure((long)((ulong)value2.Download.CompressedData), new object[]
					{
						"stat",
						"srv_query_traffic_down_rate",
						"query",
						key
					}));
					foreach (KeyValuePair<string, int> keyValuePair2 in value2.DALCalls)
					{
						string key2 = keyValuePair2.Key;
						int value3 = keyValuePair2.Value;
						outMeasures.Add(this.m_service.MakeMeasure((long)value3, new object[]
						{
							"stat",
							"srv_query_dal_procedure_calls",
							"query",
							key,
							"procedure",
							key2
						}));
					}
				}
			}
			foreach (KeyValuePair<string, DALProcedureAggregate> keyValuePair3 in dictionary2)
			{
				string key3 = keyValuePair3.Key;
				DALProcedureAggregate value4 = keyValuePair3.Value;
				outMeasures.Add(this.m_service.MakeMeasure((long)(value4.DatabaseTime.Total.TotalMilliseconds / (double)value4.Executed), new object[]
				{
					"stat",
					"srv_dal_procedure_db_time",
					"procedure",
					key3
				}));
				outMeasures.Add(this.m_service.MakeMeasure((long)value4.DatabaseTime.Max.TotalMilliseconds, new object[]
				{
					"stat",
					"srv_dal_procedure_db_time_peak",
					"procedure",
					key3
				}));
				outMeasures.Add(this.m_service.MakeMeasure((long)(value4.DatabaseQueries / value4.Executed), new object[]
				{
					"stat",
					"srv_dal_procedure_db_queries",
					"procedure",
					key3
				}));
				outMeasures.Add(this.m_service.MakeMeasure((long)(value4.ConnectingTime.Total.TotalMilliseconds / (double)value4.Executed), new object[]
				{
					"stat",
					"srv_dal_procedure_db_connect_time",
					"procedure",
					key3
				}));
				outMeasures.Add(this.m_service.MakeMeasure((long)value4.ConnectingTime.Max.TotalMilliseconds, new object[]
				{
					"stat",
					"srv_dal_procedure_db_connect_time_peak",
					"procedure",
					key3
				}));
				outMeasures.Add(this.m_service.MakeMeasure((long)(value4.CacheTime.Total.TotalMilliseconds / (double)value4.Executed), new object[]
				{
					"stat",
					"srv_dal_procedure_cache_time",
					"procedure",
					key3
				}));
				outMeasures.Add(this.m_service.MakeMeasure((long)value4.CacheTime.Max.TotalMilliseconds, new object[]
				{
					"stat",
					"srv_dal_procedure_cache_time_peak",
					"procedure",
					key3
				}));
				outMeasures.Add(this.m_service.MakeMeasure((long)value4.L1CacheHits, new object[]
				{
					"stat",
					"srv_dal_procedure_cache_hits_l1",
					"procedure",
					key3
				}));
				outMeasures.Add(this.m_service.MakeMeasure((long)value4.L1CacheMisses, new object[]
				{
					"stat",
					"srv_dal_procedure_cache_misses_l1",
					"procedure",
					key3
				}));
				outMeasures.Add(this.m_service.MakeMeasure((long)value4.L1CacheClear, new object[]
				{
					"stat",
					"srv_dal_procedure_cache_clear_l1",
					"procedure",
					key3
				}));
				outMeasures.Add(this.m_service.MakeMeasure((long)value4.L2CacheHits, new object[]
				{
					"stat",
					"srv_dal_procedure_cache_hits_l2",
					"procedure",
					key3
				}));
				outMeasures.Add(this.m_service.MakeMeasure((long)value4.L2CacheMisses, new object[]
				{
					"stat",
					"srv_dal_procedure_cache_misses_l2",
					"procedure",
					key3
				}));
				outMeasures.Add(this.m_service.MakeMeasure((long)value4.L2CacheClear, new object[]
				{
					"stat",
					"srv_dal_procedure_cache_clear_l2",
					"procedure",
					key3
				}));
			}
			if (mmAggregate.Executed > 0)
			{
				outMeasures.Add(this.m_service.MakeMeasure((long)mmAggregate.Executed, new object[]
				{
					"stat",
					"srv_mm_runs",
					"server",
					Resources.ServerName,
					"host",
					Resources.Hostname
				}));
				outMeasures.Add(this.m_service.MakeMeasure((long)mmAggregate.MaxPlayers, new object[]
				{
					"stat",
					"srv_mm_players_max",
					"server",
					Resources.ServerName,
					"host",
					Resources.Hostname
				}));
				outMeasures.Add(this.m_service.MakeMeasure((long)(mmAggregate.Players / mmAggregate.Executed), new object[]
				{
					"stat",
					"srv_mm_players_avg",
					"server",
					Resources.ServerName,
					"host",
					Resources.Hostname
				}));
				outMeasures.Add(this.m_service.MakeMeasure((long)(mmAggregate.ExecuteTime.Total.TotalMilliseconds / (double)mmAggregate.Executed), new object[]
				{
					"stat",
					"srv_mm_run_time",
					"server",
					Resources.ServerName,
					"host",
					Resources.Hostname
				}));
				outMeasures.Add(this.m_service.MakeMeasure((long)mmAggregate.ExecuteTime.Max.TotalMilliseconds, new object[]
				{
					"stat",
					"srv_mm_run_time_peak",
					"server",
					Resources.ServerName,
					"host",
					Resources.Hostname
				}));
			}
			for (int num2 = count; num2 != outMeasures.Count; num2++)
			{
				outMeasures[num2].Dimensions.Add("date", value);
			}
		}

		// Token: 0x06002609 RID: 9737 RVA: 0x000A0F38 File Offset: 0x0009F338
		private TimeSpan Max(TimeSpan a, TimeSpan b)
		{
			return (!(a < b)) ? a : b;
		}

		// Token: 0x0600260A RID: 9738 RVA: 0x000A0F50 File Offset: 0x0009F350
		private void QueryManager_OnQueryCompleted(QueryContext ctx)
		{
			if (!this.m_service.CheckMode(TelemetryMode.Monitoring))
			{
				return;
			}
			lock (this)
			{
				this.m_queryAggregates.AddToAggregates(ctx);
				this.m_procedureAggregates.AddToAggregates(ctx.Stats.DALCalls);
			}
		}

		// Token: 0x0600260B RID: 9739 RVA: 0x000A0FC0 File Offset: 0x0009F3C0
		private void OnlineClient_OnOnlineQueryStats(OnlineQueryStats stats)
		{
			if (!this.m_service.CheckMode(TelemetryMode.Monitoring))
			{
				return;
			}
			lock (this)
			{
				this.m_queryAggregates.AddToAggregates(stats);
			}
		}

		// Token: 0x0600260C RID: 9740 RVA: 0x000A1018 File Offset: 0x0009F418
		private void OnMatchmakingPerformerStats(MatchmakingPerformerStats stats)
		{
			if (!this.m_service.CheckMode(TelemetryMode.Monitoring))
			{
				return;
			}
			lock (this)
			{
				this.m_mmAggregate.ExecuteTime.apply(stats.TimeSpend);
				this.m_mmAggregate.MaxPlayers = Math.Max(this.m_mmAggregate.MaxPlayers, stats.PlayersTotal);
				this.m_mmAggregate.Players += stats.PlayersTotal;
				this.m_mmAggregate.Executed++;
			}
		}

		// Token: 0x04001386 RID: 4998
		private TelemetryService m_service;

		// Token: 0x04001387 RID: 4999
		private ThreadPoolProxy.Stats m_threadPoolStats;

		// Token: 0x04001388 RID: 5000
		private QueryAggregateStorage m_queryAggregates = new QueryAggregateStorage();

		// Token: 0x04001389 RID: 5001
		private ProcedureAggregateStorage m_procedureAggregates = new ProcedureAggregateStorage();

		// Token: 0x0400138A RID: 5002
		private MMAggregate m_mmAggregate = new MMAggregate();
	}
}
