using System;
using System.Collections.Generic;
using MasterServer.Database;

namespace MasterServer.DAL.Impl
{
	// Token: 0x02000016 RID: 22
	internal class MissionSystem : IMissionSystem
	{
		// Token: 0x060000CF RID: 207 RVA: 0x00007D96 File Offset: 0x00005F96
		public MissionSystem(DAL dal)
		{
			this.m_dal = dal;
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00007DC8 File Offset: 0x00005FC8
		public DALResultVoid RemoveMission(Guid uid)
		{
			DALStats dalstats = new DALStats();
			DALResultVoid result;
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor(dalstats))
			{
				mySqlAccessor.ExecuteNonQuery("CALL RemoveMission(?uid)", new object[]
				{
					"?uid",
					uid.ToByteArray()
				});
				result = new DALResultVoid(dalstats);
			}
			return result;
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00007E30 File Offset: 0x00006030
		public DALResult<SMission> GetMission(Guid uid)
		{
			CacheProxy.Options<SMission> options = new CacheProxy.Options<SMission>
			{
				db_serializer = this.m_mission_serializer
			};
			options.query("CALL GetMission(?uid)", new object[]
			{
				"?uid",
				uid.ToByteArray()
			});
			return this.m_dal.CacheProxy.Get<SMission>(options);
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00007E88 File Offset: 0x00006088
		public DALResultMulti<SMission> GetMissions(int period)
		{
			DALStats dalstats = new DALStats();
			DALResultMulti<SMission> result;
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor(dalstats))
			{
				using (DBDataReader dbdataReader = mySqlAccessor.ExecuteReader("CALL GetMissions(?period)", new object[]
				{
					"?period",
					period
				}))
				{
					IEnumerable<SMission> val = SerializeHelper.Deserialize<SMission>(dbdataReader, this.m_mission_serializer);
					result = new DALResultMulti<SMission>(val, dalstats);
				}
			}
			return result;
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00007F18 File Offset: 0x00006118
		public DALResultMulti<Guid> GetCurrentMissions()
		{
			DALStats dalstats = new DALStats();
			DALResultMulti<Guid> result;
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor(dalstats))
			{
				using (DBDataReader dbdataReader = mySqlAccessor.ExecuteReader("CALL GetCurrentMissions()", new object[0]))
				{
					List<Guid> list = new List<Guid>();
					while (dbdataReader.Read())
					{
						list.Add(dbdataReader.GetGuid(0));
					}
					result = new DALResultMulti<Guid>(list, dalstats);
				}
			}
			return result;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00007FB0 File Offset: 0x000061B0
		public DALResultVoid SaveMission(Guid uid, string name, string gameMode, string data, int generation)
		{
			this.m_dal.ValidateFixedSizeColumnData("missions", "data", data.Length);
			DALStats dalstats = new DALStats();
			DALResultVoid result;
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor(dalstats))
			{
				mySqlAccessor.ExecuteNonQuery("CALL SaveMission(?uid, ?name, ?mode, ?data, ?generation)", new object[]
				{
					"?uid",
					uid.ToByteArray(),
					"?name",
					name,
					"?mode",
					gameMode,
					"?data",
					data,
					"?generation",
					generation
				});
				result = new DALResultVoid(dalstats);
			}
			return result;
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00008070 File Offset: 0x00006270
		public DALResultVoid AddCurrentMission(Guid uid)
		{
			DALStats dalstats = new DALStats();
			DALResultVoid result;
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor(dalstats))
			{
				mySqlAccessor.ExecuteNonQuery("CALL AddCurrentMission(?uid)", new object[]
				{
					"?uid",
					uid.ToByteArray()
				});
				result = new DALResultVoid(dalstats);
			}
			return result;
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x000080D8 File Offset: 0x000062D8
		public DALResultVoid RemoveCurrentMission(Guid uid)
		{
			DALStats dalstats = new DALStats();
			DALResultVoid result;
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor(dalstats))
			{
				mySqlAccessor.ExecuteNonQuery("CALL RemoveCurrentMission(?uid)", new object[]
				{
					"?uid",
					uid.ToByteArray()
				});
				result = new DALResultVoid(dalstats);
			}
			return result;
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00008140 File Offset: 0x00006340
		public DALResultMulti<SoftShufflePoolData> GetSoftShufflePools()
		{
			DALStats dalstats = new DALStats();
			List<SoftShufflePoolData> list = new List<SoftShufflePoolData>();
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor(dalstats))
			{
				using (DBDataReader dbdataReader = mySqlAccessor.ExecuteReader("CALL GetSoftShufflePools()", new object[0]))
				{
					list = (List<SoftShufflePoolData>)SerializeHelper.Deserialize<SoftShufflePoolData>(dbdataReader, this.m_soft_shuffle_pool_serializer);
				}
				foreach (SoftShufflePoolData softShufflePoolData in list)
				{
					using (DBDataReader dbdataReader2 = mySqlAccessor.ExecuteReader("CALL GetSoftShuffleElements(?pool_key)", new object[]
					{
						"?pool_key",
						softShufflePoolData.m_key
					}))
					{
						softShufflePoolData.m_elements = (List<SoftShufflePoolElement>)SerializeHelper.Deserialize<SoftShufflePoolElement>(dbdataReader2, this.m_soft_shuffle_element_serializer);
					}
				}
			}
			return new DALResultMulti<SoftShufflePoolData>(list, dalstats);
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x0000826C File Offset: 0x0000646C
		public DALResultVoid SaveSoftShufflePool(SoftShufflePoolData pool)
		{
			DALStats dalstats = new DALStats();
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor(dalstats))
			{
				mySqlAccessor.ExecuteNonQuery("CALL SaveSoftShufflePool(?pool_key, ?shuffle_idx, ?marker_pos)", new object[]
				{
					"?pool_key",
					pool.m_key,
					"?shuffle_idx",
					pool.m_softShuffleIdx,
					"?marker_pos",
					pool.m_marker
				});
				mySqlAccessor.ExecuteNonQuery("CALL DeleteSoftShuffleElements(?pool_id)", new object[]
				{
					"?pool_id",
					pool.m_key
				});
				foreach (SoftShufflePoolElement softShufflePoolElement in pool.m_elements)
				{
					mySqlAccessor.ExecuteNonQuery("CALL SaveSoftShuffleElement(\t?pool_key, ?element_key, ?element_pos, ?element_usage_count)", new object[]
					{
						"?pool_key",
						pool.m_key,
						"?element_key",
						softShufflePoolElement.Key,
						"?element_pos",
						softShufflePoolElement.Pos,
						"?element_usage_count",
						softShufflePoolElement.UsageCount
					});
				}
			}
			return new DALResultVoid(dalstats);
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x000083DC File Offset: 0x000065DC
		public DALResult<int> GetGeneration()
		{
			DALStats dalstats = new DALStats();
			DALResult<int> result;
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor(dalstats))
			{
				int val = int.Parse(mySqlAccessor.ExecuteScalar("CALL GetGameStateValue('generation')", new object[0]).ToString());
				result = new DALResult<int>(val, dalstats);
			}
			return result;
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00008440 File Offset: 0x00006640
		public DALResultVoid SaveGeneration(int generation)
		{
			DALStats dalstats = new DALStats();
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor(dalstats))
			{
				mySqlAccessor.ExecuteScalar("CALL SetGameStateValue('generation', ?val)", new object[]
				{
					"?val",
					generation.ToString()
				});
			}
			return new DALResultVoid(dalstats);
		}

		// Token: 0x0400004F RID: 79
		private DAL m_dal;

		// Token: 0x04000050 RID: 80
		private MissionSerializer m_mission_serializer = new MissionSerializer();

		// Token: 0x04000051 RID: 81
		private SoftShufflePoolSerializer m_soft_shuffle_pool_serializer = new SoftShufflePoolSerializer();

		// Token: 0x04000052 RID: 82
		private SoftShuffleElementSerializer m_soft_shuffle_element_serializer = new SoftShuffleElementSerializer();
	}
}
