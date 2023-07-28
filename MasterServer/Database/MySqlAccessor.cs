using System;
using MasterServer.Core;
using MasterServer.Core.Diagnostics.Profiler;
using MasterServer.DAL;
using MySql.Data.MySqlClient;

namespace MasterServer.Database
{
	// Token: 0x0200082A RID: 2090
	public class MySqlAccessor : IDisposable
	{
		// Token: 0x06002B2D RID: 11053 RVA: 0x000BA15C File Offset: 0x000B855C
		public MySqlAccessor() : this(Resources.MasterConnectionPool)
		{
		}

		// Token: 0x06002B2E RID: 11054 RVA: 0x000BA169 File Offset: 0x000B8569
		public MySqlAccessor(DALStats dal_stats) : this(Resources.MasterConnectionPool, dal_stats)
		{
		}

		// Token: 0x06002B2F RID: 11055 RVA: 0x000BA177 File Offset: 0x000B8577
		public MySqlAccessor(DALStats dal_stats, int cmdTimeout) : this(Resources.MasterConnectionPool, null, dal_stats, cmdTimeout)
		{
		}

		// Token: 0x06002B30 RID: 11056 RVA: 0x000BA187 File Offset: 0x000B8587
		public MySqlAccessor(ConnectionPool master_pool) : this(master_pool, null, null, 0)
		{
		}

		// Token: 0x06002B31 RID: 11057 RVA: 0x000BA193 File Offset: 0x000B8593
		public MySqlAccessor(ConnectionPool master_pool, DALStats dal_stats) : this(master_pool, null, dal_stats, 0)
		{
		}

		// Token: 0x06002B32 RID: 11058 RVA: 0x000BA19F File Offset: 0x000B859F
		public MySqlAccessor(ConnectionPool master_pool, DALStats dal_stats, int cmdTimeout) : this(master_pool, null, dal_stats, cmdTimeout)
		{
		}

		// Token: 0x06002B33 RID: 11059 RVA: 0x000BA1AB File Offset: 0x000B85AB
		public MySqlAccessor(ConnectionPool master_pool, ConnectionPool slave_pool) : this(master_pool, slave_pool, null, 0)
		{
		}

		// Token: 0x06002B34 RID: 11060 RVA: 0x000BA1B7 File Offset: 0x000B85B7
		public MySqlAccessor(ConnectionPool master_pool, ConnectionPool slave_pool, DALStats dal_stats, int cmdTimeout)
		{
			this.m_cmdTimeout = cmdTimeout;
			this.m_masterPool = master_pool;
			this.m_slavePool = slave_pool;
			this.DALStats = dal_stats;
		}

		// Token: 0x170003F4 RID: 1012
		// (get) Token: 0x06002B35 RID: 11061 RVA: 0x000BA1DC File Offset: 0x000B85DC
		// (set) Token: 0x06002B36 RID: 11062 RVA: 0x000BA1E4 File Offset: 0x000B85E4
		public DALStats DALStats
		{
			get
			{
				return this.m_dalStats;
			}
			set
			{
				this.m_dalStats = value;
			}
		}

		// Token: 0x06002B37 RID: 11063 RVA: 0x000BA1ED File Offset: 0x000B85ED
		public void Dispose()
		{
			if (this.m_masterConnection != null)
			{
				this.m_masterConnection.Dispose();
				this.m_masterConnection = null;
			}
			if (this.m_slaveConnection != null)
			{
				this.m_slaveConnection.Dispose();
				this.m_slaveConnection = null;
			}
		}

		// Token: 0x06002B38 RID: 11064 RVA: 0x000BA22C File Offset: 0x000B862C
		protected MySqlConnection get_connection(DBAccessMode mode)
		{
			DateTime now = DateTime.Now;
			MySqlConnection connection;
			if (mode == DBAccessMode.Slave && this.pick_slave_pool())
			{
				try
				{
					if (this.m_slaveConnection == null)
					{
						this.m_slaveConnection = this.m_slavePool.CreateConnection();
					}
					connection = this.m_slaveConnection.Connection;
				}
				catch
				{
					Log.Warning("Failed to connect to slave DB, using master");
					if (this.m_masterConnection == null)
					{
						this.m_masterConnection = this.m_masterPool.CreateConnection();
					}
					connection = this.m_masterConnection.Connection;
				}
			}
			else
			{
				if (this.m_masterConnection == null)
				{
					this.m_masterConnection = this.m_masterPool.CreateConnection();
				}
				connection = this.m_masterConnection.Connection;
			}
			if (this.DALStats != null)
			{
				this.DALStats.ConnectionAllocTime += DateTime.Now - now;
			}
			return connection;
		}

