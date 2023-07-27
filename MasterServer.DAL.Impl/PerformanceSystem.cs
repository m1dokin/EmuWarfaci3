using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MasterServer.Database;
using Util.Common;

namespace MasterServer.DAL.Impl
{
	// Token: 0x02000018 RID: 24
	internal class PerformanceSystem : IPerformanceSystem
	{
		// Token: 0x060000E2 RID: 226 RVA: 0x00008738 File Offset: 0x00006938
		public PerformanceSystem(DAL dal)
		{
			this.m_dal = dal;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x0000876C File Offset: 0x0000696C
		public DALResult<ProfilePerformance> GetProfilePerformance(ulong profile_id, List<Guid> current_missions)
		{
			DALStats stats = new DALStats();
			CacheProxy.Options<ProfilePerformance> options = new CacheProxy.Options<ProfilePerformance>
			{
				stats = stats,
				get_data = delegate()
				{
					ProfilePerformance result = default(ProfilePerformance);
					result.Missions = new List<ProfilePerformance.MissionPerfInfo>();
					result.ProfileID = profile_id;
					using (MySqlAccessor mySqlAccessor = new MySqlAccessor(stats))
					{
						using (DBDataReader dbdataReader = mySqlAccessor.ExecuteReader("CALL GetProfilePerformance(?pid)", new object[]
						{
							"?pid",
							profile_id
						}))
						{
							while (dbdataReader.Read())
							{
								Guid mission = (Guid)dbdataReader["mission_id"];
								ProfilePerformance.MissionPerfInfo item = result.Missions.Find((ProfilePerformance.MissionPerfInfo X) => X.MissionID == mission);
								if (item.Performances == null)
								{
									item.MissionID = mission;
									item.Performances = new List<PerformanceInfo>();
									item.Status = ((!ParseUtils.ParseBool(dbdataReader["finished"].ToString())) ? MissionStatus.Failed : MissionStatus.Finished);
									result.Missions.Add(item);
								}
								PerformanceInfo item2 = default(PerformanceInfo);
								item2.Stat = uint.Parse(dbdataReader["stat_type"].ToString());
								item2.Performance = uint.Parse(dbdataReader["performance"].ToString());
								item.Performances.Add(item2);
							}
						}
					}
					return result;
				}
			};
			return this.m_dal.CacheProxy.Get<ProfilePerformance>(options);
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x000087C8 File Offset: 0x000069C8
		public DALResultVoid CleanupMissionPerformance()
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL CleanupMissionPerformance()", new object[0]);
			setOptions.db_transaction = true;
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00008804 File Offset: 0x00006A04
		public DALResult<MasterRecord> GetPerformanceMasterRecord(string type)
		{
			DALStats stats = new DALStats();
			CacheProxy.Options<MasterRecord> options = new CacheProxy.Options<MasterRecord>
			{
				stats = stats,
				get_data = delegate()
				{
					MasterRecord result = default(MasterRecord);
					result.Records = new List<MasterRecord.Record>();
					using (MySqlAccessor mySqlAccessor = new MySqlAccessor(stats))
					{
						using (DBDataReader dbdataReader = mySqlAccessor.ExecuteReader("CALL GetPerformanceMasterRecord(?type)", new object[]
						{
							"?type",
							type
						}))
						{
							while (dbdataReader.Read())
							{
								string mission = dbdataReader["stat_id"].ToString();
								uint stat = uint.Parse(dbdataReader["stat_type"].ToString());
								byte[] array = (byte[])dbdataReader["samples"];
								MasterRecord.Record item = result.Records.Find((MasterRecord.Record X) => X.RecordID == mission);
								if (item.StatSamples == null)
								{
									item = default(MasterRecord.Record);
									item.RecordID = mission;
									ulong utc = ulong.Parse(dbdataReader["last_update"].ToString());
									item.LastUpdateUtc = TimeUtils.UTCTimestampToUTCTime(utc);
									item.StatSamples = new List<MasterRecord.StatSamples>();
									result.Records.Add(item);
								}
								MasterRecord.StatSamples item2 = item.StatSamples.Find((MasterRecord.StatSamples X) => X.Stat == stat);
								if (item2.Samples == null)
								{
									item2 = default(MasterRecord.StatSamples);
									item2.Stat = stat;
									item2.Samples = new List<KeyValuePair<float, float>>();
									item.StatSamples.Add(item2);
								}
								using (MemoryStream memoryStream = new MemoryStream(array))
								{
									using (BinaryReader binaryReader = new BinaryReader(memoryStream))
									{
										for (int i = array.Length / 8; i > 0; i--)
										{
											float key = binaryReader.ReadSingle();
											float value = binaryReader.ReadSingle();
											item2.Samples.Add(new KeyValuePair<float, float>(key, value));
										}
									}
								}
							}
						}
					}
					DateTime minLastUpdateUtc;
					if (result.Records.Count != 0)
					{
						minLastUpdateUtc = (from X in result.Records
						select X.LastUpdateUtc).Min<DateTime>();
					}
					else
					{
						minLastUpdateUtc = TimeUtils.UTCZero;
					}
					result.MinLastUpdateUtc = minLastUpdateUtc;
					return result;
				}
			};
			return this.m_dal.CacheProxy.Get<MasterRecord>(options);
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00008860 File Offset: 0x00006A60
		private DALResultVoid UpdateMissionPerformance(DALStats stats, Guid missionId, ulong profileId, uint stat, uint performance, MissionStatus missionStatus)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				stats = stats
			};
			setOptions.query("CALL UpdateMissionPerformance(?mission, ?pid, ?stat, ?perf, ?fin)", new object[]
			{
				"?mission",
				missionId.ToByteArray(),
				"?pid",
				profileId,
				"?stat",
				stat,
				"?perf",
				performance,
				"?fin",
				(missionStatus != MissionStatus.Finished) ? 0 : 1
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00008908 File Offset: 0x00006B08
		public DALResultVoid UpdateMissionPerformance(PerformanceUpdate update, List<Guid> current_missions)
		{
			DALStats stats = new DALStats();
			foreach (ulong profileId in update.ProfilesIds)
			{
				foreach (PerformanceInfo performanceInfo in update.Performances)
				{
					this.UpdateMissionPerformance(stats, update.MissionID, profileId, performanceInfo.Stat, performanceInfo.Performance, update.Status);
					if (update.Status == MissionStatus.Failed)
					{
						break;
					}
				}
			}
			return new DALResultVoid(stats);
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x000089E8 File Offset: 0x00006BE8
		public DALResult<bool> TryBeginUpdate(string onlineId, string lockRecord, TimeSpan updateFreq)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("SELECT LockPerformanceRecords(?lockmp, ?lock, ?freq_sec)", new object[]
			{
				"?lockmp",
				onlineId,
				"?lock",
				lockRecord,
				"?freq_sec",
				(int)updateFreq.TotalSeconds
			});
			setOptions.db_transaction = true;
			bool val = int.Parse(this.m_dal.CacheProxy.SetScalar(setOptions).ToString()) > 0;
			return new DALResult<bool>(val, setOptions.stats);
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00008A70 File Offset: 0x00006C70
		public DALResultVoid EndUpdate(List<MasterRecord.Record> mission_records, string lockRecord)
		{
			DALStats stats = new DALStats();
			foreach (MasterRecord.Record record in mission_records)
			{
				foreach (MasterRecord.StatSamples statSamples in record.StatSamples)
				{
					using (MemoryStream memoryStream = new MemoryStream(statSamples.Samples.Count * 4 * 2))
					{
						using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
						{
							for (int num = 0; num != statSamples.Samples.Count; num++)
							{
								binaryWriter.Write(statSamples.Samples[num].Key);
								binaryWriter.Write(statSamples.Samples[num].Value);
							}
							this.m_dal.ValidateFixedSizeColumnData("performance_master_records", "samples", (int)memoryStream.Length);
							CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
							{
								stats = stats
							};
							setOptions.query("CALL UpdateMissionPerformanceRecord(?mission, ?stat, ?samples)", new object[]
							{
								"?mission",
								record.RecordID.ToCharArray(),
								"?stat",
								statSamples.Stat,
								"?samples",
								memoryStream.GetBuffer()
							});
							this.m_dal.CacheProxy.Set(setOptions);
						}
					}
				}
			}
			CacheProxy.SetOptions setOptions2 = new CacheProxy.SetOptions
			{
				stats = stats
			};
			setOptions2.query("CALL UnlockPerformanceRecords(?lock);", new object[]
			{
				"?lock",
				lockRecord
			});
			return this.m_dal.CacheProxy.Set(setOptions2);
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00008CD4 File Offset: 0x00006ED4
		public DALResult<bool> SetMissionProfileWin(Guid missionId, ulong profileId)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("SELECT SetMissionProfileWin(?mission, ?pid)", new object[]
			{
				"?mission",
				missionId.ToByteArray(),
				"?pid",
				profileId
			});
			bool val = int.Parse(this.m_dal.CacheProxy.SetScalar(setOptions).ToString()) > 0;
			return new DALResult<bool>(val, setOptions.stats);
		}

		// Token: 0x04000055 RID: 85
		private readonly DAL m_dal;

		// Token: 0x04000056 RID: 86
		private readonly Random m_random = new Random((int)DateTime.Now.Ticks);

		// Token: 0x02000019 RID: 25
		private class PivotSerializer<T> : IDBSerializer<IEnumerable<T>>
		{
			// Token: 0x060000EB RID: 235 RVA: 0x00008D46 File Offset: 0x00006F46
			public PivotSerializer(IEnumerable<IDBSerializer<T>> serializers)
			{
				this.m_serializers = serializers;
			}

			// Token: 0x060000EC RID: 236 RVA: 0x00008D58 File Offset: 0x00006F58
			public void Deserialize(IDataReaderEx reader, out IEnumerable<T> ret)
			{
				List<T> list = new List<T>();
				foreach (IDBSerializer<T> idbserializer in this.m_serializers)
				{
					T item;
					idbserializer.Deserialize(reader, out item);
					list.Add(item);
				}
				ret = list;
			}

			// Token: 0x04000057 RID: 87
			private readonly IEnumerable<IDBSerializer<T>> m_serializers;
		}
	}
}
