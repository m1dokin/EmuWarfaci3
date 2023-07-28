using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.DAL.Utils;
using MySql.Data.MySqlClient;

namespace MasterServer.Database
{
	// Token: 0x020001D1 RID: 465
	public class DatabaseManagementHelper
	{
		// Token: 0x060008C2 RID: 2242 RVA: 0x000211A0 File Offset: 0x0001F5A0
		public bool OnUpdateStage(DBUpdateStage stage, IDBUpdateService updater)
		{
			if (stage != DBUpdateStage.Schema)
			{
				if (stage != DBUpdateStage.Procedures)
				{
					if (stage == DBUpdateStage.Data)
					{
						if (!this.UpdateData(updater))
						{
							Log.Error<string, string, string>("Failed to process data for database {0} on server {1} for user {2}.", Resources.SqlDbName, Resources.SqlServerAddr, Resources.SqlLogin);
							return false;
						}
					}
				}
				else if (!this.UpdateProcedures())
				{
					Log.Error<string, string, string>("Failed to update procedures for database {0} on server {1} for user {2}.", Resources.SqlDbName, Resources.SqlServerAddr, Resources.SqlLogin);
					return false;
				}
			}
			else if (!this.UpdateSchema())
			{
				Log.Error<string, string, string>("Failed to update database {0} on server {1} for user {2}. Quit.", Resources.SqlDbName, Resources.SqlServerAddr, Resources.SqlLogin);
				return false;
			}
			return true;
		}

		// Token: 0x060008C3 RID: 2243 RVA: 0x0002124C File Offset: 0x0001F64C
		public void SyncVersionWithSlaves()
		{
			Log.Info("Syncing content with slaves...");
			string dataVersion = this.GetDataVersion();
			DateTime now = DateTime.Now;
			List<ConnectionPool> list = new List<ConnectionPool>(Resources.SlaveConnectionPools);
			int num = 0;
			while (num != list.Count)
			{
				num = 0;
				bool flag = DateTime.Now - now > this.VERSION_SYNC_TIMEOUT;
				for (int num2 = 0; num2 != list.Count; num2++)
				{
					if (this.CheckSlaveVersion(list[num2], Resources.SqlDbVersion, dataVersion))
					{
						num++;
					}
					else if (flag)
					{
						list.RemoveAt(num2);
						continue;
					}
				}
				if (num != list.Count)
				{
					Thread.Sleep(500);
				}
			}
			Resources.SetSlaveConnectionPools(list.ToArray());
		}

		// Token: 0x060008C4 RID: 2244 RVA: 0x0002131C File Offset: 0x0001F71C
		private bool CheckSlaveVersion(ConnectionPool slavePool, DBVersion master_schema, string master_data)
		{
			try
			{
				using (MySqlAccessor mySqlAccessor = new MySqlAccessor(slavePool))
				{
					using (DBDataReader dbdataReader = mySqlAccessor.ExecuteReader("SELECT * FROM versions", new object[0]))
					{
						DBSchema dbschema = new DBSchema();
						while (dbdataReader.Read())
						{
							string text = dbdataReader["version"].ToString() + '.' + dbdataReader["patch"].ToString();
							if (dbdataReader.ContainsColumn("fork"))
							{
								text = text + '.' + dbdataReader["fork"].ToString();
							}
							else
							{
								text += ".0";
							}
							DBVersion version;
							if (DBVersion.TryParse(text, out version))
							{
								dbschema.Add(version);
							}
						}
						if (master_schema != dbschema.LatestVersion)
						{
							return false;
						}
					}
					if (master_data != this.GetDataVersion())
					{
						return false;
					}
				}
			}
			catch
			{
				return false;
			}
			return true;
		}

		// Token: 0x060008C5 RID: 2245 RVA: 0x00021490 File Offset: 0x0001F890
		private string GetDataVersion()
		{
			IDALService service = ServicesManager.GetService<IDALService>();
			return service.CommonSystem.GetTotalDataVersionStamp() + service.ECatalog.GetTotalDataVersionStamp();
		}

