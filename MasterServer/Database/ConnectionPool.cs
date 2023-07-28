using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using MasterServer.Core;
using MasterServer.Core.Timers;
using MySql.Data.MySqlClient;

namespace MasterServer.Database
{
	// Token: 0x020001B6 RID: 438
	public class ConnectionPool : IDisposable
	{
		// Token: 0x170000DD RID: 221
		// (get) Token: 0x0600082D RID: 2093 RVA: 0x0001F5D8 File Offset: 0x0001D9D8
		public int TotalConnections
		{
			get
			{
				object thisLock = this.m_thisLock;
				int result;
				lock (thisLock)
				{
					result = this.m_pool.Count + this.m_inUse.Count;
				}
				return result;
			}
		}

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x0600082E RID: 2094 RVA: 0x0001F630 File Offset: 0x0001DA30
		public ConnectionPool.Config ConnectionConfig
		{
			get
			{
				object thisLock = this.m_thisLock;
				ConnectionPool.Config config;
				lock (thisLock)
				{
					config = this.m_config;
				}
				return config;
			}
		}

		// Token: 0x170000DF RID: 223
		// (set) Token: 0x0600082F RID: 2095 RVA: 0x0001F678 File Offset: 0x0001DA78
		protected int MaxTimeInUse
		{
			set
			{
				object thisLock = this.m_thisLock;
				lock (thisLock)
				{
					if (value > 0)
					{
						this.m_config.MaxTimeInUse = TimeSpan.FromSeconds((double)value);
					}
				}
			}
		}

		// Token: 0x170000E0 RID: 224
		// (set) Token: 0x06000830 RID: 2096 RVA: 0x0001F6D0 File Offset: 0x0001DAD0
		protected int InactivityTimeout
		{
			set
			{
				object thisLock = this.m_thisLock;
				lock (thisLock)
				{
					if (value > 0)
					{
						this.m_config.InactivityTimeout = TimeSpan.FromSeconds((double)value);
					}
				}
			}
		}

		// Token: 0x170000E1 RID: 225
		// (set) Token: 0x06000831 RID: 2097 RVA: 0x0001F728 File Offset: 0x0001DB28
		protected int MinConnections
		{
			set
			{
				object thisLock = this.m_thisLock;
				lock (thisLock)
				{
					if (value > 1 && value < this.m_config.MaxConnections)
					{
						this.m_config.MinConnections = value;
					}
				}
			}
		}

		// Token: 0x170000E2 RID: 226
		// (set) Token: 0x06000832 RID: 2098 RVA: 0x0001F78C File Offset: 0x0001DB8C
		protected int MaxConnections
		{
			set
			{
				object thisLock = this.m_thisLock;
				lock (thisLock)
				{
					if (value > 1 && value > this.m_config.MinConnections)
					{
						this.m_config.MaxConnections = value;
					}
				}
			}
		}

		// Token: 0x170000E3 RID: 227
		// (set) Token: 0x06000833 RID: 2099 RVA: 0x0001F7F0 File Offset: 0x0001DBF0
		protected int AllocationTimeout
		{
			set
			{
				object thisLock = this.m_thisLock;
				lock (thisLock)
				{
					if (value > 0)
					{
						this.m_config.AllocationTimeout = TimeSpan.FromSeconds((double)value);
					}
				}
			}
		}

		// Token: 0x170000E4 RID: 228
		// (set) Token: 0x06000834 RID: 2100 RVA: 0x0001F848 File Offset: 0x0001DC48
		protected int CommandTimeout
		{
			set
			{
				object thisLock = this.m_thisLock;
				lock (thisLock)
				{
					if (value > 0)
					{
						this.m_config.CommandTimeout = value;
					}
				}
			}
		}

		// Token: 0x170000E5 RID: 229
		// (set) Token: 0x06000835 RID: 2101 RVA: 0x0001F898 File Offset: 0x0001DC98
		protected uint ConnectionTimeout
		{
			set
			{
				object thisLock = this.m_thisLock;
				lock (thisLock)
				{
					if (value > 0U)
					{
						this.m_config.ConnectionTimeout = value;
					}
				}
			}
		}

		// Token: 0x170000E6 RID: 230
		// (set) Token: 0x06000836 RID: 2102 RVA: 0x0001F8E8 File Offset: 0x0001DCE8
		protected string IsolationLevel
		{
			set
			{
				object thisLock = this.m_thisLock;
				lock (thisLock)
				{
					IsolationLevel transactionLevel = (IsolationLevel)Enum.Parse(typeof(IsolationLevel), value, true);
					this.m_config.TransactionLevel = transactionLevel;
				}
			}
		}

