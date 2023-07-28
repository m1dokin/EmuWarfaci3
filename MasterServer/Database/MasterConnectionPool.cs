using System;
using System.Data;
using MasterServer.Core.Configuration;
using MySql.Data.MySqlClient;

namespace MasterServer.Database
{
	// Token: 0x020001BB RID: 443
	internal class MasterConnectionPool : ConfigurableConnectionPool
	{
		// Token: 0x06000857 RID: 2135 RVA: 0x0002071C File Offset: 0x0001EB1C
		public void Init(ConfigSection connection, ConfigSection pooling, bool pool)
		{
			MySqlConnectionStringBuilder mySqlConnectionStringBuilder = new MySqlConnectionStringBuilder();
			mySqlConnectionStringBuilder.UserID = connection.Get("User");
			mySqlConnectionStringBuilder.Password = connection.Get("Password");
			mySqlConnectionStringBuilder.Server = connection.Get("Server");
			mySqlConnectionStringBuilder.Database = connection.Get("Database");
			mySqlConnectionStringBuilder.Pooling = false;
			mySqlConnectionStringBuilder.AllowUserVariables = true;
			mySqlConnectionStringBuilder.AllowZeroDateTime = true;
			mySqlConnectionStringBuilder.CharacterSet = "utf8";
			ConnectionPool.Config @default = ConnectionPool.Config.Default;
			@default.MaxTimeInUse = new TimeSpan(0, 0, int.Parse(pooling.Get("MaxTimeInUse")));
			@default.MaxConnections = int.Parse(pooling.Get("MaxConnections"));
			@default.MinConnections = int.Parse(pooling.Get("MinConnections"));
			@default.InactivityTimeout = new TimeSpan(0, 0, int.Parse(pooling.Get("InactivityTimeout")));
			@default.AllocationTimeout = new TimeSpan(0, 0, int.Parse(pooling.Get("AllocationTimeout")));
			@default.Pooling = pool;
			if (pooling.HasValue("IsolationLevel"))
			{
				@default.TransactionLevel = (IsolationLevel)Enum.Parse(typeof(IsolationLevel), pooling.Get("IsolationLevel"));
			}
			if (!uint.TryParse(pooling.Get("ConnectionTimeout"), out @default.ConnectionTimeout))
			{
				@default.ConnectionTimeout = ConnectionPool.Config.Default.ConnectionTimeout;
			}
			if (!int.TryParse(pooling.Get("CommandTimeout"), out @default.CommandTimeout))
			{
				@default.CommandTimeout = ConnectionPool.Config.Default.CommandTimeout;
			}
			mySqlConnectionStringBuilder.ConnectionTimeout = @default.ConnectionTimeout;
			@default.ConnectionString = mySqlConnectionStringBuilder.ToString();
			base.Init(@default, pooling);
		}
	}
}
