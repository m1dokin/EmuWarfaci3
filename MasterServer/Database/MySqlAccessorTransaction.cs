using System;
using System.Data;
using MasterServer.Core.Diagnostics.Profiler;
using MasterServer.DAL;
using MySql.Data.MySqlClient;

namespace MasterServer.Database
{
	// Token: 0x0200082D RID: 2093
	public class MySqlAccessorTransaction : MySqlAccessor
	{
		// Token: 0x06002B54 RID: 11092 RVA: 0x000BA771 File Offset: 0x000B8B71
		public MySqlAccessorTransaction()
		{
		}

		// Token: 0x06002B55 RID: 11093 RVA: 0x000BA779 File Offset: 0x000B8B79
		public MySqlAccessorTransaction(DALStats dal_stats) : base(dal_stats)
		{
		}

		// Token: 0x06002B56 RID: 11094 RVA: 0x000BA782 File Offset: 0x000B8B82
		public MySqlAccessorTransaction(DALStats dal_stats, int cmdTimeout) : base(dal_stats, cmdTimeout)
		{
		}

		// Token: 0x06002B57 RID: 11095 RVA: 0x000BA78C File Offset: 0x000B8B8C
		public MySqlAccessorTransaction(ConnectionPool pool) : base(pool)
		{
		}

		// Token: 0x06002B58 RID: 11096 RVA: 0x000BA795 File Offset: 0x000B8B95
		public MySqlAccessorTransaction(ConnectionPool pool, DALStats dal_stats) : base(pool, dal_stats)
		{
		}

		// Token: 0x06002B59 RID: 11097 RVA: 0x000BA79F File Offset: 0x000B8B9F
		public MySqlAccessorTransaction(ConnectionPool pool, DALStats dal_stats, int cmdTimeout) : base(pool, dal_stats, cmdTimeout)
		{
		}

		// Token: 0x06002B5A RID: 11098 RVA: 0x000BA7AC File Offset: 0x000B8BAC
		protected override void transaction_begin(DBAccessMode mode)
		{
			if (this.m_transaction != null)
			{
				throw new Exception("Transaction already started");
			}
			this.m_connection = base.get_connection(mode);
			this.m_tr_time = new TimeExecution();
			this.m_queries = 0;
			IsolationLevel iso = IsolationLevel.RepeatableRead;
			if (this.m_masterConnection != null)
			{
				iso = this.m_masterPool.ConnectionConfig.TransactionLevel;
			}
			else if (this.m_slaveConnection != null)
			{
				iso = this.m_slavePool.ConnectionConfig.TransactionLevel;
			}
			this.m_transaction = this.m_connection.BeginTransaction(iso);
		}

		// Token: 0x06002B5B RID: 11099 RVA: 0x000BA84C File Offset: 0x000B8C4C
		protected override void transaction_end(bool commit)
		{
			if (this.m_transaction == null)
			{
				throw new Exception("Transaction already finished");
			}
			if (commit)
			{
				this.m_transaction.Commit();
			}
			else
			{
				this.m_transaction.Rollback();
			}
			this.m_transaction.Dispose();
			this.m_transaction = null;
			this.m_connection = null;
			if (base.DALStats == null)
			{
				return;
			}
			base.DALStats.DBTime += this.m_tr_time.Stop();
			base.DALStats.DBQueries += this.m_queries;
		}

		// Token: 0x06002B5C RID: 11100 RVA: 0x000BA8F0 File Offset: 0x000B8CF0
		public override int ExecuteNonQuery(string query, DBAccessMode mode, params object[] args)
		{
			if (this.m_transaction == null)
			{
				throw new Exception("Transaction isn't started");
			}
			base.validate_parameters(args);
			int result;
			using (MySqlCommand mySqlCommand = new MySqlCommand(query, this.m_connection, this.m_transaction))
			{
				base.set_parameters(mySqlCommand, args);
				this.m_queries++;
				result = mySqlCommand.ExecuteNonQuery();
			}
			return result;
		}

		// Token: 0x06002B5D RID: 11101 RVA: 0x000BA970 File Offset: 0x000B8D70
		public override object ExecuteScalar(string query, DBAccessMode mode, params object[] args)
		{
			if (this.m_transaction == null)
			{
				throw new Exception("Transaction isn't started");
			}
			base.validate_parameters(args);
			object result;
			using (MySqlCommand mySqlCommand = new MySqlCommand(query, this.m_connection, this.m_transaction))
			{
				base.set_parameters(mySqlCommand, args);
				this.m_queries++;
				result = mySqlCommand.ExecuteScalar();
			}
			return result;
		}

		// Token: 0x06002B5E RID: 11102 RVA: 0x000BA9F0 File Offset: 0x000B8DF0
		public override DBDataReader ExecuteReader(string query, DBAccessMode mode, params object[] args)
		{
			if (this.m_transaction == null)
			{
				throw new Exception("Transaction isn't started");
			}
			base.validate_parameters(args);
			DBDataReader result;
			using (MySqlCommand mySqlCommand = new MySqlCommand(query, this.m_connection, this.m_transaction))
			{
				base.set_parameters(mySqlCommand, args);
				this.m_queries++;
				result = new DBDataReader(this, mySqlCommand.ExecuteReader(), null);
			}
			return result;
		}

		// Token: 0x04001709 RID: 5897
		private MySqlTransaction m_transaction;

		// Token: 0x0400170A RID: 5898
		private MySqlConnection m_connection;

		// Token: 0x0400170B RID: 5899
		private TimeExecution m_tr_time;

		// Token: 0x0400170C RID: 5900
		private int m_queries;
	}
}
