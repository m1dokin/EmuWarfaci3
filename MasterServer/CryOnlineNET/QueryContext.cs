using System;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using MasterServer.Core.Diagnostics.Profiler;
using MasterServer.Database;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x0200018F RID: 399
	internal class QueryContext
	{
		// Token: 0x0600073A RID: 1850 RVA: 0x0001C10C File Offset: 0x0001A50C
		public QueryContext(string querySId, int queryId, string tag, QueryType type, string jid) : this(querySId, queryId, tag, type, jid, new QueryStats())
		{
		}

		// Token: 0x0600073B RID: 1851 RVA: 0x0001C120 File Offset: 0x0001A520
		public QueryContext(string querySId, int queryId, string tag, QueryType type, string jid, QueryStats stats)
		{
			this.QuerySId = querySId;
			this.QueryId = queryId;
			this.Tag = tag;
			this.Type = type;
			this.OnlineID = jid;
			this.StartThreadId = Thread.CurrentThread.ManagedThreadId;
			this.StartTime = new TimeExecution();
			this.Stats = stats;
		}

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x0600073C RID: 1852 RVA: 0x0001C17C File Offset: 0x0001A57C
		public IObjectCache L1Cache
		{
			get
			{
				L1Cache result;
				if (QueryContext.L1CacheEnabled)
				{
					if ((result = this.m_l1Cache) == null)
					{
						result = (this.m_l1Cache = new L1Cache());
					}
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		// Token: 0x0600073D RID: 1853 RVA: 0x0001C1B4 File Offset: 0x0001A5B4
		public void Succedded()
		{
			this.Stats.Succeeded = true;
		}

		// Token: 0x0600073E RID: 1854 RVA: 0x0001C1C2 File Offset: 0x0001A5C2
		public void SetError(EOnlineError error, int customCode)
		{
			this.Stats.OnlineError = error;
			this.Stats.CustomCode = customCode;
		}

		// Token: 0x0600073F RID: 1855 RVA: 0x0001C1DC File Offset: 0x0001A5DC
		public override string ToString()
		{
			return string.Format("QueryContext[{0}] {1} {2} {3}", new object[]
			{
				this.GetHashCode(),
				this.Type,
				this.Tag,
				this.OnlineID
			});
		}

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x06000740 RID: 1856 RVA: 0x0001C21C File Offset: 0x0001A61C
		public static QueryContext Current
		{
			get
			{
				return (QueryContext)CallContext.LogicalGetData(QueryContext.ContextKey);
			}
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x06000741 RID: 1857 RVA: 0x0001C22D File Offset: 0x0001A62D
		// (set) Token: 0x06000742 RID: 1858 RVA: 0x0001C234 File Offset: 0x0001A634
		public static bool L1CacheEnabled { get; set; }

		// Token: 0x0400045B RID: 1115
		public static readonly string ContextKey = "query_context";

		// Token: 0x0400045C RID: 1116
		public string QuerySId;

		// Token: 0x0400045D RID: 1117
		public int QueryId;

		// Token: 0x0400045E RID: 1118
		public readonly string Tag;

		// Token: 0x0400045F RID: 1119
		public readonly QueryType Type;

		// Token: 0x04000460 RID: 1120
		public readonly string OnlineID;

		// Token: 0x04000461 RID: 1121
		public readonly int StartThreadId;

		// Token: 0x04000462 RID: 1122
		public readonly TimeExecution StartTime;

		// Token: 0x04000463 RID: 1123
		public QueryStats Stats;

		// Token: 0x04000464 RID: 1124
		private L1Cache m_l1Cache;
	}
}