		// Token: 0x06000837 RID: 2103 RVA: 0x0001F948 File Offset: 0x0001DD48
		public void Init(ConnectionPool.Config cfg)
		{
			this.m_lastId = 0;
			this.m_pool = new List<ConnectionPool.Entity>();
			this.m_inUse = new List<ConnectionPool.Entity>();
			this.m_config = default(ConnectionPool.Config);
			this.SetConfig(cfg);
			Log.Verbose("ConnectionPool initialized, connection string: {0}", new object[]
			{
				this.m_config.ConnectionString
			});
		}

		// Token: 0x06000838 RID: 2104 RVA: 0x0001F9A8 File Offset: 0x0001DDA8
		public void SetConfig(ConnectionPool.Config cfg)
		{
			object thisLock = this.m_thisLock;
			lock (thisLock)
			{
				if (string.IsNullOrEmpty(this.m_config.ConnectionString) && string.IsNullOrEmpty(cfg.ConnectionString))
				{
					throw new InvalidOperationException("Config must specify a connection string");
				}
				if (!string.IsNullOrEmpty(this.m_config.ConnectionString) && !string.IsNullOrEmpty(cfg.ConnectionString) && this.m_config.ConnectionString != cfg.ConnectionString)
				{
					if (this.m_inUse.Count != 0)
					{
						throw new InvalidOperationException("Cannot change connection string while connections in use");
					}
					while (this.m_pool.Count != 0)
					{
						this.DeleteEntity(this.m_pool[this.m_pool.Count - 1]);
					}
				}
				this.m_config.MinConnections = Math.Max(cfg.MinConnections, 1);
				this.m_config.MaxConnections = Math.Max(this.m_config.MinConnections, cfg.MaxConnections);
				this.m_config.MaxTimeInUse = cfg.MaxTimeInUse;
				this.m_config.InactivityTimeout = cfg.InactivityTimeout;
				this.m_config.AllocationTimeout = cfg.AllocationTimeout;
				this.m_config.ConnectionString = cfg.ConnectionString;
				this.m_config.ConnectionTimeout = cfg.ConnectionTimeout;
				this.m_config.CommandTimeout = cfg.CommandTimeout;
				this.m_config.Pooling = cfg.Pooling;
				this.m_config.TransactionLevel = cfg.TransactionLevel;
				if (this.m_config.Pooling && this.CreateEntity() == null)
				{
					throw new ApplicationException(string.Format("Failed to connect pool connection to DB: {0}", this.m_config.ConnectionString));
				}
				if (this.m_timeoutTimer != null)
				{
					this.m_timeoutTimer.Dispose();
				}
				this.m_conStrBuilder = new MySqlConnectionStringBuilder(this.m_config.ConnectionString);
				long value = this.m_config.InactivityTimeout.Ticks / 2L;
				this.m_timeoutTimer = new SafeTimer(new TimerCallback(this.FindInactive), this, TimeSpan.FromTicks(value));
			}
		}

		// Token: 0x06000839 RID: 2105 RVA: 0x0001FC10 File Offset: 0x0001E010
		public void Dispose()
		{
			Log.Verbose("Disposing ConnectionPool", new object[0]);
			this.OnDisposing();
			if (this.m_inUse.Count != 0)
			{
				Log.Warning("Some connections are still in use when disposing the connection pool");
			}
			while (this.m_pool.Count != 0)
			{
				this.DeleteEntity(this.m_pool[this.m_pool.Count - 1]);
			}
			if (this.m_timeoutTimer != null)
			{
				this.m_timeoutTimer.Dispose();
			}
		}

		// Token: 0x0600083A RID: 2106 RVA: 0x0001FC96 File Offset: 0x0001E096
		protected virtual void OnDisposing()
		{
		}

