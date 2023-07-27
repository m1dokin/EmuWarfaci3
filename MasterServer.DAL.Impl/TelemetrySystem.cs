using System;
using System.Collections.Generic;
using System.IO;
using MasterServer.Core;
using OLAPHypervisor;

namespace MasterServer.DAL.Impl
{
	// Token: 0x02000025 RID: 37
	internal class TelemetrySystem : ITelemetrySystem, IDisposable
	{
		// Token: 0x06000187 RID: 391 RVA: 0x0000E8D0 File Offset: 0x0000CAD0
		public TelemetrySystem(DAL dal)
		{
			this.m_dal = dal;
		}

		// Token: 0x06000188 RID: 392 RVA: 0x0000E8E0 File Offset: 0x0000CAE0
		private void InitHypervisor()
		{
			Log.CurrentLogger = new MasterServerLogRedirect();
			this.m_hypervisor = new Hypervisor();
			HypervisorInitParams hypervisorInitParams = new HypervisorInitParams(Resources.TelemetryConnectionCfg, Resources.TelemetrySchemaCfg, Path.Combine(Resources.TelemetryResourceDir, "updates"), Resources.AggregationEnabled);
			if (!this.m_hypervisor.Init(hypervisorInitParams) || this.m_hypervisor.DBSchema.Cubes.Count == 0)
			{
				throw new ServiceModuleException("Failed to init telemetry OLAP hypervisor");
			}
			this.m_hypervisor.OnAggregationCompleted += new Hypervisor.AggregationTaskCompletedDeleg(this.OnAggregationCompleted);
		}

		// Token: 0x06000189 RID: 393 RVA: 0x0000E973 File Offset: 0x0000CB73
		public void Start()
		{
			this.InitHypervisor();
			this.m_statMappings = new StatsMappings(Resources.TelemetryStatsMap, this.m_hypervisor);
		}

		// Token: 0x0600018A RID: 394 RVA: 0x0000E991 File Offset: 0x0000CB91
		public DALResultVoid RunAggregation()
		{
			this.m_hypervisor.RunAggregation();
			return new DALResultVoid(new DALStats());
		}

		// Token: 0x0600018B RID: 395 RVA: 0x0000E9A8 File Offset: 0x0000CBA8
		private void OnAggregationCompleted(AggregationStats stats)
		{
			string text = DateTime.Now.ToString("yyyy-MM-dd");
			this.AddMeasure((long)stats.TotalTime.TotalMilliseconds / 100L, new object[]
			{
				"stat",
				"srv_telem_aggregation_time",
				"server",
				Resources.ServerName,
				"host",
				Resources.Hostname,
				"date",
				text
			});
			this.AddMeasure((long)stats.RowsAdded, new object[]
			{
				"stat",
				"srv_telem_aggregation_rows_added",
				"server",
				Resources.ServerName,
				"host",
				Resources.Hostname,
				"date",
				text
			});
			this.AddMeasure((long)stats.RowsRemoved, new object[]
			{
				"stat",
				"srv_telem_aggregation_rows_removed",
				"server",
				Resources.ServerName,
				"host",
				Resources.Hostname,
				"date",
				text
			});
		}

		// Token: 0x0600018C RID: 396 RVA: 0x0000EAC0 File Offset: 0x0000CCC0
		public DALResultMulti<Measure> GetPlayerStats(ulong profile_id)
		{
			CacheProxy.Options<Measure> options = new CacheProxy.Options<Measure>
			{
				get_data_stream = (() => this.m_hypervisor.GetDataFrontend().GetPlayerTelemetry(profile_id))
			};
			return this.m_dal.CacheProxy.GetStream<Measure>(options);
		}

		// Token: 0x0600018D RID: 397 RVA: 0x0000EB0C File Offset: 0x0000CD0C
		public DALResultMulti<Measure> GetPlayerStatsRaw(ulong profileId)
		{
			CacheProxy.Options<Measure> options = new CacheProxy.Options<Measure>
			{
				get_data_stream = (() => this.m_hypervisor.GetDataFrontend().GetPlayerTelemetryRawData(profileId))
			};
			return this.m_dal.CacheProxy.GetStream<Measure>(options);
		}

		// Token: 0x0600018E RID: 398 RVA: 0x0000EB58 File Offset: 0x0000CD58
		public DALResultVoid ResetPlayerStats(ulong profile_id)
		{
			this.m_hypervisor.GetDataFrontend().ResetPlayerTelemetry(profile_id);
			return new DALResultVoid(new DALStats());
		}

		// Token: 0x0600018F RID: 399 RVA: 0x0000EB75 File Offset: 0x0000CD75
		public DALResultVoid ResetTelemetryTestStats()
		{
			this.m_hypervisor.GetDataFrontend().ResetTestTelemetry();
			return new DALResultVoid(new DALStats());
		}

