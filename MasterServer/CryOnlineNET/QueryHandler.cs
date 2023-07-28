using System;
using System.Threading.Tasks;
using System.Xml;
using MasterServer.Core;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x02000190 RID: 400
	internal abstract class QueryHandler : IDisposable
	{
		// Token: 0x14000022 RID: 34
		// (add) Token: 0x06000745 RID: 1861 RVA: 0x000041A4 File Offset: 0x000025A4
		// (remove) Token: 0x06000746 RID: 1862 RVA: 0x000041DC File Offset: 0x000025DC
		public event EventHandler<QueryEventArgs> QueryEvent;

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x06000747 RID: 1863 RVA: 0x00004212 File Offset: 0x00002612
		// (set) Token: 0x06000748 RID: 1864 RVA: 0x0000421A File Offset: 0x0000261A
		public CryOnlineQueryBinder Binder { get; set; }

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x06000749 RID: 1865 RVA: 0x00004223 File Offset: 0x00002623
		// (set) Token: 0x0600074A RID: 1866 RVA: 0x0000422B File Offset: 0x0000262B
		public string Tag { get; set; }

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x0600074B RID: 1867 RVA: 0x00004234 File Offset: 0x00002634
		// (set) Token: 0x0600074C RID: 1868 RVA: 0x0000423C File Offset: 0x0000263C
		public string QoSClass { get; set; }

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x0600074D RID: 1869 RVA: 0x00004245 File Offset: 0x00002645
		// (set) Token: 0x0600074E RID: 1870 RVA: 0x0000424D File Offset: 0x0000264D
		public ECompressType CompressionType { get; set; }

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x0600074F RID: 1871 RVA: 0x00004256 File Offset: 0x00002656
		// (set) Token: 0x06000750 RID: 1872 RVA: 0x0000425E File Offset: 0x0000265E
		public QueryManager Manager { get; set; }

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x06000751 RID: 1873 RVA: 0x00004267 File Offset: 0x00002667
		// (set) Token: 0x06000752 RID: 1874 RVA: 0x0000426F File Offset: 0x0000266F
		internal EOnlineError BlockedErrorCode { get; set; }

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x06000753 RID: 1875 RVA: 0x00004278 File Offset: 0x00002678
		// (set) Token: 0x06000754 RID: 1876 RVA: 0x00004282 File Offset: 0x00002682
		internal bool IsBlocked
		{
			get
			{
				return this.m_isBlocked;
			}
			set
			{
				this.m_isBlocked = value;
			}
		}

		// Token: 0x06000755 RID: 1877 RVA: 0x0000428D File Offset: 0x0000268D
		public virtual int HandleRequest(SOnlineQuery query, XmlElement request)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000756 RID: 1878 RVA: 0x00004294 File Offset: 0x00002694
		public virtual Task<int> HandleRequestAsync(SOnlineQuery query, XmlElement request)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000757 RID: 1879 RVA: 0x0000429B File Offset: 0x0000269B
		public virtual void SendRequest(string onlineId, XmlElement request, params object[] args)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000758 RID: 1880 RVA: 0x000042A2 File Offset: 0x000026A2
		public virtual object HandleResponse(SOnlineQuery query, XmlElement response)
		{
			return null;
		}

		// Token: 0x06000759 RID: 1881 RVA: 0x000042A5 File Offset: 0x000026A5
		public virtual void OnQueryError(SQueryError error)
		{
			Log.Error<string, EOnlineError, string>("Request {0} failed, code {1}: {2}", this.Tag, error.online_error, error.description);
		}

		// Token: 0x0600075A RID: 1882 RVA: 0x000042C3 File Offset: 0x000026C3
		public virtual void Dispose()
		{
		}

		// Token: 0x0600075B RID: 1883 RVA: 0x000042C5 File Offset: 0x000026C5
		protected virtual void OnQueryEvent(QueryEventArgs e)
		{
			if (this.QueryEvent != null)
			{
				e.QueryName = this.Tag;
				this.QueryEvent(this, e);
			}
		}

		// Token: 0x04000466 RID: 1126
		public const int QUERY_OK = 0;

		// Token: 0x04000467 RID: 1127
		public const int QUERY_FAIL = -1;

		// Token: 0x04000468 RID: 1128
		public const int QUERY_QOS_LIMIT = -2;

		// Token: 0x04000469 RID: 1129
		public const int QUERY_INVALID_SESSION = -3;

		// Token: 0x0400046A RID: 1130
		public const int QUERY_LASTING_SERVER_FAILURE = -4;

		// Token: 0x0400046B RID: 1131
		public const int QUERY_SERVICE_LOCKED = -5;

		// Token: 0x0400046C RID: 1132
		private volatile bool m_isBlocked;
	}
}
