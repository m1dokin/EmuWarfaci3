using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using HK2Net;
using HK2Net.Kernel;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Diagnostics.Profiler;
using MasterServer.Core.Diagnostics.Threading;
using MasterServer.Core.Services;
using MasterServer.Database;
using Util.Common;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x02000195 RID: 405
	[Service]
	[Singleton]
	internal class QueryManager : ServiceModule, IQueryManager
	{
		// Token: 0x06000778 RID: 1912 RVA: 0x0001C358 File Offset: 0x0001A758
		public QueryManager(IContainer container)
		{
			this.m_container = container;
			ConfigSection section = Resources.DBMasterSettings.GetSection("L1Cache");
			QueryContext.L1CacheEnabled = true;
			if (section != null)
			{
				QueryContext.L1CacheEnabled = (string.Compare("true", section.Get("enabled"), true) == 0);
				section.OnConfigChanged += this.OnConfigChanged;
			}
			this.QueryExtraDelay = 0;
			this.RequestDelays = new QueryDelay();
			this.ResponseDelays = new QueryDelay();
			this.m_query_binders = new Dictionary<string, CryOnlineQueryBinder>();
			int cacheTimeoutSec;
			Resources.XMPPSettings.Get("OutgoingRequestTTL", out cacheTimeoutSec);
			this.m_pending_requests = new CacheDictionary<int, TaskCompletionSource<object>>(cacheTimeoutSec, CacheDictionaryMode.Expiration);
			this.m_pending_requests.ItemExpired += this.OnRequestExpired;
		}

		// Token: 0x14000024 RID: 36
		// (add) Token: 0x06000779 RID: 1913 RVA: 0x0001C434 File Offset: 0x0001A834
		// (remove) Token: 0x0600077A RID: 1914 RVA: 0x0001C46C File Offset: 0x0001A86C
		public event Action<QueryContext> QueryCompleted;

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x0600077B RID: 1915 RVA: 0x0001C4A2 File Offset: 0x0001A8A2
		// (set) Token: 0x0600077C RID: 1916 RVA: 0x0001C4AA File Offset: 0x0001A8AA
		public int QueryExtraDelay { get; set; }

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x0600077D RID: 1917 RVA: 0x0001C4B3 File Offset: 0x0001A8B3
		// (set) Token: 0x0600077E RID: 1918 RVA: 0x0001C4BB File Offset: 0x0001A8BB
		public QueryDelay RequestDelays { get; private set; }

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x0600077F RID: 1919 RVA: 0x0001C4C4 File Offset: 0x0001A8C4
		// (set) Token: 0x06000780 RID: 1920 RVA: 0x0001C4CC File Offset: 0x0001A8CC
		public QueryDelay ResponseDelays { get; private set; }

		// Token: 0x06000781 RID: 1921 RVA: 0x0001C4D5 File Offset: 0x0001A8D5
		public override void Init()
		{
			base.Init();
			this.CreateQueryHandlers();
			this.CreateQuerySizeApprx();
		}

		// Token: 0x06000782 RID: 1922 RVA: 0x0001C4EC File Offset: 0x0001A8EC
		private void CreateQueryHandlers()
		{
			IEnumerable<QueryHandler> enumerable = this.m_container.CreateSet<QueryHandler>();
			foreach (QueryHandler queryHandler in enumerable)
			{
				QueryAttributes attribute = ReflectionUtils.GetAttribute<QueryAttributes>(queryHandler.GetType());
				queryHandler.Tag = attribute.TagName;
				queryHandler.QoSClass = attribute.QoSClass;
				queryHandler.CompressionType = attribute.CompressionType;
				queryHandler.Manager = this;
				CryOnlineQueryBinder cryOnlineQueryBinder = new CryOnlineQueryBinder(queryHandler);
				queryHandler.Binder = cryOnlineQueryBinder;
				this.m_query_binders.Add(queryHandler.Tag, cryOnlineQueryBinder);
			}
		}

		// Token: 0x06000783 RID: 1923 RVA: 0x0001C5A0 File Offset: 0x0001A9A0
		private void CreateQuerySizeApprx()
		{
			ConfigSection section = Resources.XMPPSettings.GetSection("SizeApproximation");
			section.OnConfigChanged += this.OnConfigChanged;
			float approxRate;
			section.Get("rate", out approxRate);
			float timeWindow;
			section.Get("tw", out timeWindow);
			section.Get("enabled", out this.m_query_size_apprx_enabled);
			Log.Info<string>("Query size approximation is {0}", (!this.m_query_size_apprx_enabled) ? "disabled" : "enabled");
			foreach (KeyValuePair<QueryAttributes, Type> keyValuePair in ReflectionUtils.GetTypesByAttribute<QueryAttributes>(Assembly.GetExecutingAssembly()))
			{
				QueryAttributes key = keyValuePair.Key;
				this.m_query_size_apprx[key.TagName] = new QueryManager.QuerySizeApprx(new EWMA(approxRate, timeWindow));
			}
		}

		// Token: 0x06000784 RID: 1924 RVA: 0x0001C694 File Offset: 0x0001AA94
		public override void Start()
		{
			IDALService service = ServicesManager.GetService<IDALService>();
			service.OnDALStats += this.OnDALStats;
			ITelemetryDALService service2 = ServicesManager.GetService<ITelemetryDALService>();
			service2.OnDALStats += this.OnDALStats;
		}

		// Token: 0x06000785 RID: 1925 RVA: 0x0001C6D4 File Offset: 0x0001AAD4
		public override void Stop()
		{
			base.Stop();
			foreach (CryOnlineQueryBinder cryOnlineQueryBinder in this.m_query_binders.Values)
			{
				cryOnlineQueryBinder.Handler.Dispose();
			}
			this.m_pending_requests.Dispose();
		}

		// Token: 0x06000786 RID: 1926 RVA: 0x0001C74C File Offset: 0x0001AB4C
		internal void RegisterBinders()
		{
			IOnline online = CryOnline.CryOnlineGetInstance();
			foreach (CryOnlineQueryBinder cryOnlineQueryBinder in this.m_query_binders.Values)
			{
				online.RegisterQueryBinder(cryOnlineQueryBinder, cryOnlineQueryBinder.Tag());
			}
		}

		// Token: 0x06000787 RID: 1927 RVA: 0x0001C7BC File Offset: 0x0001ABBC
		internal void UnregisterBinders()
		{
			IOnline online = CryOnline.CryOnlineGetInstance();
			if (online == null)
			{
				return;
			}
			foreach (CryOnlineQueryBinder binder in this.m_query_binders.Values)
			{
				online.UnregisterQueryBinder(binder);
			}
		}

		// Token: 0x06000788 RID: 1928 RVA: 0x0001C82C File Offset: 0x0001AC2C
		private void UpdateQueryBlockingFlag(string tag, bool flag, EOnlineError errorCode)
		{
			CryOnlineQueryBinder cryOnlineQueryBinder;
			if (!this.m_query_binders.TryGetValue(tag, out cryOnlineQueryBinder))
			{
				return;
			}
			cryOnlineQueryBinder.Handler.IsBlocked = flag;
			cryOnlineQueryBinder.Handler.BlockedErrorCode = ((!flag) ? EOnlineError.eOnlineError_NoError : errorCode);
		}

		// Token: 0x06000789 RID: 1929 RVA: 0x0001C871 File Offset: 0x0001AC71
		public IEnumerable<string> GetQueries()
		{
			return this.m_query_binders.Keys.ToList<string>();
		}

		// Token: 0x0600078A RID: 1930 RVA: 0x0001C884 File Offset: 0x0001AC84
		public IEnumerable<string> GetQueries(string subString)
		{
			return (from q in this.m_query_binders.Keys
			where q.Contains(subString)
			select q).ToList<string>();
		}

		// Token: 0x0600078B RID: 1931 RVA: 0x0001C8C0 File Offset: 0x0001ACC0
		public void UpdateQueryBlockingFlags(IEnumerable<string> tags, bool isBlocked, EOnlineError errorCode)
		{
			foreach (string tag in tags)
			{
				this.UpdateQueryBlockingFlag(tag, isBlocked, errorCode);
			}
		}

		// Token: 0x0600078C RID: 1932 RVA: 0x0001C918 File Offset: 0x0001AD18
		public void DumpQueryBlockingFlags()
		{
			List<QueryHandler> list = (from b in this.m_query_binders.Values
			select b.Handler into h
			where h.IsBlocked
			select h).ToList<QueryHandler>();
			if (list.Count == 0)
			{
				Log.Info("There are no blocked queries");
			}
			else
			{
				Log.Info("Listing all blocked queries:");
				foreach (QueryHandler queryHandler in list)
				{
					Log.Info<string, EOnlineError>("{0} (code: {1:D})", queryHandler.Tag, queryHandler.BlockedErrorCode);
				}
			}
		}

		// Token: 0x0600078D RID: 1933 RVA: 0x0001C9F8 File Offset: 0x0001ADF8
		public void DumpQueryBlockingFlags(IEnumerable<string> tags)
		{
			foreach (string text in tags)
			{
				CryOnlineQueryBinder cryOnlineQueryBinder;
				if (!this.m_query_binders.TryGetValue(text, out cryOnlineQueryBinder))
				{
					Log.Info<string>("Handler for {0} query wasn't found!", text);
				}
				else
				{
					Log.Info<string, string, EOnlineError>("Query {0} is {1} with error code {2:D}", text, (!cryOnlineQueryBinder.Handler.IsBlocked) ? "NOT blocked" : "blocked", cryOnlineQueryBinder.Handler.BlockedErrorCode);
				}
			}
		}

		// Token: 0x0600078E RID: 1934 RVA: 0x0001CAA0 File Offset: 0x0001AEA0
		private void addQueryEventHandler(string tagName, EventHandler<QueryEventArgs> handler)
		{
			CryOnlineQueryBinder cryOnlineQueryBinder = this.m_query_binders[tagName];
			cryOnlineQueryBinder.Handler.QueryEvent += handler;
		}

		// Token: 0x0600078F RID: 1935 RVA: 0x0001CAC8 File Offset: 0x0001AEC8
		private void removeQueryEventHandler(string tagName, EventHandler<QueryEventArgs> handler)
		{
			CryOnlineQueryBinder cryOnlineQueryBinder = this.m_query_binders[tagName];
			cryOnlineQueryBinder.Handler.QueryEvent -= handler;
		}

		// Token: 0x06000790 RID: 1936 RVA: 0x0001CAF0 File Offset: 0x0001AEF0
		public void Response(QueryHandler handler, SOnlineQuery query, XmlElement response)
		{
			this.ResponseDelays.Delay(query.tag);
			string outerXml = this.GetOuterXml(handler.Tag, response);
			handler.Binder.SendResponse(query, outerXml);
		}

		// Token: 0x06000791 RID: 1937 RVA: 0x0001CB2C File Offset: 0x0001AF2C
		public void ResponseError(QueryHandler handler, SOnlineQuery query, int code, XmlElement request)
		{
			SQueryError error = query.QueryError(code);
			handler.Binder.SendResponseError(error, (request == null) ? string.Empty : this.GetOuterXml(handler.Tag, request));
		}

		// Token: 0x06000792 RID: 1938 RVA: 0x0001CB6C File Offset: 0x0001AF6C
		private void QueryRequestDelay(string tag)
		{
			if (this.QueryExtraDelay > 0)
			{
				Thread.Sleep(this.QueryExtraDelay);
			}
			this.RequestDelays.Delay(tag);
		}

		// Token: 0x06000793 RID: 1939 RVA: 0x0001CB91 File Offset: 0x0001AF91
		public void Request(string tag, string receiver, params object[] args)
		{
			this.RequestAsync(tag, receiver, args).ContinueWith(delegate(Task<object> t)
			{
				this.LogRequestException(t.Exception);
			}, TaskContinuationOptions.OnlyOnFaulted);
		}

		// Token: 0x06000794 RID: 1940 RVA: 0x0001CBB4 File Offset: 0x0001AFB4
		private void LogRequestException(AggregateException aggregateException)
		{
			QueryException ex = aggregateException.InnerException as QueryException;
			if (ex != null && ex.OnlineError == EOnlineError.eOnlineError_QueryTimeout)
			{
				return;
			}
			ex = ((!aggregateException.InnerExceptions.Any<Exception>()) ? null : (aggregateException.InnerExceptions[0] as QueryException));
			if (ex != null && ex.OnlineError == EOnlineError.eOnlineError_QueryTimeout)
			{
				return;
			}
			Log.Error(aggregateException);
		}

		// Token: 0x06000795 RID: 1941 RVA: 0x0001CC2C File Offset: 0x0001B02C
		public Task<object> RequestAsync(string tag, string receiver, params object[] args)
		{
			Task<object> task;
			using (QueryManager.QueryCtxGuard queryCtxGuard = new QueryManager.QueryCtxGuard(this, tag, QueryType.Outgoing_Request, receiver))
			{
				CryOnlineQueryBinder cryOnlineQueryBinder = this.m_query_binders[tag];
				QueryHandler handler = cryOnlineQueryBinder.Handler;
				Exception exception;
				try
				{
					XmlDocument xmlDocument = new XmlDocument();
					XmlElement xmlElement = xmlDocument.CreateElement(tag);
					xmlDocument.AppendChild(xmlElement);
					handler.SendRequest(receiver, xmlElement, args);
					object request_lock = this.m_request_lock;
					lock (request_lock)
					{
						if (handler.IsBlocked)
						{
							exception = new QueryException(handler.BlockedErrorCode, tag + " query is blocked");
						}
						else
						{
							string outerXml = this.GetOuterXml(tag, xmlElement);
							int num = cryOnlineQueryBinder.Request(receiver, outerXml);
							if (num != -1)
							{
								TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>(num);
								this.m_pending_requests.AddNew(num, taskCompletionSource);
								queryCtxGuard.SetQueryID(num);
								queryCtxGuard.Succeeded();
								return taskCompletionSource.Task;
							}
							exception = new QueryException(EOnlineError.eOnlineError_LostConnection, "Failed to initiate request");
							queryCtxGuard.SetError(EOnlineError.eOnlineError_LostConnection, 1010);
						}
					}
				}
				catch (Exception inner)
				{
					exception = new QueryException(EOnlineError.eOnlineError_ParseError, "Failed to form request", inner);
				}
				TaskCompletionSource<object> taskCompletionSource2 = new TaskCompletionSource<object>();
				taskCompletionSource2.SetException(exception);
				task = taskCompletionSource2.Task;
			}
			return task;
		}

		// Token: 0x06000796 RID: 1942 RVA: 0x0001CDD8 File Offset: 0x0001B1D8
		private void OnRequestExpired(int key, TaskCompletionSource<object> tcs)
		{
			tcs.SetException(new QueryException(EOnlineError.eOnlineError_QueryTimeout, string.Format("Query failed by timeout, id {0}", key)));
		}

		// Token: 0x06000797 RID: 1943 RVA: 0x0001CDFC File Offset: 0x0001B1FC
		internal Task HandleRequestAsync(CryOnlineQueryBinder binder, SOnlineQuery query, string payload)
		{
			QueryManager.QueryCtxGuard ctx = new QueryManager.QueryCtxGuard(this, query.sId, query.id, query.tag, QueryType.Incoming_Request, query.online_id);
			Task result;
			try
			{
				this.QueryRequestDelay(query.tag);
				QueryHandler handler = binder.Handler;
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(payload);
				XmlElement documentElement = xmlDocument.DocumentElement;
				Task<int> task = handler.HandleRequestAsync(query, documentElement);
				result = task.ContinueWith(delegate(Task<int> t)
				{
					try
					{
						int result2 = t.Result;
						if (result2 == 0)
						{
							ctx.Succeeded();
						}
						else
						{
							SQueryError squeryError2 = (result2 != -5) ? query.QueryError(result2) : query.QueryError(handler.BlockedErrorCode, result2);
							ctx.SetError(squeryError2.online_error, squeryError2.custom_code);
							binder.SendResponseError(squeryError2, payload);
						}
					}
					catch (Exception e)
					{
						Log.Error(e);
						SQueryError squeryError3 = query.QueryError(-1);
						ctx.SetError(squeryError3.online_error, squeryError3.custom_code);
						binder.SendResponseError(squeryError3, payload);
					}
					finally
					{
						ctx.Dispose();
					}
				});
			}
			catch (Exception)
			{
				SQueryError squeryError = query.QueryError(-1);
				ctx.SetError(squeryError.online_error, squeryError.custom_code);
				Log.Error("[QueryManager] Query processing error. Tag: {0}, online id: {1}, id: {2}, type: {3}, sId: {4}, payload: {5}", new object[]
				{
					query.tag,
					query.online_id,
					query.id,
					query.type,
					query.sId,
					payload ?? "null"
				});
				binder.SendResponseError(squeryError, payload);
				ctx.Dispose();
				throw;
			}
			return result;
		}

		// Token: 0x06000798 RID: 1944 RVA: 0x0001CFA4 File Offset: 0x0001B3A4
		internal void HandleResponse(CryOnlineQueryBinder binder, SOnlineQuery query, string payload)
		{
			object result = null;
			SQueryError squeryError = null;
			using (QueryManager.QueryCtxGuard queryCtxGuard = new QueryManager.QueryCtxGuard(this, query.sId, query.id, binder.Handler.Tag, QueryType.Response, query.online_id))
			{
				try
				{
					XmlElement response = null;
					if (!string.IsNullOrEmpty(payload))
					{
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.LoadXml(payload);
						response = xmlDocument.DocumentElement;
					}
					result = binder.Handler.HandleResponse(query, response);
					queryCtxGuard.Succeeded();
				}
				catch (Exception e)
				{
					Log.Error(e);
					squeryError = query.QueryError(EOnlineError.eOnlineError_ParseError, "Query processing error", 0);
					queryCtxGuard.SetError(squeryError.online_error, squeryError.custom_code);
				}
			}
			this.RequestCompleted(query.id, result, squeryError);
		}

		// Token: 0x06000799 RID: 1945 RVA: 0x0001D084 File Offset: 0x0001B484
		internal void HandleError(CryOnlineQueryBinder binder, SQueryError error)
		{
			using (new QueryManager.QueryCtxGuard(this, error.sId, error.id, binder.Handler.Tag, QueryType.Response, error.online_id))
			{
				try
				{
					binder.Handler.OnQueryError(error);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
				Log.Error("[Outgoing request failed: {0} to {1}[{2}][sid:{3}], error code is {4}:{5}", new object[]
				{
					binder.Handler.Tag,
					error.online_id,
					error.id,
					error.sId,
					error.online_error,
					error.custom_code
				});
				this.RequestCompleted(error.id, null, error);
			}
		}

		// Token: 0x0600079A RID: 1946 RVA: 0x0001D16C File Offset: 0x0001B56C
		private void RequestCompleted(int query_id, object result, SQueryError error)
		{
			object request_lock = this.m_request_lock;
			TaskCompletionSource<object> taskCompletionSource;
			lock (request_lock)
			{
				if (!this.m_pending_requests.Pop(query_id, out taskCompletionSource))
				{
					Log.Warning<int>("Response for non-existing request {0} received", query_id);
					return;
				}
			}
			if (error == null)
			{
				taskCompletionSource.SetResult(result);
			}
			else
			{
				taskCompletionSource.SetException(new QueryException(error.online_error, error.description));
			}
		}

		// Token: 0x0600079B RID: 1947 RVA: 0x0001D1F8 File Offset: 0x0001B5F8
		private void NotifyQueryCompleted(QueryContext ctx)
		{
			string text = (!(ctx.Stats.AsyncTime == TimeSpan.Zero)) ? string.Format(" async {0}", ctx.Stats.AsyncTime) : string.Empty;
			QueryType type = ctx.Type;
			if (type != QueryType.Incoming_Request)
			{
				if (type != QueryType.Outgoing_Request)
				{
					if (type == QueryType.Response)
					{
						Log.Info<string>("[Response: {0}", QueryManager.GetBasicQueryContexParamsString(ctx));
					}
				}
				else
				{
					Log.Info<string>("[Outgoing request: {0}", QueryManager.GetBasicQueryContexParamsString(ctx));
				}
			}
			else if (ctx.Stats.Succeeded)
			{
				Log.Info<string, TimeSpan, string>("[Request served: {0} in {1}{2}]", QueryManager.GetBasicQueryContexParamsString(ctx), ctx.Stats.ProcessingTime, text);
			}
			else
			{
				Log.Error("[Request failed: {0} in {1}{2} with error code {3}:{4}]", new object[]
				{
					QueryManager.GetBasicQueryContexParamsString(ctx),
					ctx.Stats.ProcessingTime,
					text,
					ctx.Stats.OnlineError,
					ctx.Stats.CustomCode
				});
			}
			try
			{
				this.QueryCompleted.InvokeEach(ctx);
			}
			catch (AggregateException p)
			{
				Log.Error<string, AggregateException>("QueryCompleted failed on {0} exception: {1}", QueryManager.GetBasicQueryContexParamsString(ctx), p);
			}
		}

		// Token: 0x0600079C RID: 1948 RVA: 0x0001D354 File Offset: 0x0001B754
		private static string GetBasicQueryContexParamsString(QueryContext ctx)
		{
			return string.Format("{0} {1} {2} [id: {3}] [sid: {4}]", new object[]
			{
				ctx.Tag,
				ctx.OnlineID,
				(ctx.Type != QueryType.Outgoing_Request) ? "from" : "to",
				ctx.QueryId,
				ctx.QuerySId
			});
		}

		// Token: 0x0600079D RID: 1949 RVA: 0x0001D3B8 File Offset: 0x0001B7B8
		private string GetOuterXml(string tag, XmlNode node)
		{
			if (!this.m_query_size_apprx_enabled)
			{
				return node.OuterXml;
			}
			QueryManager.QuerySizeApprx querySizeApprx = this.m_query_size_apprx[tag];
			int length = querySizeApprx.Length;
			StringBuilder sb = new StringBuilder(length);
			StringWriter stringWriter = new StringWriter(sb);
			XmlTextWriter w = new XmlTextWriter(stringWriter);
			node.WriteTo(w);
			string text = stringWriter.ToString();
			querySizeApprx.FeedSize(text.Length);
			return text;
		}

		// Token: 0x0600079E RID: 1950 RVA: 0x0001D420 File Offset: 0x0001B820
		public static void RequestSt(string tag, string receiver, params object[] args)
		{
			IQueryManager service = ServicesManager.GetService<IQueryManager>();
			service.Request(tag, receiver, args);
		}

		// Token: 0x0600079F RID: 1951 RVA: 0x0001D43C File Offset: 0x0001B83C
		public void BroadcastRequest(string tag, List<string> recievers, params object[] args)
		{
			QueryManager.BroadcastRequestSt(tag, recievers, args);
		}

		// Token: 0x060007A0 RID: 1952 RVA: 0x0001D446 File Offset: 0x0001B846
		public void BroadcastRequest(string tag, string node, List<string> recievers, params object[] args)
		{
			QueryManager.BroadcastRequestSt(tag, node, recievers, args);
		}

		// Token: 0x060007A1 RID: 1953 RVA: 0x0001D452 File Offset: 0x0001B852
		public static void BroadcastRequestSt(string tag, List<string> recievers, params object[] args)
		{
			QueryManager.BroadcastRequestSt(tag, "k01.", recievers, args);
		}

		// Token: 0x060007A2 RID: 1954 RVA: 0x0001D464 File Offset: 0x0001B864
		public static void BroadcastRequestSt(string tag, string node, List<string> recievers, params object[] args)
		{
			if (recievers.Count == 0)
			{
				return;
			}
			IOnlineClient service = ServicesManager.GetService<IOnlineClient>();
			object[] array = new object[args.Length + 1];
			Array.Copy(args, 0, array, 1, args.Length);
			int num = (int)Math.Ceiling((double)recievers.Count / 100.0);
			for (int i = 0; i < num; i++)
			{
				int num2 = i * 100;
				int count = Math.Min((i + 1) * 100, recievers.Count - num2);
				array[0] = string.Join(",", recievers.GetRange(num2, count).ToArray());
				QueryManager.RequestSt(tag, node + service.XmppHost, array);
			}
		}

		// Token: 0x060007A3 RID: 1955 RVA: 0x0001D510 File Offset: 0x0001B910
		private void OnDALStats(DALProxyStats stats)
		{
			QueryContext queryContext = QueryContext.Current;
			if (queryContext != null && queryContext.Type != QueryType.Response)
			{
				queryContext.Stats.DALCalls.Add(stats);
			}
		}

		// Token: 0x060007A4 RID: 1956 RVA: 0x0001D548 File Offset: 0x0001B948
		private void OnConfigChanged(ConfigEventArgs args)
		{
			if (args.SectionName == "L1Cache" && args.Name == "enabled")
			{
				QueryContext.L1CacheEnabled = (string.Compare(args.sValue, "true", true) == 0);
			}
			if (args.SectionName == "SizeApproximation" && args.Name == "enabled")
			{
				this.m_query_size_apprx_enabled = (string.Compare(args.sValue, "true", true) == 0);
			}
		}

		// Token: 0x04000477 RID: 1143
		private readonly IContainer m_container;

		// Token: 0x04000478 RID: 1144
		private Dictionary<string, CryOnlineQueryBinder> m_query_binders;

		// Token: 0x04000479 RID: 1145
		private object m_request_lock = new object();

		// Token: 0x0400047A RID: 1146
		private CacheDictionary<int, TaskCompletionSource<object>> m_pending_requests;

		// Token: 0x0400047B RID: 1147
		private Dictionary<string, QueryManager.QuerySizeApprx> m_query_size_apprx = new Dictionary<string, QueryManager.QuerySizeApprx>();

		// Token: 0x0400047C RID: 1148
		private bool m_query_size_apprx_enabled;

		// Token: 0x02000196 RID: 406
		private class QuerySizeApprx
		{
			// Token: 0x060007A8 RID: 1960 RVA: 0x0001D5FA File Offset: 0x0001B9FA
			public QuerySizeApprx(EWMA ap)
			{
				this.m_apprx = ap;
			}

			// Token: 0x170000D5 RID: 213
			// (get) Token: 0x060007A9 RID: 1961 RVA: 0x0001D609 File Offset: 0x0001BA09
			public int Length
			{
				get
				{
					return (int)Math.Max(64f, this.m_value * 1.2f);
				}
			}

			// Token: 0x060007AA RID: 1962 RVA: 0x0001D624 File Offset: 0x0001BA24
			public void FeedSize(int payloadSize)
			{
				float num = (float)payloadSize;
				Interlocked.CompareExchange(ref this.m_value, num, 0f);
				Interlocked.Exchange(ref this.m_value, this.m_apprx.Approximate(this.m_value, num));
			}

			// Token: 0x04000483 RID: 1155
			public float m_value;

			// Token: 0x04000484 RID: 1156
			private EWMA m_apprx;
		}

		// Token: 0x02000197 RID: 407
		private class QueryCtxGuard : IDisposable
		{
			// Token: 0x060007AB RID: 1963 RVA: 0x0001D664 File Offset: 0x0001BA64
			public QueryCtxGuard(QueryManager qm, string tag, QueryType t, string onlineId) : this(qm, "-1", -1, tag, t, onlineId)
			{
			}

			// Token: 0x060007AC RID: 1964 RVA: 0x0001D677 File Offset: 0x0001BA77
			public QueryCtxGuard(QueryManager qm, string sid, int id, string tag, QueryType t, string onlineId) : this(qm, new QueryContext(sid, id, tag, t, onlineId))
			{
			}

			// Token: 0x060007AD RID: 1965 RVA: 0x0001D690 File Offset: 0x0001BA90
			protected QueryCtxGuard(QueryManager qm, QueryContext ctx)
			{
				this.qm = qm;
				this.ctx = ctx;
				this.proc_timer = new TimeExecution();
				this.start_thread = Thread.CurrentThread;
				ThreadTracker.Register(this.start_thread, string.Format("{0} {1} from {2}", ctx.Type, ctx.Tag, ctx.OnlineID), ThreadTracker.ReportState.Disabled);
				if (ctx.Type != QueryType.Outgoing_Request)
				{
					CallContext.LogicalSetData(QueryContext.ContextKey, ctx);
				}
			}

			// Token: 0x060007AE RID: 1966 RVA: 0x0001D70B File Offset: 0x0001BB0B
			public void SetQueryID(int id)
			{
				this.ctx.QuerySId = id.ToString();
				this.ctx.QueryId = id;
			}

			// Token: 0x060007AF RID: 1967 RVA: 0x0001D731 File Offset: 0x0001BB31
			public void Succeeded()
			{
				this.ctx.Succedded();
			}

			// Token: 0x060007B0 RID: 1968 RVA: 0x0001D73E File Offset: 0x0001BB3E
			public void SetError(EOnlineError error, int customCode)
			{
				this.ctx.SetError(error, customCode);
			}

			// Token: 0x060007B1 RID: 1969 RVA: 0x0001D750 File Offset: 0x0001BB50
			public void Dispose()
			{
				if (this.disposed)
				{
					return;
				}
				try
				{
					this.ctx.Stats.ProcessingTime = this.proc_timer.Stop();
					ThreadTracker.Unregister(this.start_thread);
					this.qm.NotifyQueryCompleted(this.ctx);
				}
				finally
				{
					if (this.ctx.Type != QueryType.Outgoing_Request)
					{
						CallContext.FreeNamedDataSlot(QueryContext.ContextKey);
					}
					this.disposed = true;
				}
			}

			// Token: 0x04000485 RID: 1157
			private readonly QueryManager qm;

			// Token: 0x04000486 RID: 1158
			private readonly QueryContext ctx;

			// Token: 0x04000487 RID: 1159
			private readonly TimeExecution proc_timer;

			// Token: 0x04000488 RID: 1160
			private readonly Thread start_thread;

			// Token: 0x04000489 RID: 1161
			private bool disposed;
		}
	}
}