		// Token: 0x06002B39 RID: 11065 RVA: 0x000BA320 File Offset: 0x000B8720
		private bool pick_slave_pool()
		{
			if (this.m_slavePool != null)
			{
				return true;
			}
			if (!object.ReferenceEquals(this.m_masterPool, Resources.MasterConnectionPool) || Resources.SlaveConnectionPools.Length == 0)
			{
				return false;
			}
			int num = MySqlAccessor.m_slaveRand.Next(Resources.SlaveConnectionPools.Length);
			this.m_slavePool = Resources.SlaveConnectionPools[num];
			return true;
		}

		// Token: 0x06002B3A RID: 11066 RVA: 0x000BA380 File Offset: 0x000B8780
		protected void validate_parameters(object[] args)
		{
			if (args.Length % 2 != 0)
			{
				throw new Exception("Invalid query parameters count");
			}
			for (int i = 0; i < args.Length; i += 2)
			{
				if (!(args[i] is string))
				{
					throw new Exception("Invalid parametrized query call");
				}
			}
		}

		// Token: 0x06002B3B RID: 11067 RVA: 0x000BA3D0 File Offset: 0x000B87D0
		protected void set_parameters(MySqlCommand cmd, object[] args)
		{
			for (int i = 0; i < args.Length; i += 2)
			{
				cmd.Parameters.AddWithValue(args[i].ToString(), args[i + 1]);
			}
			if (this.m_masterConnection != null)
			{
				cmd.CommandTimeout = ((this.m_cmdTimeout != 0) ? this.m_cmdTimeout : this.m_masterPool.ConnectionConfig.CommandTimeout);
				this.m_masterConnection.Query = cmd.CommandText;
			}
			if (this.m_slaveConnection != null)
			{
				cmd.CommandTimeout = ((this.m_cmdTimeout != 0) ? this.m_cmdTimeout : this.m_slavePool.ConnectionConfig.CommandTimeout);
				this.m_slaveConnection.Query = cmd.CommandText;
			}
		}

		// Token: 0x06002B3C RID: 11068 RVA: 0x000BA4A0 File Offset: 0x000B88A0
		public bool TryConnect()
		{
			return this.TryConnect(DBAccessMode.Master);
		}

		// Token: 0x06002B3D RID: 11069 RVA: 0x000BA4AC File Offset: 0x000B88AC
		public bool TryConnect(DBAccessMode mode)
		{
			bool result;
			try
			{
				MySqlConnection mySqlConnection = this.get_connection(mode);
				result = true;
			}
			catch (Exception e)
			{
				Log.Error(e);
				result = false;
			}
			return result;
		}

		// Token: 0x06002B3E RID: 11070 RVA: 0x000BA4E8 File Offset: 0x000B88E8
		public DBDataReader ExecuteReader(string query, params object[] args)
		{
			return this.ExecuteReader(query, DBAccessMode.Master, args);
		}

		// Token: 0x06002B3F RID: 11071 RVA: 0x000BA4F4 File Offset: 0x000B88F4
		public virtual DBDataReader ExecuteReader(string query, DBAccessMode mode, params object[] args)
		{
			this.validate_parameters(args);
			DBDataReader result;
			using (MySqlCommand mySqlCommand = new MySqlCommand(query, this.get_connection(mode)))
			{
				this.set_parameters(mySqlCommand, args);
				TimeExecution sw = new TimeExecution();
				result = new DBDataReader(this, mySqlCommand.ExecuteReader(), sw);
			}
			return result;
		}

		// Token: 0x06002B40 RID: 11072 RVA: 0x000BA558 File Offset: 0x000B8958
		public int ExecuteNonQuery(string query, params object[] args)
		{
			return this.ExecuteNonQuery(query, DBAccessMode.Master, args);
		}

		// Token: 0x06002B41 RID: 11073 RVA: 0x000BA564 File Offset: 0x000B8964
		public virtual int ExecuteNonQuery(string query, DBAccessMode mode, params object[] args)
		{
			this.validate_parameters(args);
			int result;
			using (MySqlCommand mySqlCommand = new MySqlCommand(query, this.get_connection(mode)))
			{
				this.set_parameters(mySqlCommand, args);
				using (new TimeDB(this))
				{
					result = mySqlCommand.ExecuteNonQuery();
				}
			}
			return result;
		}

