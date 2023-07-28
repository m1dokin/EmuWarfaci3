using System;
using System.Data;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MySql.Data.MySqlClient;

namespace MasterServer.Database
{
	// Token: 0x020001BC RID: 444
	public class CustomConnectionPool : ConnectionPool
	{
		// Token: 0x06000859 RID: 2137 RVA: 0x000208E4 File Offset: 0x0001ECE4
		public void Init(string initStr)
		{
			string[] array = initStr.Split(new char[]
			{
				';'
			});
			this.Init(array[0], array[1], array[2], array[3]);
		}

		// Token: 0x0600085A RID: 2138 RVA: 0x00020915 File Offset: 0x0001ED15
		public void Init(string server, string db, string user, string pass)
		{
			this.Init(server, db, user, pass, 0);
		}

		// Token: 0x0600085B RID: 2139 RVA: 0x00020924 File Offset: 0x0001ED24
		public void Init(string server, string db, string user, string pass, int command_timeout)
		{
			ConfigSection section = Resources.DBMasterSettings.GetSection("Pooling");
			MySqlConnectionStringBuilder mySqlConnectionStringBuilder = new MySqlConnectionStringBuilder();
			mySqlConnectionStringBuilder.Server = server;
			mySqlConnectionStringBuilder.Database = db;
			mySqlConnectionStringBuilder.UserID = user;
			mySqlConnectionStringBuilder.Password = pass;
			mySqlConnectionStringBuilder.Pooling = false;
			mySqlConnectionStringBuilder.AllowUserVariables = true;
			mySqlConnectionStringBuilder.AllowZeroDateTime = true;
			mySqlConnectionStringBuilder.CharacterSet = "utf8";
			ConnectionPool.Config @default = ConnectionPool.Config.Default;
			@default.MaxTimeInUse = new TimeSpan(0, 0, int.Parse(section.Get("MaxTimeInUse")));
			@default.MaxConnections = int.Parse(section.Get("MaxConnections"));
			@default.MinConnections = 0;
			@default.InactivityTimeout = new TimeSpan(0, 0, int.Parse(section.Get("InactivityTimeout")));
			@default.AllocationTimeout = new TimeSpan(0, 0, int.Parse(section.Get("AllocationTimeout")));
			@default.Pooling = false;
			if (section.HasValue("IsolationLevel"))
			{
				@default.TransactionLevel = (IsolationLevel)Enum.Parse(typeof(IsolationLevel), section.Get("IsolationLevel"));
			}
			if (!uint.TryParse(section.Get("ConnectionTimeout"), out @default.ConnectionTimeout))
			{
				@default.ConnectionTimeout = ConnectionPool.Config.Default.ConnectionTimeout;
			}
			if (command_timeout != 0)
			{
				@default.CommandTimeout = command_timeout;
			}
			else if (!int.TryParse(section.Get("CommandTimeout"), out @default.CommandTimeout))
			{
				@default.CommandTimeout = ConnectionPool.Config.Default.CommandTimeout;
			}
			mySqlConnectionStringBuilder.ConnectionTimeout = @default.ConnectionTimeout;
			@default.ConnectionString = mySqlConnectionStringBuilder.ToString();
			base.Init(@default);
		}
	}
}
