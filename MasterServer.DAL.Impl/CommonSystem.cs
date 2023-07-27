using System;
using System.Collections.Generic;
using MasterServer.Core;
using MasterServer.Database;
using MySql.Data.MySqlClient;

namespace MasterServer.DAL.Impl
{
	// Token: 0x0200000C RID: 12
	internal class CommonSystem : ICommonSystem
	{
		// Token: 0x06000046 RID: 70 RVA: 0x00003A05 File Offset: 0x00001C05
		public CommonSystem(DAL dal)
		{
			this.m_dal = dal;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00003A2C File Offset: 0x00001C2C
		public DALResultVoid UpdateServer(string msId, SServerEntity entity)
		{
			DALStats dalstats = new DALStats();
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor(dalstats))
			{
				mySqlAccessor.ExecuteNonQuery("CALL UpdateServer(?srvid, ?host, ?port, ?node, ?mode, ?status, ?mission, ?oid, ?msid, ?build_type, ?perf)", new object[]
				{
					"?srvid",
					entity.ServerId,
					"?host",
					entity.Hostname,
					"?port",
					entity.Port,
					"?node",
					entity.Node,
					"?mode",
					entity.Mode,
					"?status",
					entity.Status,
					"?mission",
					entity.MissionKey,
					"?oid",
					entity.OnlineId,
					"?msid",
					msId,
					"?build_type",
					entity.BuildType,
					"?perf",
					entity.PerformanceIndex
				});
			}
			return new DALResultVoid(dalstats);
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00003B60 File Offset: 0x00001D60
		public DALResultMulti<SServerEntity> GetFreeServers(string msId)
		{
			DALStats dalstats = new DALStats();
			DALResultMulti<SServerEntity> result;
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor(dalstats))
			{
				using (DBDataReader dbdataReader = mySqlAccessor.ExecuteReader("CALL GetFreeServers(?msid)", new object[]
				{
					"?msid",
					msId
				}))
				{
					IEnumerable<SServerEntity> val = SerializeHelper.Deserialize<SServerEntity>(dbdataReader, this.m_server_entity_serializer);
					result = new DALResultMulti<SServerEntity>(val, dalstats);
				}
			}
			return result;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00003BEC File Offset: 0x00001DEC
		public DALResultVoid DebugExecuteNoQuery(string sql)
		{
			DALStats dalstats = new DALStats();
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor(dalstats))
			{
				mySqlAccessor.ExecuteNonQuery(sql, new object[0]);
			}
			return new DALResultVoid(dalstats);
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00003C3C File Offset: 0x00001E3C
		public DALResultVoid DebugDropProcedure(string procedureName)
		{
			DALStats dalstats = new DALStats();
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor(dalstats))
			{
				string text = string.Empty;
				using (DBDataReader dbdataReader = mySqlAccessor.ExecuteReader("SELECT CONCAT('DROP ',ROUTINE_TYPE,' `',ROUTINE_NAME,'`') as stmt FROM information_schema.ROUTINES where ROUTINE_SCHEMA = ?schema AND ROUTINE_NAME = ?routine_name", new object[]
				{
					"?schema",
					Resources.SqlDbName,
					"?routine_name",
					procedureName
				}))
				{
					while (dbdataReader.Read())
					{
						text = text + dbdataReader[0].ToString() + ';';
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					mySqlAccessor.ExecuteNonQuery(text, new object[0]);
				}
			}
			return new DALResultVoid(dalstats);
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00003D14 File Offset: 0x00001F14
		public DALResultMulti<SVersionStamp> GetDataVersionStamps()
		{
			DALStats dalstats = new DALStats();
			DALResultMulti<SVersionStamp> result;
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor(dalstats))
			{
				using (DBDataReader dbdataReader = mySqlAccessor.ExecuteReader("CALL GetDataVersionStamps()", new object[0]))
				{
					IEnumerable<SVersionStamp> val = SerializeHelper.Deserialize<SVersionStamp>(dbdataReader, this.m_version_stamp_serializer);
					result = new DALResultMulti<SVersionStamp>(val, dalstats);
				}
			}
			return result;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00003D94 File Offset: 0x00001F94
		public DALResultVoid SetDataVersionStamp(string group, string hash)
		{
			DALStats dalstats = new DALStats();
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor(dalstats))
			{
				mySqlAccessor.ExecuteNonQuery("CALL SetDataVersionStamp(?group, ?hash)", new object[]
				{
					"?group",
					group,
					"?hash",
					hash
				});
			}
			return new DALResultVoid(dalstats);
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00003E00 File Offset: 0x00002000
		public DALResult<string> GetTotalDataVersionStamp()
		{
			DALStats dalstats = new DALStats();
			DALResult<string> result;
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor(dalstats))
			{
				object obj = mySqlAccessor.ExecuteScalar("CALL GetTotalDataVersionStamp()", new object[0]);
				result = new DALResult<string>(obj.ToString(), dalstats);
			}
			return result;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00003E5C File Offset: 0x0000205C
		public DALResult<bool> TryLockUpdaterPermission(string onlineId)
		{
			DALStats dalstats = new DALStats();
			bool val = false;
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor(dalstats))
			{
				try
				{
					object obj = mySqlAccessor.ExecuteScalar("SELECT LockUpdaterPermission(?oid)", new object[]
					{
						"?oid",
						onlineId
					});
					val = (int.Parse(obj.ToString()) != 0);
				}
				catch (MySqlException)
				{
					val = true;
				}
			}
			return new DALResult<bool>(val, dalstats);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00003EEC File Offset: 0x000020EC
		public DALResultVoid UnlockUpdaterPermission(string onlineId)
		{
			DALStats dalstats = new DALStats();
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor(dalstats))
			{
				mySqlAccessor.ExecuteNonQuery("CALL UnlockUpdaterPermission(?oid)", new object[]
				{
					"?oid",
					onlineId
				});
			}
			return new DALResultVoid(dalstats);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00003F4C File Offset: 0x0000214C
		public DALResultVoid ResetUpdaterPermission()
		{
			DALStats dalstats = new DALStats();
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor(dalstats))
			{
				mySqlAccessor.ExecuteNonQuery("CALL ResetUpdaterPermission()", new object[0]);
			}
			return new DALResultVoid(dalstats);
		}

		// Token: 0x04000019 RID: 25
		private DAL m_dal;

		// Token: 0x0400001A RID: 26
		private ServerEntitySerializer m_server_entity_serializer = new ServerEntitySerializer();

		// Token: 0x0400001B RID: 27
		private VersionStampSerializer m_version_stamp_serializer = new VersionStampSerializer();
	}
}
