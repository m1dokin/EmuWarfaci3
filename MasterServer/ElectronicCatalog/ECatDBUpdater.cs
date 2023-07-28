using System;
using System.IO;
using System.Linq;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.DAL.Utils;
using MasterServer.Database;
using MySql.Data.MySqlClient;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x02000232 RID: 562
	internal class ECatDBUpdater
	{
		// Token: 0x06000BEB RID: 3051 RVA: 0x0002D7DC File Offset: 0x0002BBDC
		public ECatDBUpdater()
		{
			this.m_updatesDir = Path.Combine(Resources.GetUpdateDirectory(), "ecatalog");
			this.m_proceduresDir = Path.Combine(this.m_updatesDir, "procedures");
			this.m_connection_config = Resources.ECatalogSettings.GetSection("ECatalog");
			if (this.m_connection_config.HasValue("UpdateTimeout"))
			{
				this.m_connection_config.Get("UpdateTimeout", out this.m_updaterTimeout);
			}
			this.ReadLocalSchema();
			this.ReadServerSchema();
		}

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x06000BEC RID: 3052 RVA: 0x0002D871 File Offset: 0x0002BC71
		// (set) Token: 0x06000BED RID: 3053 RVA: 0x0002D879 File Offset: 0x0002BC79
		public DBSchema LocalSchema { get; private set; }

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x06000BEE RID: 3054 RVA: 0x0002D882 File Offset: 0x0002BC82
		// (set) Token: 0x06000BEF RID: 3055 RVA: 0x0002D88A File Offset: 0x0002BC8A
		public DBSchema ServerSchema { get; private set; }

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x06000BF0 RID: 3056 RVA: 0x0002D894 File Offset: 0x0002BC94
		public bool HasDBUpdateRights
		{
			get
			{
				ConfigSection section = Resources.ECatalogSettings.GetSection("ECatalog");
				bool flag = !string.IsNullOrEmpty(section.Get("UpdaterUser")) && !string.IsNullOrEmpty(section.Get("UpdaterPassword"));
				return flag && (!this.HasStandaloneDB || Resources.RealmDBUpdaterPermission);
			}
		}

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x06000BF1 RID: 3057 RVA: 0x0002D8F9 File Offset: 0x0002BCF9
		public bool HasStandaloneDB
		{
			get
			{
				return Resources.SqlServerAddr != this.m_connection_config.Get("Server") || Resources.SqlDbName != this.m_connection_config.Get("Database");
			}
		}

		// Token: 0x06000BF2 RID: 3058 RVA: 0x0002D938 File Offset: 0x0002BD38
		public void CreateDatabase()
		{
			using (ConnectionPool connectionPool = this.CreateConnectionPoolGlobal())
			{
				using (MySqlAccessor mySqlAccessor = new MySqlAccessor(connectionPool))
				{
					mySqlAccessor.ExecuteNonQuery(string.Format("CREATE DATABASE IF NOT EXISTS {0}", this.m_connection_config.Get("Database")), new object[0]);
				}
			}
		}

		// Token: 0x06000BF3 RID: 3059 RVA: 0x0002D9BC File Offset: 0x0002BDBC
		public void UpdateDatabase()
		{
			if (this.LocalSchema.Equals(this.ServerSchema))
			{
				return;
			}
			using (ConnectionPool connectionPool = this.CreateConnectionPool())
			{
				using (MySqlAccessor mySqlAccessor = new MySqlAccessor(connectionPool))
				{
					foreach (DBVersion dbversion in this.LocalSchema.Versions)
					{
						if (!this.ServerSchema.Versions.Contains(dbversion))
						{
							string path = Path.Combine(this.m_updatesDir, dbversion.ToString());
							foreach (string text in Directory.GetFiles(path, "*.sql"))
							{
								string name = string.Format("{0}/{1}", dbversion, Path.GetFileName(text));
								this.InstallUpdate(text, name, mySqlAccessor);
							}
							Log.Info<DBVersion>("ECatalog update {0} installed successfully", dbversion);
							this.ServerSchema.Add(dbversion);
						}
					}
				}
			}
		}

		// Token: 0x06000BF4 RID: 3060 RVA: 0x0002DB18 File Offset: 0x0002BF18
		private void InstallUpdate(string file, string name, MySqlAccessor acc)
		{
			try
			{
				Log.Info<string>("Executing Ecatalog update {0} ...", name);
				string query = this.ReadScript(file);
				acc.ExecuteNonQuery(query, new object[0]);
			}
			catch (Exception ex)
			{
				Log.Error<string, string>("Failed to install ECatalog update {0}: {1}", name, ex.Message);
				throw;
			}
		}

		// Token: 0x06000BF5 RID: 3061 RVA: 0x0002DB70 File Offset: 0x0002BF70
		public void UpdateProcedures()
		{
			Log.Info("Updating Ecatalog procedures");
			if (this.HasStandaloneDB)
			{
				Log.Info("Clearing ECatalog DB procedures");
				this.DropAllProcedures();
			}
			using (ConnectionPool connectionPool = this.CreateConnectionPool())
			{
				using (MySqlAccessor mySqlAccessor = new MySqlAccessor(connectionPool))
				{
					foreach (string text in Directory.GetFiles(this.m_proceduresDir, "*.sql"))
					{
						this.InstallUpdate(text, Path.GetFileName(text), mySqlAccessor);
						Log.Info<string>("ECatalog procedures update {0} installed successfully", Path.GetFileName(text));
					}
				}
			}
		}

		// Token: 0x06000BF6 RID: 3062 RVA: 0x0002DC3C File Offset: 0x0002C03C
		private void DropAllProcedures()
		{
			using (ConnectionPool connectionPool = this.CreateConnectionPool())
			{
				using (MySqlAccessor mySqlAccessor = new MySqlAccessor(connectionPool))
				{
					string text = string.Empty;
					using (DBDataReader dbdataReader = mySqlAccessor.ExecuteReader("SELECT CONCAT('DROP ',ROUTINE_TYPE,' `',ROUTINE_NAME,'`') as stmt FROM information_schema.ROUTINES where ROUTINE_SCHEMA = ?schema", new object[]
					{
						"?schema",
						this.m_connection_config.Get("Database")
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
			}
		}

		// Token: 0x06000BF7 RID: 3063 RVA: 0x0002DD2C File Offset: 0x0002C12C
		private string ReadScript(string filename)
		{
			string text = File.ReadAllText(filename);
			return text.Replace("DELIMITER ;;", string.Empty).Replace("DELIMITER ;", string.Empty).Replace(";;", ";");
		}

		// Token: 0x06000BF8 RID: 3064 RVA: 0x0002DD70 File Offset: 0x0002C170
		private void ReadLocalSchema()
		{
			this.LocalSchema = new DBSchema();
			foreach (string path in Directory.GetDirectories(this.m_updatesDir))
			{
				DBVersion version;
				if (DBVersion.TryParse(Path.GetFileName(path), out version) && Directory.GetFiles(path, "*.sql").Length != 0)
				{
					this.LocalSchema.Add(version);
				}
			}
		}

		// Token: 0x06000BF9 RID: 3065 RVA: 0x0002DDE4 File Offset: 0x0002C1E4
		private void ReadServerSchema()
		{
			try
			{
				using (ConnectionPool connectionPool = this.CreateConnectionPoolRO())
				{
					using (MySqlAccessor mySqlAccessor = new MySqlAccessor(connectionPool))
					{
						this.ServerSchema = new DBSchema();
						using (DBDataReader dbdataReader = mySqlAccessor.ExecuteReader("SELECT version FROM ecat_versions", new object[0]))
						{
							DBVersion rhs = this.LocalSchema.Versions.First<DBVersion>();
							while (dbdataReader.Read())
							{
								DBVersion dbversion;
								if (DBVersion.TryParse(dbdataReader["version"].ToString(), out dbversion) && !(dbversion < rhs))
								{
									this.ServerSchema.Add(dbversion);
								}
							}
						}
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000BFA RID: 3066 RVA: 0x0002DEF0 File Offset: 0x0002C2F0
		public bool CheckEncoding()
		{
			try
			{
				using (ConnectionPool connectionPool = this.CreateConnectionPoolRO())
				{
					using (MySqlAccessor mySqlAccessor = new MySqlAccessor(connectionPool))
					{
						string query = string.Format("select default_character_set_name, default_collation_name from information_schema.SCHEMATA where schema_name='{0}'", this.m_connection_config.Get("Database"));
						using (DBDataReader dbdataReader = mySqlAccessor.ExecuteReader(query, new object[0]))
						{
							while (dbdataReader.Read())
							{
								string text = dbdataReader["default_character_set_name"].ToString();
								string text2 = dbdataReader["default_collation_name"].ToString();
								if (text != "utf8")
								{
									Log.Error<string, string>("Database '{0}' has incorrect charset {1}, expected utf8", this.m_connection_config.Get("Database"), text);
									return false;
								}
								if (text2 != "utf8_general_ci")
								{
									Log.Error<string, string>("Database '{0}' has incorrect collation {1}, expected utf8_general_ci", this.m_connection_config.Get("Database"), text2);
									return false;
								}
							}
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

		// Token: 0x06000BFB RID: 3067 RVA: 0x0002E084 File Offset: 0x0002C484
		private ConnectionPool CreateConnectionPool()
		{
			CustomConnectionPool customConnectionPool = new CustomConnectionPool();
			customConnectionPool.Init(this.m_connection_config.Get("Server"), this.m_connection_config.Get("Database"), this.m_connection_config.Get("UpdaterUser"), this.m_connection_config.Get("UpdaterPassword"), this.m_updaterTimeout);
			return customConnectionPool;
		}

		// Token: 0x06000BFC RID: 3068 RVA: 0x0002E0E4 File Offset: 0x0002C4E4
		private ConnectionPool CreateConnectionPoolRO()
		{
			CustomConnectionPool customConnectionPool = new CustomConnectionPool();
			customConnectionPool.Init(this.m_connection_config.Get("Server"), this.m_connection_config.Get("Database"), this.m_connection_config.Get("User"), this.m_connection_config.Get("Password"), this.m_updaterTimeout);
			return customConnectionPool;
		}

		// Token: 0x06000BFD RID: 3069 RVA: 0x0002E144 File Offset: 0x0002C544
		private ConnectionPool CreateConnectionPoolGlobal()
		{
			CustomConnectionPool customConnectionPool = new CustomConnectionPool();
			customConnectionPool.Init(this.m_connection_config.Get("Server"), string.Empty, this.m_connection_config.Get("UpdaterUser"), this.m_connection_config.Get("UpdaterPassword"), this.m_updaterTimeout);
			return customConnectionPool;
		}

		// Token: 0x0400058D RID: 1421
		private string m_updatesDir;

		// Token: 0x0400058E RID: 1422
		private string m_proceduresDir;

		// Token: 0x0400058F RID: 1423
		private ConfigSection m_connection_config;

		// Token: 0x04000590 RID: 1424
		private int m_updaterTimeout = 1800;
	}
}
