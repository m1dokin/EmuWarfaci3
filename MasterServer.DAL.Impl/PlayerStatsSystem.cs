using System;
using System.Collections.Generic;
using MasterServer.DAL.PlayerStats;
using MasterServer.DAL.Utils;
using MasterServer.Database;
using OLAPHypervisor;

namespace MasterServer.DAL.Impl
{
	// Token: 0x0200001A RID: 26
	internal class PlayerStatsSystem : IPlayerStatsSystem
	{
		// Token: 0x060000ED RID: 237 RVA: 0x000092A2 File Offset: 0x000074A2
		public PlayerStatsSystem(DAL dal)
		{
			this.m_dal = dal;
		}

		// Token: 0x060000EE RID: 238 RVA: 0x000092B1 File Offset: 0x000074B1
		public void Start()
		{
			this.m_aggOperations = this.m_dal.TelemetrySystem.GetAggregationOps().Value;
			this.m_defaultAggOp = this.m_dal.TelemetrySystem.GetDefaultAggregationOp().Value;
		}

		// Token: 0x060000EF RID: 239 RVA: 0x000092EC File Offset: 0x000074EC
		public DALResultVoid UpdatePlayerStats(ulong profileId, List<Measure> data)
		{
			PlayerStatsSystem.<UpdatePlayerStats>c__AnonStorey1 <UpdatePlayerStats>c__AnonStorey = new PlayerStatsSystem.<UpdatePlayerStats>c__AnonStorey1();
			<UpdatePlayerStats>c__AnonStorey.profileId = profileId;
			<UpdatePlayerStats>c__AnonStorey.data = data;
			<UpdatePlayerStats>c__AnonStorey.$this = this;
			DALStats dalstats = new DALStats();
			using (MySqlAccessorTransaction acc = new MySqlAccessorTransaction(dalstats))
			{
				acc.Transaction(delegate()
				{
					byte[] array = null;
					DataArchiveSerializer<PlayerStatistics> dataArchiveSerializer = new DataArchiveSerializer<PlayerStatistics>(new PlayerStatisticsDataSerializer());
					using (DBDataReader dbdataReader = acc.ExecuteReader("CALL GetPlayerStats(?pid)", new object[]
					{
						"?pid",
						<UpdatePlayerStats>c__AnonStorey.profileId
					}))
					{
						if (dbdataReader.Read())
						{
							array = (byte[])dbdataReader["stats"];
						}
					}
					PlayerStatistics playerStatistics = (array == null) ? new PlayerStatistics
					{
						Measures = new List<Measure>()
					} : dataArchiveSerializer.Deserialize(array, DBVersion.Zero);
					for (int i = 0; i < <UpdatePlayerStats>c__AnonStorey.data.Count; i++)
					{
						Measure measure = <UpdatePlayerStats>c__AnonStorey.data[i];
						bool flag = false;
						int num = 0;
						while (num < playerStatistics.Measures.Count && !flag)
						{
							Measure value = playerStatistics.Measures[num];
							if (value.DimensionsEqual(measure.Dimensions))
							{
								value.AggregateOp = measure.AggregateOp;
								if (measure.AggregateOp != 4 && !<UpdatePlayerStats>c__AnonStorey.$this.m_aggOperations.TryGetValue(value.Dimensions["stat"], out value.AggregateOp))
								{
									value.AggregateOp = <UpdatePlayerStats>c__AnonStorey.$this.m_defaultAggOp;
								}
								value.ApplyAggregation(measure);
								playerStatistics.Measures[num] = value;
								flag = true;
							}
							num++;
						}
						if (!flag)
						{
							playerStatistics.Measures.Add(measure);
						}
					}
					byte[] array2 = dataArchiveSerializer.Serialize(new PlayerStatistics
					{
						Measures = playerStatistics.Measures,
						Version = playerStatistics.Version
					});
					<UpdatePlayerStats>c__AnonStorey.$this.m_dal.ValidateFixedSizeColumnData("profile_stats", "stats", array2.Length);
					acc.ExecuteNonQuery("CALL UpdatePlayerStats(?pid, ?data)", new object[]
					{
						"?pid",
						<UpdatePlayerStats>c__AnonStorey.profileId,
						"?data",
						array2
					});
				});
			}
			return new DALResultVoid(dalstats);
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00009380 File Offset: 0x00007580
		public DALResult<PlayerStatistics> GetPlayerStats(ulong profileId)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL GetPlayerStats(?pid)", new object[]
			{
				"?pid",
				profileId
			});
			byte[] array = (byte[])this.m_dal.CacheProxy.SetScalar(setOptions);
			DataArchiveSerializer<PlayerStatistics> dataArchiveSerializer = new DataArchiveSerializer<PlayerStatistics>(new PlayerStatisticsDataSerializer());
			return new DALResult<PlayerStatistics>((array == null) ? new PlayerStatistics
			{
				Measures = new List<Measure>()
			} : dataArchiveSerializer.Deserialize(array, DBVersion.Zero), setOptions.stats);
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x0000940C File Offset: 0x0000760C
		public DALResultVoid ResetPlayerStats(ulong profileId)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL ResetPlayerStats(?pid)", new object[]
			{
				"?pid",
				profileId
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x04000058 RID: 88
		private DAL m_dal;

		// Token: 0x04000059 RID: 89
		private Dictionary<string, EAggOperation> m_aggOperations;

		// Token: 0x0400005A RID: 90
		private EAggOperation m_defaultAggOp;
	}
}