		// Token: 0x06000190 RID: 400 RVA: 0x0000EB91 File Offset: 0x0000CD91
		public DALResult<EAggOperation> GetDefaultAggregationOp()
		{
			return new DALResult<EAggOperation>(this.m_statMappings.DefaultAggOp, new DALStats());
		}

		// Token: 0x06000191 RID: 401 RVA: 0x0000EBA8 File Offset: 0x0000CDA8
		public DALResult<Dictionary<string, EAggOperation>> GetAggregationOps()
		{
			Dictionary<string, EAggOperation> dictionary = new Dictionary<string, EAggOperation>(this.m_statMappings.StatMap.Count);
			foreach (KeyValuePair<string, StatsMappings.StatParams> keyValuePair in this.m_statMappings.StatMap)
			{
				dictionary.Add(keyValuePair.Key, keyValuePair.Value.AggOp);
			}
			return new DALResult<Dictionary<string, EAggOperation>>(dictionary, new DALStats());
		}

		// Token: 0x06000192 RID: 402 RVA: 0x0000EC40 File Offset: 0x0000CE40
		public DALResult<bool> SetAggregationOps(string stat, EAggOperation newAggOp)
		{
			Cube cube;
			EAggOperation eaggOperation;
			bool flag = this.m_statMappings.GetStatParams(stat, out cube, out eaggOperation);
			if (flag)
			{
				flag = this.m_statMappings.SetStatParams(stat, cube, newAggOp);
			}
			return new DALResult<bool>(flag, new DALStats());
		}

		// Token: 0x06000193 RID: 403 RVA: 0x0000EC80 File Offset: 0x0000CE80
		public void AddMeasure(long value, params object[] args)
		{
			if (args.Length % 2 != 0)
			{
				throw new ServiceModuleException("Invalid number of parameters to build dimensions");
			}
			SortedList<string, string> sortedList = new SortedList<string, string>();
			for (int i = 0; i < args.Length; i += 2)
			{
				if (args[i + 1] != null)
				{
					sortedList.Add(args[i].ToString(), args[i + 1].ToString());
				}
			}
			Measure msr = default(Measure);
			msr.Dimensions = sortedList;
			msr.Value = value;
			msr.RowCount = 1L;
			this.AddMeasure(msr);
		}

		// Token: 0x06000194 RID: 404 RVA: 0x0000ED08 File Offset: 0x0000CF08
		public DALResultVoid AddMeasure(Measure msr)
		{
			Cube cube;
			EAggOperation aggregateOp;
			if (this.m_statMappings.GetStatParams(msr.Dimensions["stat"], out cube, out aggregateOp))
			{
				if (msr.AggregateOp != 4)
				{
					msr.AggregateOp = aggregateOp;
				}
				this.m_hypervisor.ApplyUpdate(new Measure[]
				{
					msr
				}, cube);
			}
			return new DALResultVoid(new DALStats());
		}

		// Token: 0x06000195 RID: 405 RVA: 0x0000ED78 File Offset: 0x0000CF78
		public DALResultVoid AddMeasure(IEnumerable<Measure> msrs)
		{
			List<Measure> list = new List<Measure>();
			Cube cube = null;
			foreach (Measure measure in msrs)
			{
				Measure item = measure;
				Cube cube2;
				EAggOperation aggregateOp;
				if (this.m_statMappings.GetStatParams(item.Dimensions["stat"], out cube2, out aggregateOp))
				{
					if (item.AggregateOp != 4)
					{
						item.AggregateOp = aggregateOp;
					}
					if (cube == null)
					{
						cube = cube2;
					}
					if (cube != cube2)
					{
						if (list.Count != 0)
						{
							this.m_hypervisor.ApplyUpdate(list, cube);
							list.Clear();
						}
						cube = cube2;
					}
					list.Add(item);
				}
			}
			if (list.Count != 0)
			{
				this.m_hypervisor.ApplyUpdate(list, cube);
			}
			return new DALResultVoid(new DALStats());
		}

		// Token: 0x06000196 RID: 406 RVA: 0x0000EE6C File Offset: 0x0000D06C
		public void Dispose()
		{
			if (this.m_hypervisor != null)
			{
				this.m_hypervisor.Dispose();
				this.m_hypervisor = null;
			}
		}

		// Token: 0x0400007A RID: 122
		private readonly DAL m_dal;

		// Token: 0x0400007B RID: 123
		private Hypervisor m_hypervisor;

		// Token: 0x0400007C RID: 124
		private StatsMappings m_statMappings;
	}
}
