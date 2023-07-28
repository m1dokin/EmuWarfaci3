using System;
using System.Collections.Generic;
using MasterServer.Core;
using MasterServer.DAL;

namespace MasterServer.Database
{
	// Token: 0x020001C3 RID: 451
	internal class DALProxyBase<TDAL> where TDAL : class, IBaseDALService
	{
		// Token: 0x170000EC RID: 236
		// (get) Token: 0x06000868 RID: 2152 RVA: 0x0000876E File Offset: 0x00006B6E
		protected BaseDALService DAL
		{
			get
			{
				if (this.m_dal == null)
				{
					this.m_dal = (ServicesManager.GetService<TDAL>() as BaseDALService);
				}
				return this.m_dal;
			}
		}

		// Token: 0x06000869 RID: 2153 RVA: 0x00008798 File Offset: 0x00006B98
		protected void Query(DALTracker tr, Func<DALResultVoid> func, string query_name, int query_retry)
		{
			using (DALTracker.CTimeDAL ctimeDAL = tr.TimeDAL())
			{
				DALResultVoid ret = null;
				Action action = delegate()
				{
					ret = func();
				};
				this.QueryImplWithRetry(action, ctimeDAL, query_name, query_retry);
				ctimeDAL.AddStats(ret.Stats);
			}
		}

		// Token: 0x0600086A RID: 2154 RVA: 0x00008818 File Offset: 0x00006C18
		protected T Query<T>(DALTracker tr, Func<DALResult<T>> func, string query_name, int query_retry)
		{
			T value;
			using (DALTracker.CTimeDAL ctimeDAL = tr.TimeDAL())
			{
				DALResult<T> ret = null;
				Action action = delegate()
				{
					ret = func();
				};
				this.QueryImplWithRetry(action, ctimeDAL, query_name, query_retry);
				ctimeDAL.AddStats(ret.Stats);
				value = ret.Value;
			}
			return value;
		}

		// Token: 0x0600086B RID: 2155 RVA: 0x000088A8 File Offset: 0x00006CA8
		protected IEnumerable<T> Query<T>(DALTracker tr, Func<DALResultMulti<T>> func, string query_name, int query_retry)
		{
			IEnumerable<T> values;
			using (DALTracker.CTimeDAL ctimeDAL = tr.TimeDAL())
			{
				DALResultMulti<T> ret = null;
				Action action = delegate()
				{
					ret = func();
				};
				this.QueryImplWithRetry(action, ctimeDAL, query_name, query_retry);
				ctimeDAL.AddStats(ret.Stats);
				values = ret.Values;
			}
			return values;
		}

		// Token: 0x0600086C RID: 2156 RVA: 0x00008938 File Offset: 0x00006D38
		private void QueryImplWithRetry(Action action, DALTracker.CTimeDAL dtr, string query_name, int query_retry)
		{
			DALStats dalstats = new DALStats();
			if (this.DAL.Config.QueryDeadlockEmulated)
			{
				this.DAL.Config.DebugDeadlockDecCounts(query_name);
			}
			try
			{
				IL_2C:
				if (this.DAL.Config.QueryDeadlockEmulated && this.DAL.Config.DebugDeadlockDecRetries(query_name))
				{
					throw new TransactionError(TransactionError.ErrorState.Deadlocked, new Exception("Emulated deadlock"));
				}
				action();
			}
			catch (TransactionError transactionError)
			{
				if (transactionError.State == TransactionError.ErrorState.Deadlocked)
				{
					dalstats.DBDeadlocksTotal++;
					if (query_retry-- > 0)
					{
						goto IL_2C;
					}
					dalstats.DBDeadlocks++;
					if (this.DAL.Config.QueryDeadlockEmulated)
					{
						this.DAL.Config.DebugDeadlockReset(query_name);
					}
				}
				dtr.AddStats(dalstats);
				throw transactionError;
			}
		}

		// Token: 0x040004F8 RID: 1272
		private BaseDALService m_dal;
	}
}
