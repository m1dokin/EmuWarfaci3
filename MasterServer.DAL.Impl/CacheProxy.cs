using System;
using System.Collections.Generic;
using MasterServer.Database;
using MySql.Data.MySqlClient;

namespace MasterServer.DAL.Impl
{
	// Token: 0x02000005 RID: 5
	internal class CacheProxy
	{
		// Token: 0x0600001B RID: 27 RVA: 0x00002AB4 File Offset: 0x00000CB4
		public DALResultMulti<T> GetStream<T>(CacheProxy.Options<T> options)
		{
			if ((string.IsNullOrEmpty(options.db_query) && options.get_data_stream == null) || (!string.IsNullOrEmpty(options.db_query) && options.get_data_stream != null) || options.get_data != null)
			{
				throw new Exception("Invalid query options");
			}
			if (options.db_mode == DBAccessMode.Unspecified)
			{
				options.db_mode = DBAccessMode.Master;
			}
			if (!string.IsNullOrEmpty(options.db_query))
			{
				return this.GetStreamDB<T>(options);
			}
			return this.GetStreamDeleg<T>(options);
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002B40 File Offset: 0x00000D40
		private DALResultMulti<T> GetStreamDB<T>(CacheProxy.Options<T> options)
		{
			options.get_data_stream = delegate()
			{
				CacheProxy.<GetStreamDB>c__AnonStorey0<T>.<GetStreamDB>c__AnonStorey2 <GetStreamDB>c__AnonStorey2 = new CacheProxy.<GetStreamDB>c__AnonStorey0<T>.<GetStreamDB>c__AnonStorey2();
				<GetStreamDB>c__AnonStorey2.ret = new List<T>();
				using (MySqlAccessor acc = this.create_accessor(options))
				{
					Exception ex;
					if (!acc.Transaction(delegate()
					{
						using (DBDataReader dbdataReader = acc.ExecuteReader(options.db_query, options.db_mode, options.db_query_args ?? <GetStreamDB>c__AnonStorey.empty_args))
						{
							while (dbdataReader.Read())
							{
								T item;
								options.db_serializer.Deserialize(dbdataReader, out item);
								<GetStreamDB>c__AnonStorey2.ret.Add(item);
							}
						}
					}, options.db_mode, out ex))
					{
						throw new TransactionError(this.GetState(ex), ex);
					}
				}
				return <GetStreamDB>c__AnonStorey2.ret;
			};
			return this.GetStreamDeleg<T>(options);
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002B84 File Offset: 0x00000D84
		private DALResultMulti<T> GetStreamDeleg<T>(CacheProxy.Options<T> options)
		{
			List<T> val = options.get_data_stream();
			return new DALResultMulti<T>(val, options.stats);
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002BAC File Offset: 0x00000DAC
		public DALResult<T> Get<T>(CacheProxy.Options<T> options)
		{
			if ((string.IsNullOrEmpty(options.db_query) && options.get_data == null) || (!string.IsNullOrEmpty(options.db_query) && options.get_data != null) || options.get_data_stream != null)
			{
				throw new Exception("Invalid query options");
			}
			if (options.db_mode == DBAccessMode.Unspecified)
			{
				options.db_mode = DBAccessMode.Master;
			}
			if (!string.IsNullOrEmpty(options.db_query))
			{
				return this.GetDB<T>(options);
			}
			return this.GetDeleg<T>(options);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002C38 File Offset: 0x00000E38
		private DALResult<T> GetDB<T>(CacheProxy.Options<T> options)
		{
			options.get_data = delegate()
			{
				CacheProxy.<GetDB>c__AnonStorey3<T>.<GetDB>c__AnonStorey5 <GetDB>c__AnonStorey2 = new CacheProxy.<GetDB>c__AnonStorey3<T>.<GetDB>c__AnonStorey5();
				<GetDB>c__AnonStorey2.ret = default(T);
				using (MySqlAccessor acc = this.create_accessor(options))
				{
					Exception ex;
					if (!acc.Transaction(delegate()
					{
						using (DBDataReader dbdataReader = acc.ExecuteReader(options.db_query, options.db_mode, options.db_query_args ?? <GetDB>c__AnonStorey.empty_args))
						{
							if (dbdataReader.Read())
							{
								options.db_serializer.Deserialize(dbdataReader, out <GetDB>c__AnonStorey2.ret);
							}
						}
					}, options.db_mode, out ex))
					{
						throw new TransactionError(this.GetState(ex), ex);
					}
				}
				return <GetDB>c__AnonStorey2.ret;
			};
			return this.GetDeleg<T>(options);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002C7C File Offset: 0x00000E7C
		private DALResult<T> GetDeleg<T>(CacheProxy.Options<T> options)
		{
			T val = options.get_data();
			return new DALResult<T>(val, options.stats);
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002CA4 File Offset: 0x00000EA4
		public DALResultVoid Set(CacheProxy.SetOptions options)
		{
			CacheProxy.<Set>c__AnonStorey7 <Set>c__AnonStorey = new CacheProxy.<Set>c__AnonStorey7();
			<Set>c__AnonStorey.options = options;
			using (MySqlAccessor acc = this.create_accessor(<Set>c__AnonStorey.options))
			{
				Exception ex;
				if (!acc.Transaction(delegate()
				{
					acc.ExecuteNonQuery(<Set>c__AnonStorey.options.db_query, <Set>c__AnonStorey.options.db_query_args);
				}, DBAccessMode.Master, out ex))
				{
					throw new TransactionError(this.GetState(ex), ex);
				}
			}
			return new DALResultVoid(<Set>c__AnonStorey.options.stats);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002D48 File Offset: 0x00000F48
		public object SetScalar(CacheProxy.SetOptions options)
		{
			CacheProxy.<SetScalar>c__AnonStorey9 <SetScalar>c__AnonStorey = new CacheProxy.<SetScalar>c__AnonStorey9();
			<SetScalar>c__AnonStorey.options = options;
			<SetScalar>c__AnonStorey.ret = null;
			using (MySqlAccessor acc = this.create_accessor(<SetScalar>c__AnonStorey.options))
			{
				Exception ex;
				if (!acc.Transaction(delegate()
				{
					<SetScalar>c__AnonStorey.ret = acc.ExecuteScalar(<SetScalar>c__AnonStorey.options.db_query, <SetScalar>c__AnonStorey.options.db_query_args);
				}, DBAccessMode.Master, out ex))
				{
					throw new TransactionError(this.GetState(ex), ex);
				}
			}
			return <SetScalar>c__AnonStorey.ret;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002DE8 File Offset: 0x00000FE8
		private MySqlAccessor create_accessor(CacheProxy.BaseOptions options)
		{
			if (options.stats == null)
			{
				options.stats = new DALStats();
			}
			if (options.connection_pool == null)
			{
				return (!options.db_transaction) ? new MySqlAccessor(options.stats, options.db_cmd_timeout) : new MySqlAccessorTransaction(options.stats, options.db_cmd_timeout);
			}
			return (!options.db_transaction) ? new MySqlAccessor(options.connection_pool, options.stats, options.db_cmd_timeout) : new MySqlAccessorTransaction(options.connection_pool, options.stats, options.db_cmd_timeout);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002E88 File Offset: 0x00001088
		private TransactionError.ErrorState GetState(Exception ex)
		{
			if (ex == null)
			{
				return TransactionError.ErrorState.Aborted;
			}
			MySqlException ex2 = ex as MySqlException;
			if (ex2 != null && (ex2.Number == 1205 || ex2.Number == 1213))
			{
				return TransactionError.ErrorState.Deadlocked;
			}
			return TransactionError.ErrorState.Other;
		}

		// Token: 0x0400000A RID: 10
		private object[] empty_args = new object[0];

		// Token: 0x02000006 RID: 6
		// (Invoke) Token: 0x06000026 RID: 38
		public delegate T DataFetchDeleg<T>();

		// Token: 0x02000007 RID: 7
		// (Invoke) Token: 0x0600002A RID: 42
		public delegate List<T> DataFetchMultiDeleg<T>();

		// Token: 0x02000008 RID: 8
		public class BaseOptions
		{
			// Token: 0x0600002E RID: 46 RVA: 0x00002EE0 File Offset: 0x000010E0
			public void query(string query, params object[] args)
			{
				this.db_query = query;
				this.db_query_args = args;
			}

			// Token: 0x0400000B RID: 11
			public string db_query;

			// Token: 0x0400000C RID: 12
			public object[] db_query_args;

			// Token: 0x0400000D RID: 13
			public bool db_transaction;

			// Token: 0x0400000E RID: 14
			public int db_cmd_timeout;

			// Token: 0x0400000F RID: 15
			public DALStats stats = new DALStats();

			// Token: 0x04000010 RID: 16
			public ConnectionPool connection_pool;
		}

		// Token: 0x02000009 RID: 9
		public class Options<T> : CacheProxy.BaseOptions
		{
			// Token: 0x04000011 RID: 17
			public IDBSerializer<T> db_serializer;

			// Token: 0x04000012 RID: 18
			public DBAccessMode db_mode;

			// Token: 0x04000013 RID: 19
			public CacheProxy.DataFetchDeleg<T> get_data;

			// Token: 0x04000014 RID: 20
			public CacheProxy.DataFetchMultiDeleg<T> get_data_stream;
		}

		// Token: 0x0200000A RID: 10
		public class SetOptions : CacheProxy.BaseOptions
		{
		}
	}
}