		// Token: 0x06002B42 RID: 11074 RVA: 0x000BA5D8 File Offset: 0x000B89D8
		public object ExecuteScalar(string query, params object[] args)
		{
			return this.ExecuteScalar(query, DBAccessMode.Master, args);
		}

		// Token: 0x06002B43 RID: 11075 RVA: 0x000BA5E4 File Offset: 0x000B89E4
		public virtual object ExecuteScalar(string query, DBAccessMode mode, params object[] args)
		{
			this.validate_parameters(args);
			object result;
			using (MySqlCommand mySqlCommand = new MySqlCommand(query, this.get_connection(mode)))
			{
				this.set_parameters(mySqlCommand, args);
				using (new TimeDB(this))
				{
					result = mySqlCommand.ExecuteScalar();
				}
			}
			return result;
		}

		// Token: 0x06002B44 RID: 11076 RVA: 0x000BA658 File Offset: 0x000B8A58
		public bool Transaction(MySqlAccessor.TransactionDelegate deleg)
		{
			return this.Transaction(deleg, DBAccessMode.Master);
		}

		// Token: 0x06002B45 RID: 11077 RVA: 0x000BA662 File Offset: 0x000B8A62
		public bool Transaction(MySqlAccessor.TransactionDelegateBool deleg)
		{
			return this.Transaction(deleg, DBAccessMode.Master);
		}

		// Token: 0x06002B46 RID: 11078 RVA: 0x000BA66C File Offset: 0x000B8A6C
		public bool Transaction(MySqlAccessor.TransactionDelegate deleg, DBAccessMode mode)
		{
			Exception ex;
			return this.Transaction(deleg, mode, out ex);
		}

		// Token: 0x06002B47 RID: 11079 RVA: 0x000BA684 File Offset: 0x000B8A84
		public bool Transaction(MySqlAccessor.TransactionDelegate deleg, DBAccessMode mode, out Exception error)
		{
			this.transaction_begin(mode);
			bool result;
			try
			{
				deleg();
				this.transaction_end(true);
				error = null;
				result = true;
			}
			catch (Exception ex)
			{
				Log.Warning<string, string>("[Transaction error] {0}\n{1}", ex.Message, ex.StackTrace);
				this.transaction_end(false);
				error = ex;
				result = false;
			}
			return result;
		}

		// Token: 0x06002B48 RID: 11080 RVA: 0x000BA6E8 File Offset: 0x000B8AE8
		public bool Transaction(MySqlAccessor.TransactionDelegateBool deleg, DBAccessMode mode)
		{
			this.transaction_begin(mode);
			bool result;
			try
			{
				bool flag = deleg();
				this.transaction_end(flag);
				result = flag;
			}
			catch (Exception ex)
			{
				Log.Warning<string, string>("[Transaction error] {0}\n{1}", ex.Message, ex.StackTrace);
				this.transaction_end(false);
				result = false;
			}
			return result;
		}

		// Token: 0x06002B49 RID: 11081 RVA: 0x000BA748 File Offset: 0x000B8B48
		protected virtual void transaction_begin(DBAccessMode mode)
		{
		}

		// Token: 0x06002B4A RID: 11082 RVA: 0x000BA74A File Offset: 0x000B8B4A
		protected virtual void transaction_end(bool commit)
		{
		}

		// Token: 0x04001702 RID: 5890
		protected ConnectionPool m_masterPool;

		// Token: 0x04001703 RID: 5891
		protected ConnectionPool m_slavePool;

		// Token: 0x04001704 RID: 5892
		protected int m_cmdTimeout;

		// Token: 0x04001705 RID: 5893
		protected ConnectionPool.Entity m_slaveConnection;

		// Token: 0x04001706 RID: 5894
		protected ConnectionPool.Entity m_masterConnection;

		// Token: 0x04001707 RID: 5895
		private static Random m_slaveRand = new Random((int)DateTime.Now.Ticks);

		// Token: 0x04001708 RID: 5896
		private DALStats m_dalStats;

		// Token: 0x0200082B RID: 2091
		// (Invoke) Token: 0x06002B4D RID: 11085
		public delegate void TransactionDelegate();

		// Token: 0x0200082C RID: 2092
		// (Invoke) Token: 0x06002B51 RID: 11089
		public delegate bool TransactionDelegateBool();
	}
}
