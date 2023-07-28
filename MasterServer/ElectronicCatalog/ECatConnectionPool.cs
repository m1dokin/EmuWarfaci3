using System;
using System.Data;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Database;
using MySql.Data.MySqlClient;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x02000231 RID: 561
	public class ECatConnectionPool : ConfigurableConnectionPool
	{
		// Token: 0x06000BEA RID: 3050 RVA: 0x0002D5FC File Offset: 0x0002B9FC
		public void Init()
		{
			ConfigSection section = Resources.ECatalogSettings.GetSection("Pooling");
			ConfigSection section2 = Resources.ECatalogSettings.GetSection("ECatalog");
			MySqlConnectionStringBuilder mySqlConnectionStringBuilder = new MySqlConnectionStringBuilder();
			mySqlConnectionStringBuilder.UserID = section2.Get("User");
			mySqlConnectionStringBuilder.Password = section2.Get("Password");
			mySqlConnectionStringBuilder.Server = section2.Get("Server");
			mySqlConnectionStringBuilder.Database = section2.Get("Database");
			mySqlConnectionStringBuilder.Pooling = false;
			mySqlConnectionStringBuilder.AllowUserVariables = true;
			mySqlConnectionStringBuilder.AllowZeroDateTime = true;
			mySqlConnectionStringBuilder.CharacterSet = "utf8";
			ConnectionPool.Config @default = ConnectionPool.Config.Default;
			@default.MaxTimeInUse = new TimeSpan(0, 0, int.Parse(section.Get("MaxTimeInUse")));
			@default.MaxConnections = int.Parse(section.Get("MaxConnections"));
			@default.MinConnections = int.Parse(section.Get("MinConnections"));
			@default.InactivityTimeout = new TimeSpan(0, 0, int.Parse(section.Get("InactivityTimeout")));
			@default.AllocationTimeout = new TimeSpan(0, 0, int.Parse(section.Get("AllocationTimeout")));
			@default.Pooling = true;
			if (section.HasValue("IsolationLevel"))
			{
				@default.TransactionLevel = (IsolationLevel)Enum.Parse(typeof(IsolationLevel), section.Get("IsolationLevel"));
			}
			if (!uint.TryParse(section.Get("ConnectionTimeout"), out @default.ConnectionTimeout))
			{
				@default.ConnectionTimeout = ConnectionPool.Config.Default.ConnectionTimeout;
			}
			if (!int.TryParse(section.Get("CommandTimeout"), out @default.CommandTimeout))
			{
				@default.CommandTimeout = ConnectionPool.Config.Default.CommandTimeout;
			}
			mySqlConnectionStringBuilder.ConnectionTimeout = @default.ConnectionTimeout;
			@default.ConnectionString = mySqlConnectionStringBuilder.ToString();
			base.Init(@default, section);
		}
	}
}
