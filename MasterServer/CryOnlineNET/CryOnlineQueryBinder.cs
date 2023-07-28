using System;
using MasterServer.Core;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x02000188 RID: 392
	internal class CryOnlineQueryBinder : IOnlineQueryBinder
	{
		// Token: 0x06000721 RID: 1825 RVA: 0x0001B894 File Offset: 0x00019C94
		public CryOnlineQueryBinder(QueryHandler handler)
		{
			this.Handler = handler;
		}

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000722 RID: 1826 RVA: 0x0001B8A3 File Offset: 0x00019CA3
		// (set) Token: 0x06000723 RID: 1827 RVA: 0x0001B8AB File Offset: 0x00019CAB
		public QueryHandler Handler { get; private set; }

		// Token: 0x06000724 RID: 1828 RVA: 0x0001B8B4 File Offset: 0x00019CB4
		public override string Tag()
		{
			return this.Handler.Tag;
		}

		// Token: 0x06000725 RID: 1829 RVA: 0x0001B8C4 File Offset: 0x00019CC4
		public int Request(string receiver, string payload)
		{
			IOnlineConnection connection = this.GetConnection();
			if (connection != null && !string.IsNullOrEmpty(receiver))
			{
				return connection.Query(payload, receiver, this, this.Handler.CompressionType);
			}
			return -1;
		}

		// Token: 0x06000726 RID: 1830 RVA: 0x0001B900 File Offset: 0x00019D00
		public override void OnRequest(IOnlineConnection connection, SOnlineQuery query, string payload)
		{
			bool flag = true;
			try
			{
				Log.Info<string, string, string>("[Incoming request: {0} from {1}[{2}]]", this.Handler.Tag, query.online_id, query.sId);
				IQoSQueue service = ServicesManager.GetService<IQoSQueue>();
				QueryManager mgr = (QueryManager)ServicesManager.GetService<IQueryManager>();
				string from_jid = (!(query.tag == "setserver")) ? query.online_id : string.Format("{0}_presence", query.online_id);
				TShapingInfo shaping_info = new TShapingInfo
				{
					query_name = this.Handler.Tag,
					query_class = this.Handler.QoSClass,
					from_jid = from_jid
				};
				SOnlineQuery query_copy = query.Clone();
				flag = service.QueueAsyncWorkItem(shaping_info, (object x) => mgr.HandleRequestAsync(this, query_copy, payload));
			}
			catch (Exception e)
			{
				Log.Error(e);
				flag = false;
			}
			if (!flag)
			{
				Log.Warning<string, string, string>("[Request rejected by QoS: {0} jid: {1}[{2}]]", this.Handler.Tag, query.online_id, query.sId);
				SQueryError error = query.QueryError(-2);
				this.SendResponseError(error, payload);
			}
		}

		// Token: 0x06000727 RID: 1831 RVA: 0x0001BA58 File Offset: 0x00019E58
		public override void OnQueryCompleted(IOnlineConnection connection, SOnlineQuery query, string payload)
		{
			try
			{
				QueryManager mgr = (QueryManager)ServicesManager.GetService<IQueryManager>();
				SOnlineQuery query_copy = query.Clone();
				ThreadPoolProxy.QueueUserWorkItem(delegate(object x)
				{
					mgr.HandleResponse(this, query_copy, payload);
				});
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		// Token: 0x06000728 RID: 1832 RVA: 0x0001BAD8 File Offset: 0x00019ED8
		public override void OnQueryError(IOnlineConnection connection, SQueryError error)
		{
			try
			{
				QueryManager mgr = (QueryManager)ServicesManager.GetService<IQueryManager>();
				SQueryError error_copy = error.Clone();
				ThreadPoolProxy.QueueUserWorkItem(delegate(object x)
				{
					mgr.HandleError(this, error_copy);
				});
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		// Token: 0x06000729 RID: 1833 RVA: 0x0001BB44 File Offset: 0x00019F44
		private IOnlineConnection GetConnection()
		{
			IOnline online = CryOnline.CryOnlineGetInstance();
			IOnlineConnection onlineConnection = (online == null) ? null : online.GetConnection(Resources.XmppOnlineDomain);
			if (onlineConnection == null)
			{
				Log.Error("Connection is unavailable, dropping query");
			}
			return onlineConnection;
		}

		// Token: 0x0600072A RID: 1834 RVA: 0x0001BB80 File Offset: 0x00019F80
		public void SendResponse(SOnlineQuery query, string response)
		{
			IOnlineConnection connection = this.GetConnection();
			if (connection != null)
			{
				connection.Response(query, response, this.Handler.CompressionType);
			}
		}

		// Token: 0x0600072B RID: 1835 RVA: 0x0001BBB0 File Offset: 0x00019FB0
		public void SendResponseError(SQueryError error, string payload)
		{
			IOnlineConnection connection = this.GetConnection();
			if (connection != null)
			{
				connection.ResponseError(error, payload ?? string.Empty);
			}
		}

		// Token: 0x0600072C RID: 1836 RVA: 0x0001BBDE File Offset: 0x00019FDE
		public static EOnlineError ErrorCode(int code)
		{
			switch (code + 5)
			{
			case 0:
				return EOnlineError.eOnlineError_ParseError;
			case 1:
				return EOnlineError.eOnlineError_LastingServerFailure;
			case 2:
				return EOnlineError.eOnlineError_InvalidSession;
			case 3:
				return EOnlineError.eOnlineError_QoSLimitReached;
			default:
				return EOnlineError.eOnlineError_ParseError;
			}
		}

		// Token: 0x0600072D RID: 1837 RVA: 0x0001BC14 File Offset: 0x0001A014
		public static string ErrorMsg(int code)
		{
			int num = -code;
			string[] array = new string[]
			{
				"ok",
				"Query processing error",
				"QoS limit reached",
				"Query invalid session",
				"DB connection lost"
			};
			if (num >= 0 && num < array.Length)
			{
				return array[num];
			}
			return "Custom query error";
		}
	}
}