		// Token: 0x0600083B RID: 2107 RVA: 0x0001FC98 File Offset: 0x0001E098
		public ConnectionPool.Entity CreateConnection()
		{
			object thisLock = this.m_thisLock;
			lock (thisLock)
			{
				try
				{
					ConnectionPool.Entity entity;
					for (entity = this.getAvailableConnection(); entity == null; entity = this.getAvailableConnection())
					{
						entity = this.CreateEntity();
						if (entity == null && !Monitor.Wait(this.m_thisLock, this.m_config.AllocationTimeout))
						{
							break;
						}
					}
					if (entity != null)
					{
						this.AllocateEntity(entity);
						return entity;
					}
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
			throw new Exception(string.Format("Failed to allocate connection entity to DB: {0}", this.m_config.ConnectionString));
		}

		// Token: 0x0600083C RID: 2108 RVA: 0x0001FD6C File Offset: 0x0001E16C
		private ConnectionPool.Entity getAvailableConnection()
		{
			ConnectionPool.Entity entity = null;
			while (this.m_pool.Count > 0)
			{
				entity = this.m_pool[this.m_pool.Count - 1];
				if (entity.Ping())
				{
					break;
				}
				Log.Verbose("Connection {0} cannot ping the server. Removing", new object[]
				{
					entity.ID
				});
				this.DeleteEntity(entity);
				entity = null;
			}
			return entity;
		}

		// Token: 0x0600083D RID: 2109 RVA: 0x0001FDE8 File Offset: 0x0001E1E8
		internal void OnRelease(ConnectionPool.Entity ent)
		{
			object thisLock = this.m_thisLock;
			lock (thisLock)
			{
				this.m_pool.Add(ent);
				this.m_inUse.Remove(ent);
				if (!this.m_config.Pooling || this.m_pool.Count > this.m_config.MaxConnections)
				{
					this.DeleteEntity(ent);
				}
				Monitor.Pulse(this.m_thisLock);
			}
		}

		// Token: 0x0600083E RID: 2110 RVA: 0x0001FE7C File Offset: 0x0001E27C
		private void AllocateEntity(ConnectionPool.Entity ent)
		{
			object thisLock = this.m_thisLock;
			lock (thisLock)
			{
				ent.OnAllocate();
				this.m_pool.Remove(ent);
				this.m_inUse.Add(ent);
			}
		}

		// Token: 0x0600083F RID: 2111 RVA: 0x0001FED8 File Offset: 0x0001E2D8
		private void DeleteEntity(ConnectionPool.Entity ent)
		{
			object thisLock = this.m_thisLock;
			lock (thisLock)
			{
				if (this.m_pool.Count == 0)
				{
					Log.Error("There is no connections left. Nothing to delete");
				}
				else
				{
					try
					{
						ent.OnDelete();
						this.m_pool.Remove(ent);
						Log.Verbose("Connection {0} deleted", new object[]
						{
							ent.ID
						});
					}
					catch (Exception e)
					{
						Log.Error(e);
					}
				}
			}
		}

		// Token: 0x06000840 RID: 2112 RVA: 0x0001FF84 File Offset: 0x0001E384
		private ConnectionPool.Entity CreateEntity()
		{
			object thisLock = this.m_thisLock;
			ConnectionPool.Entity result;
			lock (thisLock)
			{
				if (this.m_config.Pooling && this.TotalConnections >= this.m_config.MaxConnections)
				{
					Log.Warning<int, string>("Too many connections {0}. Increase maximum value in configuration file for DB: {1}", this.TotalConnections, this.m_config.ConnectionString);
					result = null;
				}
				else
				{
					try
					{
						ConnectionPool.Entity entity = new ConnectionPool.Entity(this, this.m_lastId++, this.m_config.ConnectionString);
						this.m_pool.Add(entity);
						Log.Verbose("Connection {0} created", new object[]
						{
							entity.ID
						});
						return entity;
					}
					catch (Exception p)
					{
						Log.Error<string, Exception>("Failed to create connection entity to DB {0}: {1}", this.m_config.ConnectionString, p);
					}
					result = null;
				}
			}
			return result;
		}

		// Token: 0x06000841 RID: 2113 RVA: 0x0002008C File Offset: 0x0001E48C
		private void FindInactive(object obj)
		{
			object thisLock = this.m_thisLock;
			lock (thisLock)
			{
				DateTime now = DateTime.Now;
				this.m_pool.Sort();
				for (int i = this.m_pool.Count - 1; i >= 0; i--)
				{
					ConnectionPool.Entity entity = this.m_pool[i];
					TimeSpan t = now - entity.LastUseTime;
					if (!this.m_config.Pooling || t > this.m_config.InactivityTimeout)
					{
						for (int j = 0; j <= i; j++)
						{
							this.m_pool[j].OnDelete();
						}
						this.m_pool.RemoveRange(0, i + 1);
						Log.Verbose("Inactive connections {0} removed", new object[]
						{
							i + 1
						});
						break;
					}
				}
				for (int k = 0; k < this.m_inUse.Count; k++)
				{
					ConnectionPool.Entity entity2 = this.m_inUse[k];
					TimeSpan t2 = now - entity2.StartUseTime;
					if (t2 > this.m_config.MaxTimeInUse)
					{
						Log.Warning<int, string>("Connection {0} query {1} is too long in use. Probably it is resource leaks.", entity2.ID, entity2.Query);
						entity2.SignalLeakWarning();
					}
				}
				if (this.m_config.Pooling && this.m_inUse.Count > 0)
				{
					for (int l = 0; l < (this.m_config.MinConnections - this.TotalConnections) / 2; l++)
					{
						this.CreateEntity();
					}
				}
				Log.Info("Pool: {0} db: {1} in_use: {2} in pool: {3}", new object[]
				{
					this.GetHashCode(),
					this.m_conStrBuilder.Database,
					this.m_inUse.Count,
					this.m_pool.Count
				});
			}
		}

		// Token: 0x040004C6 RID: 1222
		private int m_lastId;

		// Token: 0x040004C7 RID: 1223
		private SafeTimer m_timeoutTimer;

		// Token: 0x040004C8 RID: 1224
		private ConnectionPool.Config m_config;

		// Token: 0x040004C9 RID: 1225
		private List<ConnectionPool.Entity> m_pool;

		// Token: 0x040004CA RID: 1226
		private List<ConnectionPool.Entity> m_inUse;

		// Token: 0x040004CB RID: 1227
		private MySqlConnectionStringBuilder m_conStrBuilder;

		// Token: 0x040004CC RID: 1228
		private readonly object m_thisLock = new object();

		// Token: 0x020001B7 RID: 439
		public class Entity : IDisposable, IComparable<ConnectionPool.Entity>
		{
			// Token: 0x06000842 RID: 2114 RVA: 0x000202B8 File Offset: 0x0001E6B8
			public Entity(ConnectionPool parent, int id, string connectionString)
			{
				this.m_lastUsed = DateTime.Now;
				this.m_startUse = this.m_lastUsed;
				this.m_parent = parent;
				this.m_id = id;
				this.m_inUse = false;
				this.m_conn = new MySqlConnection(connectionString);
				this.m_conn.Open();
				using (MySqlCommand mySqlCommand = new MySqlCommand("SET time_zone = '+00:00'", this.m_conn))
				{
					mySqlCommand.ExecuteNonQuery();
				}
			}

			// Token: 0x170000E7 RID: 231
			// (get) Token: 0x06000843 RID: 2115 RVA: 0x00020348 File Offset: 0x0001E748
			// (set) Token: 0x06000844 RID: 2116 RVA: 0x0002035C File Offset: 0x0001E75C
			public string Query
			{
				get
				{
					return this.m_query ?? string.Empty;
				}
				set
				{
					this.m_query = value;
				}
			}

			// Token: 0x170000E8 RID: 232
			// (get) Token: 0x06000845 RID: 2117 RVA: 0x00020365 File Offset: 0x0001E765
			public int ID
			{
				get
				{
					return this.m_id;
				}
			}

			// Token: 0x170000E9 RID: 233
			// (get) Token: 0x06000846 RID: 2118 RVA: 0x0002036D File Offset: 0x0001E76D
			public DateTime LastUseTime
			{
				get
				{
					return this.m_lastUsed;
				}
			}

			// Token: 0x170000EA RID: 234
			// (get) Token: 0x06000847 RID: 2119 RVA: 0x00020375 File Offset: 0x0001E775
			public DateTime StartUseTime
			{
				get
				{
					return this.m_startUse;
				}
			}

			// Token: 0x06000848 RID: 2120 RVA: 0x0002037D File Offset: 0x0001E77D
			public void Dispose()
			{
				this.LeakCheck();
				this.m_lastUsed = DateTime.Now;
				this.m_inUse = false;
				this.m_parent.OnRelease(this);
			}

			// Token: 0x06000849 RID: 2121 RVA: 0x000203A3 File Offset: 0x0001E7A3
			internal void OnAllocate()
			{
				this.m_inUse = true;
				this.m_startUse = DateTime.Now;
			}

			// Token: 0x0600084A RID: 2122 RVA: 0x000203B8 File Offset: 0x0001E7B8
			internal void OnDelete()
			{
				try
				{
					this.m_conn.Close();
					this.m_conn.Dispose();
					this.m_conn = null;
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}

			// Token: 0x0600084B RID: 2123 RVA: 0x00020404 File Offset: 0x0001E804
			public bool Ping()
			{
				return this.m_conn.Ping();
			}

			// Token: 0x170000EB RID: 235
			// (get) Token: 0x0600084C RID: 2124 RVA: 0x00020411 File Offset: 0x0001E811
			public MySqlConnection Connection
			{
				get
				{
					this.LeakCheck();
					if (!this.m_inUse)
					{
						throw new InvalidOperationException("Accessing connection of the entity returned to pool");
					}
					return this.m_conn;
				}
			}

			// Token: 0x0600084D RID: 2125 RVA: 0x00020435 File Offset: 0x0001E835
			public void SignalLeakWarning()
			{
				this.m_signalLeak = true;
			}

			// Token: 0x0600084E RID: 2126 RVA: 0x00020440 File Offset: 0x0001E840
			private void LeakCheck()
			{
				if (!this.m_signalLeak)
				{
					return;
				}
				this.m_signalLeak = false;
				StackTrace stackTrace = new StackTrace(true);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("Connection {0} leak (query {1}) detected at:\n", this.ID, this.Query);
				for (int num = 0; num != stackTrace.FrameCount; num++)
				{
					this.PrintFrame(num, stackTrace.GetFrame(num), stringBuilder);
				}
				Log.Warning(stringBuilder.ToString());
			}

			// Token: 0x0600084F RID: 2127 RVA: 0x000204C0 File Offset: 0x0001E8C0
			private void PrintFrame(int id, StackFrame frame, StringBuilder sb)
			{
				MethodBase method = frame.GetMethod();
				sb.Append(id);
				sb.Append("  ");
				if (method != null)
				{
					sb.AppendLine(string.Format("{0}::{1}", method.DeclaringType, method.Name));
				}
				if (frame.GetFileName() != null)
				{
					sb.AppendLine(string.Format(" at {0}:{1}", frame.GetFileName(), frame.GetFileLineNumber()));
				}
			}

			// Token: 0x06000850 RID: 2128 RVA: 0x00020540 File Offset: 0x0001E940
			public int CompareTo(ConnectionPool.Entity other)
			{
				return this.LastUseTime.CompareTo(other.LastUseTime);
			}

			// Token: 0x040004CD RID: 1229
			private DateTime m_lastUsed;

			// Token: 0x040004CE RID: 1230
			private DateTime m_startUse;

			// Token: 0x040004CF RID: 1231
			private MySqlConnection m_conn;

			// Token: 0x040004D0 RID: 1232
			private ConnectionPool m_parent;

			// Token: 0x040004D1 RID: 1233
			private string m_query;

			// Token: 0x040004D2 RID: 1234
			private int m_id;

			// Token: 0x040004D3 RID: 1235
			private bool m_inUse;

			// Token: 0x040004D4 RID: 1236
			private volatile bool m_signalLeak;
		}

		// Token: 0x020001B8 RID: 440
		public struct Config
		{
			// Token: 0x040004D5 RID: 1237
			public int MinConnections;

			// Token: 0x040004D6 RID: 1238
			public int MaxConnections;

			// Token: 0x040004D7 RID: 1239
			public TimeSpan InactivityTimeout;

			// Token: 0x040004D8 RID: 1240
			public TimeSpan MaxTimeInUse;

			// Token: 0x040004D9 RID: 1241
			public TimeSpan AllocationTimeout;

			// Token: 0x040004DA RID: 1242
			public uint ConnectionTimeout;

			// Token: 0x040004DB RID: 1243
			public int CommandTimeout;

			// Token: 0x040004DC RID: 1244
			public string ConnectionString;

			// Token: 0x040004DD RID: 1245
			public bool Pooling;

			// Token: 0x040004DE RID: 1246
			public IsolationLevel TransactionLevel;

			// Token: 0x040004DF RID: 1247
			public static ConnectionPool.Config Default = new ConnectionPool.Config
			{
				MinConnections = 0,
				MaxConnections = int.MaxValue,
				InactivityTimeout = new TimeSpan(0, 1, 0),
				MaxTimeInUse = TimeSpan.MaxValue,
				AllocationTimeout = new TimeSpan(0, 0, 10),
				ConnectionTimeout = 15U,
				CommandTimeout = 45,
				Pooling = false,
				TransactionLevel = System.Data.IsolationLevel.RepeatableRead
			};
		}
	}
}