		// Token: 0x060008C6 RID: 2246 RVA: 0x000214C0 File Offset: 0x0001F8C0
		public bool CheckDatabase()
		{
			ConfigSection section = Resources.DBMasterSettings.GetSection("Master");
			bool flag = !section.HasValue("UpdaterUser") || string.IsNullOrEmpty(section.Get("UpdaterUser"));
			if (flag)
			{
				flag = this.CreateTables(Resources.SqlServerAddr, Resources.SqlLogin, Resources.SqlPassword, Resources.SqlDbName);
			}
			else
			{
				flag = this.CreateTables(Resources.SqlServerAddr, section.Get("UpdaterUser"), section.Get("UpdaterPassword"), Resources.SqlDbName);
			}
			if (!flag)
			{
				return false;
			}
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor())
			{
				if (!mySqlAccessor.TryConnect())
				{
					return false;
				}
				if (!this.CheckEncoding(mySqlAccessor))
				{
					return false;
				}
				DBSchema sqlDbSchema = new DBSchema();
				if (!this.GetSchema(mySqlAccessor, ref sqlDbSchema))
				{
					return false;
				}
				Resources.SqlDbSchema = sqlDbSchema;
			}
			try
			{
				Type type = this.GetUpdater().GetType();
				MethodInfo method = type.GetMethod("GetUpdateDBSchema");
				Resources.DbUpdateSchema = (DBSchema)method.Invoke(this.GetUpdater(), null);
			}
			catch (Exception ex)
			{
				Log.Error(string.Format("(Exception): {0}\n{1}", ex.Message, ex.StackTrace));
				return false;
			}
			return true;
		}

		// Token: 0x060008C7 RID: 2247 RVA: 0x00021638 File Offset: 0x0001FA38
		private bool CheckEncoding(MySqlAccessor sqlAcc)
		{
			try
			{
				string query = string.Format("select default_character_set_name, default_collation_name from information_schema.SCHEMATA where schema_name='{0}'", Resources.SqlDbName);
				using (DBDataReader dbdataReader = sqlAcc.ExecuteReader(query, new object[0]))
				{
					while (dbdataReader.Read())
					{
						string text = dbdataReader["default_character_set_name"].ToString();
						string text2 = dbdataReader["default_collation_name"].ToString();
						if (text != "utf8")
						{
							Log.Error<string, string>("Database '{0}' has incorrect charset {1}, expected utf8", Resources.SqlDbName, text);
							return false;
						}
						if (text2 != "utf8_general_ci")
						{
							Log.Error<string, string>("Database '{0}' has incorrect collation {1}, expected utf8_general_ci", Resources.SqlDbName, text2);
							return false;
						}
					}
				}
			}
			catch (MySqlException e)
			{
				Log.Error(e);
			}
			return true;
		}

		// Token: 0x060008C8 RID: 2248 RVA: 0x0002172C File Offset: 0x0001FB2C
		private bool UpdateDatabaseProcessConsole(ConsoleKey key)
		{
			if (key == ConsoleKey.Y)
			{
				Console.WriteLine(string.Empty);
				Console.WriteLine("Update started.");
				this.m_isUpdateDB = true;
			}
			else if (key == ConsoleKey.N)
			{
				Console.WriteLine(string.Empty);
				Console.WriteLine("Update cancelled.");
				this.m_isUpdateDB = false;
			}
			return true;
		}

		// Token: 0x060008C9 RID: 2249 RVA: 0x00021788 File Offset: 0x0001FB88
		private object GetUpdater()
		{
			if (this.m_updater == null)
			{
				string resourceFullPath = Resources.GetResourceFullPath(Resources.ResFiles.MSA_UPDATE);
				Log.Info<string>("Try to load updater dll from {0}", resourceFullPath);
				Assembly assembly = Assembly.LoadFrom(resourceFullPath);
				this.m_updater = assembly.CreateInstance("MasterServerUpdate.Update");
			}
			return this.m_updater;
		}

