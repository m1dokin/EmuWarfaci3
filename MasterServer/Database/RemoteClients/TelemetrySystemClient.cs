using System;
using System.Collections.Generic;
using System.Reflection;
using MasterServer.DAL;
using OLAPHypervisor;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x02000216 RID: 534
	internal class TelemetrySystemClient : DALCacheProxy<ITelemetryDALService>, ITelemetrySystemClient
	{
		// Token: 0x06000B9A RID: 2970 RVA: 0x0002BBA6 File Offset: 0x00029FA6
		internal void Reset(ITelemetrySystem telemetrySystem)
		{
			this.m_telemetrySystem = telemetrySystem;
		}

		// Token: 0x06000B9B RID: 2971 RVA: 0x0002BBB0 File Offset: 0x00029FB0
		public void RunAggregation()
		{
			DALCacheProxy<ITelemetryDALService>.SetOptions options = new DALCacheProxy<ITelemetryDALService>.SetOptions
			{
				set_func = (() => this.m_telemetrySystem.RunAggregation())
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B9C RID: 2972 RVA: 0x0002BBE4 File Offset: 0x00029FE4
		[Obsolete("For receive player statistics use PlayerStatsService")]
		public IEnumerable<Measure> GetPlayerStats(ulong profileId)
		{
			DALCacheProxy<ITelemetryDALService>.Options<Measure> options = new DALCacheProxy<ITelemetryDALService>.Options<Measure>
			{
				get_data_stream = (() => this.m_telemetrySystem.GetPlayerStats(profileId))
			};
			return base.GetDataStream<Measure>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B9D RID: 2973 RVA: 0x0002BC2C File Offset: 0x0002A02C
		public IEnumerable<Measure> GetPlayerStatsRaw(ulong profileId)
		{
			DALCacheProxy<ITelemetryDALService>.Options<Measure> options = new DALCacheProxy<ITelemetryDALService>.Options<Measure>
			{
				get_data_stream = (() => this.m_telemetrySystem.GetPlayerStatsRaw(profileId))
			};
			return base.GetDataStream<Measure>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B9E RID: 2974 RVA: 0x0002BC74 File Offset: 0x0002A074
		public void ResetPlayerStats(ulong profileId)
		{
			DALCacheProxy<ITelemetryDALService>.SetOptions options = new DALCacheProxy<ITelemetryDALService>.SetOptions
			{
				set_func = (() => this.m_telemetrySystem.ResetPlayerStats(profileId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B9F RID: 2975 RVA: 0x0002BCBC File Offset: 0x0002A0BC
		public void ResetTelemetryTestStats()
		{
			DALCacheProxy<ITelemetryDALService>.SetOptions options = new DALCacheProxy<ITelemetryDALService>.SetOptions
			{
				set_func = (() => this.m_telemetrySystem.ResetTelemetryTestStats())
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000BA0 RID: 2976 RVA: 0x0002BCF0 File Offset: 0x0002A0F0
		public EAggOperation GetDefaultAggregationOp()
		{
			DALCacheProxy<ITelemetryDALService>.Options<EAggOperation> options = new DALCacheProxy<ITelemetryDALService>.Options<EAggOperation>
			{
				get_data = (() => this.m_telemetrySystem.GetDefaultAggregationOp())
			};
			return base.GetData<EAggOperation>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000BA1 RID: 2977 RVA: 0x0002BD24 File Offset: 0x0002A124
		public Dictionary<string, EAggOperation> GetAggregationOps()
		{
			DALCacheProxy<ITelemetryDALService>.Options<Dictionary<string, EAggOperation>> options = new DALCacheProxy<ITelemetryDALService>.Options<Dictionary<string, EAggOperation>>
			{
				get_data = (() => this.m_telemetrySystem.GetAggregationOps())
			};
			return base.GetData<Dictionary<string, EAggOperation>>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000BA2 RID: 2978 RVA: 0x0002BD58 File Offset: 0x0002A158
		public bool SetAggregationOps(string stat, EAggOperation newAggOp)
		{
			DALCacheProxy<ITelemetryDALService>.Options<bool> options = new DALCacheProxy<ITelemetryDALService>.Options<bool>
			{
				get_data = (() => this.m_telemetrySystem.SetAggregationOps(stat, newAggOp))
			};
			return base.GetData<bool>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000BA3 RID: 2979 RVA: 0x0002BDA8 File Offset: 0x0002A1A8
		public void AddMeasure(IEnumerable<Measure> measures)
		{
			try
			{
				DALCacheProxy<ITelemetryDALService>.SetOptions options = new DALCacheProxy<ITelemetryDALService>.SetOptions
				{
					set_func = (() => this.m_telemetrySystem.AddMeasure(measures))
				};
				base.SetAndClear(MethodBase.GetCurrentMethod(), options);
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		// Token: 0x04000564 RID: 1380
		private ITelemetrySystem m_telemetrySystem;
	}
}