		// Token: 0x060008CA RID: 2250 RVA: 0x000217D4 File Offset: 0x0001FBD4
		private bool UpdateData(IDBUpdateService updater)
		{
			bool result = false;
			try
			{
				Type type = this.GetUpdater().GetType();
				MethodInfo method = type.GetMethod("UpdateData");
				result = (bool)method.Invoke(this.GetUpdater(), new object[]
				{
					updater
				});
			}
			catch (Exception ex)
			{
				Log.Error(string.Format("(Exception) Failed to process data: {0}\n{1}", ex.Message, ex.StackTrace));
				return false;
			}
			return result;
		}

		// Token: 0x060008CB RID: 2251 RVA: 0x00021854 File Offset: 0x0001FC54
		public bool UpdateProcedures()
		{
			bool result = false;
			try
			{
				Type type = this.GetUpdater().GetType();
				MethodInfo method = type.GetMethod("UpdateProcedures");
				result = (bool)method.Invoke(this.GetUpdater(), new object[0]);
			}
			catch (Exception e)
			{
				Log.Error("(Exception) Failed to update procedures");
				Log.Error(e);
				return false;
			}
			return result;
		}

		// Token: 0x060008CC RID: 2252 RVA: 0x000218C8 File Offset: 0x0001FCC8
		private bool UpdateSchema()
		{
			if (Resources.DbUpdateSchema == Resources.SqlDbSchema)
			{
				return true;
			}
			IEnumerable<DBVersion> source;
			if (!Resources.SqlDbSchema.CanBeUpdatedTo(Resources.DbUpdateSchema, out source))
			{
				Log.Warning<string>("Database patches <{0}> are missed. Please solve this problem manually.", (from v in source
				select v.ToString()).Aggregate((string workingSentence, string next) => next + " " + workingSentence));
				return false;
			}
			if (Resources.LatestDbUpdateVersion < Resources.SqlDbVersion)
			{
				Log.Warning<DBVersion, DBVersion>("Database version {0} is higher than MasterServer version {1}. Please update MasterServer.", Resources.SqlDbVersion, Resources.LatestDbUpdateVersion);
				return false;
			}
			Console.WriteLine("Do you want to update database '{0}' from version {1} to version {2}? (Y/N)", Resources.SqlDbName, Resources.SqlDbVersion, Resources.LatestDbUpdateVersion);
			ConsoleCmdManager.ConsoleInteract(new ConsoleCmdManager.ProcessConsoleDelegate(this.UpdateDatabaseProcessConsole), new ConsoleKey[]
			{
				ConsoleKey.Y
			});
			if (!this.m_isUpdateDB)
			{
				return true;
			}
			bool flag = true;
			try
			{
				Type type = this.GetUpdater().GetType();
				MethodInfo method = type.GetMethod("UpdateSchema");
				flag = (bool)method.Invoke(this.GetUpdater(), new object[]
				{
					Resources.SqlDbSchema
				});
			}
			catch (Exception ex)
			{
				Log.Error(string.Format("(Exception) Failed to update DB: {0}\n{1}", ex.Message, ex.StackTrace));
				return false;
			}
			if (flag)
			{
				Resources.SqlDbSchema = Resources.DbUpdateSchema;
			}
			return flag;
		}

		// Token: 0x060008CD RID: 2253 RVA: 0x00021A54 File Offset: 0x0001FE54
		private bool RebuildDatabaseProcessConsole(ConsoleKey key)
		{
			if (key == ConsoleKey.Y)
			{
				Console.WriteLine(string.Empty);
				Console.WriteLine("Rebuild started.");
			}
			else if (key == ConsoleKey.N)
			{
				Resources.IsForceUpdateDatabase = false;
				Console.WriteLine(string.Empty);
				Console.WriteLine("Rebuild cancelled.");
			}
			return true;
		}

		// Token: 0x060008CE RID: 2254 RVA: 0x00021AA8 File Offset: 0x0001FEA8
		private bool CreateTablesProcessConsole(ConsoleKey key)
		{
			if (key == ConsoleKey.Y)
			{
				Console.WriteLine(string.Empty);
				Console.WriteLine("Create started.");
				this.m_isCreateNewDb = true;
			}
			else if (key == ConsoleKey.N)
			{
				Console.WriteLine(string.Empty);
				Console.WriteLine("Create cancelled.");
				this.m_isCreateNewDb = false;
			}
			return true;
		}

		// Token: 0x060008CF RID: 2255 RVA: 0x00021B04 File Offset: 0x0001FF04
		private bool CreateTables(string srv, string user, string pwd, string dbName)
		{
			ConnectionPool.Config @default = ConnectionPool.Config.Default;
			@default.ConnectionString = new MySqlConnectionStringBuilder(Resources.MasterConnectionPool.ConnectionConfig.ConnectionString)
			{
				Database = string.Empty,
				UserID = user,
				Password = pwd
			}.ToString();
			using (ConnectionPool connectionPool = new ConnectionPool())
			{
				connectionPool.Init(@default);
				using (MySqlAccessor mySqlAccessor = new MySqlAccessor(connectionPool))
				{
					if (Resources.IsForceUpdateDatabase)
					{
						Console.WriteLine("You are running in force_db_update mode, do you wish to reset all data in '{0}'? (Y/N)", dbName);
						ConsoleCmdManager.ConsoleInteract(new ConsoleCmdManager.ProcessConsoleDelegate(this.RebuildDatabaseProcessConsole), new ConsoleKey[]
						{
							ConsoleKey.N
						});
					}
					if (Resources.IsForceUpdateDatabase)
					{
						mySqlAccessor.ExecuteNonQuery(string.Format("DROP DATABASE IF EXISTS `{0}`", Resources.SqlDbName), new object[0]);
					}
					try
					{
						mySqlAccessor.ExecuteScalar(string.Format("SELECT id FROM {0}.versions LIMIT 1", Resources.SqlDbName), new object[0]);
						return true;
					}
					catch
					{
						if (!Resources.DBUpdaterPermission)
						{
							Log.Error<string>("Database '{0}' does not exist, run with db_updater permissions to create one", dbName);
							return false;
						}
						Console.WriteLine("Database '{0}' does not exist, do you wish to create it? (Y/N)", dbName);
						ConsoleCmdManager.ConsoleInteract(new ConsoleCmdManager.ProcessConsoleDelegate(this.CreateTablesProcessConsole), new ConsoleKey[]
						{
							ConsoleKey.Y
						});
						if (!this.m_isCreateNewDb)
						{
							return false;
						}
						mySqlAccessor.ExecuteNonQuery(string.Format("CREATE DATABASE IF NOT EXISTS {0}", Resources.SqlDbName), new object[0]);
					}
				}
			}
			return true;
		}

		// Token: 0x060008D0 RID: 2256 RVA: 0x00021CDC File Offset: 0x000200DC
		private bool GetSchema(MySqlAccessor sqlAcc, ref DBSchema currentSchema)
		{
			try
			{
				using (DBDataReader dbdataReader = sqlAcc.ExecuteReader("SELECT * FROM versions", new object[0]))
				{
					while (dbdataReader.Read())
					{
						string text = dbdataReader["version"].ToString() + '.' + dbdataReader["patch"].ToString();
						if (dbdataReader.ContainsColumn("fork"))
						{
							text = text + '.' + dbdataReader["fork"].ToString();
						}
						else
						{
							text += ".0";
						}
						DBVersion version;
						if (!DBVersion.TryParse(text, out version))
						{
							Log.Warning<string>("Database '{0}' has not correct version info!", Resources.SqlDbName);
							return false;
						}
						currentSchema.Add(version);
					}
				}
			}
			catch (MySqlException)
			{
				return true;
			}
			catch (Exception e)
			{
				Log.Error(e);
				return false;
			}
			return true;
		}

		// Token: 0x0400051D RID: 1309
		private bool m_isUpdateDB;

		// Token: 0x0400051E RID: 1310
		private bool m_isCreateNewDb;

		// Token: 0x0400051F RID: 1311
		private object m_updater;

		// Token: 0x04000520 RID: 1312
		private TimeSpan VERSION_SYNC_TIMEOUT = new TimeSpan(0, 0, 30);
	}
}
