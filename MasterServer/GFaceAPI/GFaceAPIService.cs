using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Diagnostics.Profiler;
using MasterServer.Core.Services;
using MasterServer.Core.Timers;
using MasterServer.Core.Web;
using MasterServer.GFaceAPI.Responses;
using Network;
using Network.Http;
using Network.Http.Builders;
using Network.Interfaces;
using Network.Metadata;

namespace MasterServer.GFaceAPI
{
	// Token: 0x02000648 RID: 1608
	[Service]
	[Singleton]
	internal class GFaceAPIService : ServiceModule, IGFaceAPIService
	{
		// Token: 0x06002232 RID: 8754 RVA: 0x0008EF1B File Offset: 0x0008D31B
		public GFaceAPIService(IHttpClientBuilder webClientBuilder, IHttpRequestFactory webRequestFactory)
		{
			this.m_webClientBuilder = webClientBuilder;
			this.m_webRequestFactory = webRequestFactory;
		}

		// Token: 0x06002233 RID: 8755 RVA: 0x0008EF34 File Offset: 0x0008D334
		public override void Init()
		{
			this.Config();
			ConfigSection section = Resources.GFaceSettings.GetSection("GFaceAPI").GetSection("account");
			section.OnConfigChanged += this.OnConfigChanged;
		}

		// Token: 0x06002234 RID: 8756 RVA: 0x0008EF74 File Offset: 0x0008D374
		public override void Start()
		{
			this.m_gfaceKeepAliveTimer = new SafeTimer(new TimerCallback(this.GFaceKeepAlivePing), null, (long)(this.m_gfaceKeepAliveTimeout * 1000), (long)(this.m_gfaceKeepAliveTimeout * 1000));
			this.WaitOnRestoreSession(null);
			if (this.m_token == null)
			{
				throw new ServiceModuleException("Failed to start gface API service!");
			}
		}

		// Token: 0x06002235 RID: 8757 RVA: 0x0008EFD4 File Offset: 0x0008D3D4
		public override void Stop()
		{
			this.m_gfaceKeepAliveTimer = null;
			ConfigSection section = Resources.GFaceSettings.GetSection("GFaceAPI").GetSection("account");
			section.OnConfigChanged -= this.OnConfigChanged;
			this.Logout();
			if (this.m_webClient != null)
			{
				this.m_webClient.Dispose();
				this.m_webClient = null;
			}
		}

		// Token: 0x06002236 RID: 8758 RVA: 0x0008F037 File Offset: 0x0008D437
		protected void OnConfigChanged(ConfigEventArgs args)
		{
			this.ConfigAccount(Resources.GFaceSettings.GetSection("GFaceAPI"));
			Log.Info("GfaceAPI Service reconfigured");
		}

		// Token: 0x06002237 RID: 8759 RVA: 0x0008F058 File Offset: 0x0008D458
		protected void Config()
		{
			ConfigSection section = Resources.GFaceSettings.GetSection("GFaceAPI");
			this.ConfigAccount(section);
			Uri[] array = (from sec in section.GetAllSections()["engine"]
			select new Uri(sec.Get("url"))).ToArray<Uri>();
			if (array.Length == 0)
			{
				throw new ServiceModuleException("No gface api URL is in configuration for GFaceAPIService");
			}
			this.m_webClient = this.m_webClientBuilder.Failover(array).Build();
		}

		// Token: 0x06002238 RID: 8760 RVA: 0x0008F0E0 File Offset: 0x0008D4E0
		protected void ConfigAccount(ConfigSection secRoot)
		{
			ConfigSection section = secRoot.GetSection("account");
			this.m_apiAuth = new GFaceAPIService.ApiAuth(section.Get("apikey"), section.Get("pwd"));
			int gfaceKeepAliveTimeout;
			section.Get("keepaliveperiod", out gfaceKeepAliveTimeout);
			int loginRetryIntervalMS;
			section.Get("loginretryintervalms", out loginRetryIntervalMS);
			int loginRetryAttempts;
			section.Get("loginretryattempts", out loginRetryAttempts);
			int sessionRestorationIntervalMS;
			section.Get("sessionrestorationintervalms", out sessionRestorationIntervalMS);
			int sessionRestorationAttempts;
			section.Get("sessionrestorationattempts", out sessionRestorationAttempts);
			this.m_gfaceKeepAliveTimeout = gfaceKeepAliveTimeout;
			this.m_loginRetryIntervalMS = loginRetryIntervalMS;
			this.m_loginRetryAttempts = loginRetryAttempts;
			this.m_sessionRestorationIntervalMS = sessionRestorationIntervalMS;
			this.m_sessionRestorationAttempts = sessionRestorationAttempts;
			if (base.State == ServiceState.Started && this.m_webClient != null && this.m_gfaceKeepAliveTimer != null)
			{
				this.m_gfaceKeepAliveTimer.Change(0L, (long)(this.m_gfaceKeepAliveTimeout * 1000));
			}
		}

		// Token: 0x06002239 RID: 8761 RVA: 0x0008F1CC File Offset: 0x0008D5CC
		public bool Request(CallOptions opt, APIBundle api, params object[] args)
		{
			GFaceError gfaceError;
			return this.Request(out gfaceError, opt, api, args);
		}

		// Token: 0x0600223A RID: 8762 RVA: 0x0008F1E4 File Offset: 0x0008D5E4
		public bool Request(out GFaceError err, CallOptions opt, APIBundle api, params object[] args)
		{
			try
			{
				Task<object> task = (opt != CallOptions.Reliable) ? this.SimpleRequestUnreliableImpl(api, args) : this.SimpleRequestImpl(api, args);
				task.Wait();
			}
			catch (Exception ex)
			{
				GFaceException ex2 = ex.InnerException as GFaceException;
				if (ex2 != null)
				{
					err = ex2.ErrorInfo;
					return false;
				}
				throw;
			}
			err = GFaceError.TheNoError;
			return true;
		}

		// Token: 0x0600223B RID: 8763 RVA: 0x0008F260 File Offset: 0x0008D660
		public void RequestAsync(Action<Exception> responseCB, CallOptions opt, APIBundle api, params object[] args)
		{
			Task<object> task = (opt != CallOptions.Reliable) ? this.SimpleRequestUnreliableImpl(api, args) : this.SimpleRequestImpl(api, args);
			task.ContinueWith(delegate(Task<object> t1)
			{
				responseCB(t1.Exception);
			});
		}

		// Token: 0x0600223C RID: 8764 RVA: 0x0008F2AB File Offset: 0x0008D6AB
		public Task RequestAsync(CallOptions opt, APIBundle api, params object[] args)
		{
			return (opt != CallOptions.Reliable) ? this.SimpleRequestUnreliableImpl(api, args) : this.SimpleRequestImpl(api, args);
		}

		// Token: 0x0600223D RID: 8765 RVA: 0x0008F2C8 File Offset: 0x0008D6C8
		public TResult Request<TResult>(CallOptions opt, APIBundle api, params object[] args) where TResult : class
		{
			Task<TResult> task = (opt != CallOptions.Reliable) ? this.CompleteRequestUnreliableImpl<TResult>(api, args) : this.CompleteRequestImpl<TResult>(api, args);
			task.Wait();
			return task.Result;
		}

		// Token: 0x0600223E RID: 8766 RVA: 0x0008F300 File Offset: 0x0008D700
		public void RequestAsync<TResult>(Action<TResult, Exception> responseCB, CallOptions opt, APIBundle api, params object[] args) where TResult : class
		{
			Task<TResult> task = (opt != CallOptions.Reliable) ? this.CompleteRequestUnreliableImpl<TResult>(api, args) : this.CompleteRequestImpl<TResult>(api, args);
			task.ContinueWith(delegate(Task<TResult> t1)
			{
				responseCB(t1.Result, t1.Exception);
			});
		}

		// Token: 0x0600223F RID: 8767 RVA: 0x0008F34B File Offset: 0x0008D74B
		public Task<TResult> RequestAsync<TResult>(CallOptions opt, APIBundle api, params object[] args) where TResult : class
		{
			return (opt != CallOptions.Reliable) ? this.CompleteRequestUnreliableImpl<TResult>(api, args) : this.CompleteRequestImpl<TResult>(api, args);
		}

		// Token: 0x06002240 RID: 8768 RVA: 0x0008F368 File Offset: 0x0008D768
		private Task<object> SimpleRequestImpl(APIBundle api, params object[] args)
		{
			return this.GetResponseReliable(delegate(string s)
			{
				SimpleResponseParser simpleResponseParser = new SimpleResponseParser(s, true);
				return null;
			}, api, args);
		}

		// Token: 0x06002241 RID: 8769 RVA: 0x0008F38F File Offset: 0x0008D78F
		private Task<object> SimpleRequestUnreliableImpl(APIBundle api, params object[] args)
		{
			return this.GetResponseUnreliable(delegate(string s)
			{
				SimpleResponseParser simpleResponseParser = new SimpleResponseParser(s, true);
				return null;
			}, api, args);
		}

		// Token: 0x06002242 RID: 8770 RVA: 0x0008F3B8 File Offset: 0x0008D7B8
		private Task<TResult> CompleteRequestImpl<TResult>(APIBundle api, params object[] args) where TResult : class
		{
			Task<object> responseReliable = this.GetResponseReliable(delegate(string s)
			{
				CompleteResponseParser<TResult> completeResponseParser = new CompleteResponseParser<TResult>(s);
				return completeResponseParser.Response;
			}, api, args);
			return responseReliable.ContinueWith<TResult>((Task<object> t1) => (TResult)((object)t1.Result));
		}

		// Token: 0x06002243 RID: 8771 RVA: 0x0008F3EC File Offset: 0x0008D7EC
		private Task<TResult> CompleteRequestUnreliableImpl<TResult>(APIBundle api, params object[] args) where TResult : class
		{
			Task<object> responseUnreliable = this.GetResponseUnreliable(delegate(string s)
			{
				CompleteResponseParser<TResult> completeResponseParser = new CompleteResponseParser<TResult>(s);
				return completeResponseParser.Response;
			}, api, args);
			return responseUnreliable.ContinueWith<TResult>((Task<object> t1) => (TResult)((object)t1.Result));
		}

		// Token: 0x06002244 RID: 8772 RVA: 0x0008F420 File Offset: 0x0008D820
		private Task<object> GetResponseUnreliable(Func<string, object> parseFunc, APIBundle api, params object[] args)
		{
			object token = this.m_token;
			return this.GetResponseAsync(parseFunc, token, GFaceAPIService.AllocStrategy.Drop, api, args);
		}

		// Token: 0x06002245 RID: 8773 RVA: 0x0008F440 File Offset: 0x0008D840
		private Task<object> GetResponseReliable(Func<string, object> parseFunc, APIBundle api, params object[] args)
		{
			GFaceAPIService.APIAsyncCallState state = new GFaceAPIService.APIAsyncCallState(parseFunc, api, args);
			object tokenCopy = this.m_token;
			Task<object> responseAsync = this.GetResponseAsync(parseFunc, tokenCopy, GFaceAPIService.AllocStrategy.Wait, api, args);
			responseAsync.ContinueWith(delegate(Task<object> t1)
			{
				this.GetResponseReliableCompleted(t1, tokenCopy, state);
			});
			return state.tcs.Task;
		}

		// Token: 0x06002246 RID: 8774 RVA: 0x0008F4A8 File Offset: 0x0008D8A8
		private void GetResponseReliableCompleted(Task<object> t1, object token, GFaceAPIService.APIAsyncCallState state)
		{
			if (!t1.IsFaulted)
			{
				state.tcs.SetResult(t1.Result);
				return;
			}
			AggregateException exception = t1.Exception;
			if (--state.attemptCountDown == 0)
			{
				state.tcs.SetException(t1.Exception);
				return;
			}
			AggregateException exception2 = t1.Exception;
			state.tcs.SetException(t1.Exception);
		}

		// Token: 0x06002247 RID: 8775 RVA: 0x0008F548 File Offset: 0x0008D948
		private Task<object> GetResponseAsync(Func<string, object> parseFunc, object token, GFaceAPIService.AllocStrategy allocStrategy, APIBundle api, params object[] args)
		{
			Task<string> task = this.DoHttpRequest(token, allocStrategy, api, args);
			return task.ContinueWith<object>((Task<string> t1) => parseFunc(t1.Result));
		}

		// Token: 0x06002248 RID: 8776 RVA: 0x0008F584 File Offset: 0x0008D984
		private Task<string> DoHttpRequest(object token, GFaceAPIService.AllocStrategy allocStrategy, APIBundle api, params object[] args)
		{
			object[] args2;
			if (token == null)
			{
				args2 = args;
			}
			else
			{
				args2 = (args.Clone() as object[]);
				GFaceAPIService.ReplaceServerTokenPlaceHolders(ref args2, token);
			}
			return this.DoHttpRequest(allocStrategy, api, args2);
		}

		// Token: 0x06002249 RID: 8777 RVA: 0x0008F5C0 File Offset: 0x0008D9C0
		private Task<string> DoHttpRequest(GFaceAPIService.AllocStrategy allocStrategy, APIBundle api, params object[] args)
		{
			string callSign = GFaceAPIService.GenCallSignature(api, args);
			Log.Verbose(Log.Group.GFaceAPI, "GFaceAPI call initiated : " + callSign, new object[0]);
			TimeExecution timer = new TimeExecution();
			IHttpRequestBuilder httpRequestBuilder = this.m_webRequestFactory.NewRequest().Reliability((allocStrategy != GFaceAPIService.AllocStrategy.Wait) ? RemoteRequestReliability.Unreliable : RemoteRequestReliability.Reliable).Domain<RequestDomain>(RequestDomain.GFace).Method(api.Method).UrlPath(GFaceAPIService.FixURL(api.APIString));
			IHttpRequest request = (api.Method != RequestMethod.GET) ? httpRequestBuilder.FormParams(args).Build() : httpRequestBuilder.QueryParams(args).Build();
			Task<IHttpResponse> task = this.m_webClient.MakeRequest(request);
			return task.ContinueWith<string>(delegate(Task<IHttpResponse> t1)
			{
				if (t1.IsFaulted)
				{
					RemoteRequestException<IHttpRequest> remoteRequestException = t1.Exception.Flatten().InnerExceptions.OfType<RemoteRequestException<IHttpRequest>>().FirstOrDefault<RemoteRequestException<IHttpRequest>>();
					if (remoteRequestException != null)
					{
						GFaceAPIService.TranslateWebRequestException(remoteRequestException).Rethrow();
					}
				}
				Log.Verbose(Log.Group.GFaceAPI, "GFaceAPI call ended in {0} : {1}", new object[]
				{
					timer.Stop(),
					callSign
				});
				string result2;
				using (IHttpResponse result = t1.Result)
				{
					result2 = new StreamReader(result.ContentStream).ReadToEnd();
				}
				return result2;
			});
		}

		// Token: 0x0600224A RID: 8778 RVA: 0x0008F698 File Offset: 0x0008DA98
		private Task<object> DoLoginQueryAsync(string apikey, string pwd)
		{
			return this.GetResponseAsync(delegate(string s)
			{
				CompleteResponseParser<RspAuth> completeResponseParser = new CompleteResponseParser<RspAuth>(s);
				return completeResponseParser.Response.auth.token;
			}, null, GFaceAPIService.AllocStrategy.Wait, GFaceAPIs.auth_login_server, new object[]
			{
				"apikey",
				apikey,
				"pwd",
				pwd
			});
		}

		// Token: 0x0600224B RID: 8779 RVA: 0x0008F6ED File Offset: 0x0008DAED
		private void Logout()
		{
		}

		// Token: 0x0600224C RID: 8780 RVA: 0x0008F6F0 File Offset: 0x0008DAF0
		private void WaitOnRestoreSession(object expiredToken)
		{
			try
			{
				Task<bool> task = this.RestoreSessionAsync(expiredToken, this.m_loginRetryAttempts, this.m_loginRetryIntervalMS);
				task.Wait();
			}
			catch (GFaceSystemException ex)
			{
				throw new ServiceModuleException("Fatal error happened while trying to restore gface session: " + ex.Message);
			}
			catch (Exception ex2)
			{
				throw new ServiceModuleException("All Gface session restoration attempts failed! Cannot continue! The LastException: " + ex2.Message);
			}
		}

		// Token: 0x0600224D RID: 8781 RVA: 0x0008F76C File Offset: 0x0008DB6C
		private Task<bool> RestoreSessionAsync(object expiredToken)
		{
			GFaceAPIService.SessionRestorationState state = new GFaceAPIService.SessionRestorationState(expiredToken);
			return this.RestoreSessionAsync(state);
		}

		// Token: 0x0600224E RID: 8782 RVA: 0x0008F788 File Offset: 0x0008DB88
		private Task<bool> RestoreSessionAsync(object expiredToken, int attempts, int intervalMS)
		{
			GFaceAPIService.SessionRestorationState state = new GFaceAPIService.SessionRestorationState(expiredToken, attempts, intervalMS);
			return this.RestoreSessionAsync(state);
		}

		// Token: 0x0600224F RID: 8783 RVA: 0x0008F7A8 File Offset: 0x0008DBA8
		private Task<bool> RestoreSessionAsync(GFaceAPIService.SessionRestorationState state)
		{
			if (Interlocked.CompareExchange(ref this.m_loggingInFlag, 1, 0) != 0)
			{
				state.tcs.SetResult(true);
				return state.tcs.Task;
			}
			Task<bool> task;
			try
			{
				if (this.m_token != state.expiredToken)
				{
					state.tcs.SetResult(true);
					this.m_loggingInFlag = 0;
					task = state.tcs.Task;
				}
				else
				{
					string apikey;
					string pwd;
					this.m_apiAuth.Get(out apikey, out pwd);
					Task<object> task2 = this.DoLoginQueryAsync(apikey, pwd);
					task2.ContinueWith(delegate(Task<object> t1)
					{
						this.RestoreAttemptCompleted(t1, state);
					});
					task = state.tcs.Task;
				}
			}
			catch
			{
				this.m_loggingInFlag = 0;
				throw;
			}
			return task;
		}

		// Token: 0x06002250 RID: 8784 RVA: 0x0008F8A4 File Offset: 0x0008DCA4
		private void RestoreAttemptCompleted(Task<object> t1, GFaceAPIService.SessionRestorationState state)
		{
			try
			{
				if (!t1.IsFaulted)
				{
					object result = t1.Result;
					if (result != null)
					{
						Interlocked.CompareExchange(ref this.m_token, result, state.expiredToken);
						state.tcs.SetResult(true);
						this.m_loggingInFlag = 0;
						return;
					}
				}
				Exception ex = t1.Exception.InnerException;
				if (--state.attepmtCountDown == 0 || (ex != null && ex is GFaceSystemException))
				{
					if (ex == null)
					{
						ex = new ApplicationException("Gface session restoration failed with no Exception!");
					}
					Log.Error(ex);
					state.tcs.SetException(ex);
					this.m_loggingInFlag = 0;
				}
				else
				{
					TimerCallback timerCallback = delegate(object st)
					{
						string apikey;
						string pwd;
						this.m_apiAuth.Get(out apikey, out pwd);
						Task<object> task = this.DoLoginQueryAsync(apikey, pwd);
						task.ContinueWith(delegate(Task<object> t2x)
						{
							this.RestoreAttemptCompleted(t2x, (GFaceAPIService.SessionRestorationState)st);
						});
					};
					if (state.attemptIntervalMS == 0)
					{
						timerCallback(state);
					}
					else
					{
						state.attemptTimer = new SafeTimer(timerCallback, state, (long)state.attemptIntervalMS, -1L);
					}
				}
			}
			catch
			{
				this.m_loggingInFlag = 0;
				throw;
			}
		}

		// Token: 0x06002251 RID: 8785 RVA: 0x0008F9B0 File Offset: 0x0008DDB0
		private void GFaceKeepAlivePing(object _notused)
		{
			object tokenCopy = this.m_token;
			if (tokenCopy == null)
			{
				return;
			}
			Task<object> responseAsync = this.GetResponseAsync(delegate(string s)
			{
				SimpleResponseParser simpleResponseParser = new SimpleResponseParser(s, false);
				return simpleResponseParser.ErrorCode;
			}, null, GFaceAPIService.AllocStrategy.Wait, GFaceAPIs.auth_session_check, new object[]
			{
				"token",
				tokenCopy
			});
			responseAsync.ContinueWith(delegate(Task<object> t1)
			{
				if (t1.IsFaulted)
				{
					Log.Error(t1.Exception);
					AggregateException exception = t1.Exception;
					Log.Warning<string>("{0}", "Gface session expired! Now retrying.");
					this.WaitOnRestoreSession(tokenCopy);
				}
			});
		}

		// Token: 0x06002252 RID: 8786 RVA: 0x0008FA37 File Offset: 0x0008DE37
		private static string FixURL(string apiStr)
		{
			if (apiStr.Length == 0 || apiStr[0] != '/')
			{
				return apiStr;
			}
			return apiStr.Substring(1);
		}

		// Token: 0x06002253 RID: 8787 RVA: 0x0008FA5C File Offset: 0x0008DE5C
		private static string GenCallSignature(APIBundle api, object[] args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("<<<{0}:{1}(", api.Method, api.APIString);
			foreach (object obj in args)
			{
				stringBuilder.Append(",");
				stringBuilder.Append(obj.ToString());
			}
			stringBuilder.Append(")>>>");
			return stringBuilder.ToString();
		}

		// Token: 0x06002254 RID: 8788 RVA: 0x0008FAD4 File Offset: 0x0008DED4
		private static void ReplaceServerTokenPlaceHolders(ref object[] args, object token)
		{
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] is ServerTokenPlaceHolder)
				{
					args[i] = (token ?? "0");
				}
			}
		}

		// Token: 0x06002255 RID: 8789 RVA: 0x0008FB18 File Offset: 0x0008DF18
		protected static GFaceError TranslateWebRequestException(RemoteRequestException<IHttpRequest> ex)
		{
			if (ex is RemoteRequestHttpException)
			{
				HttpStatusCode statusCode = ((RemoteRequestHttpException)ex).StatusCode;
				switch (statusCode)
				{
				case HttpStatusCode.BadRequest:
					return new GFaceError(GErrorCode.RequestError, ex.Message);
				default:
					switch (statusCode)
					{
					case HttpStatusCode.InternalServerError:
					case HttpStatusCode.ServiceUnavailable:
						break;
					default:
						if (statusCode == HttpStatusCode.OK)
						{
							return GFaceError.TheNoError;
						}
						if (statusCode != HttpStatusCode.RequestTimeout)
						{
						}
						break;
					}
					return new GFaceError(GErrorCode.NetworkError, ex.Message);
				case HttpStatusCode.Forbidden:
				case HttpStatusCode.NotFound:
					return new GFaceError(GErrorCode.FatalNetworkError, ex.Message);
				}
			}
			else
			{
				if (ex is RemoteRequestSocketException<IHttpRequest>)
				{
					SocketError statusCode2 = ((RemoteRequestSocketException<IHttpRequest>)ex).StatusCode;
					switch (statusCode2)
					{
					case SocketError.NetworkUnreachable:
					case SocketError.ConnectionAborted:
					case SocketError.ConnectionReset:
					case SocketError.NotConnected:
					case SocketError.TimedOut:
						break;
					default:
						if (statusCode2 != SocketError.HostNotFound && statusCode2 != SocketError.TryAgain)
						{
							if (statusCode2 == SocketError.Success)
							{
								return GFaceError.TheNoError;
							}
							if (statusCode2 != SocketError.HostUnreachable && statusCode2 != SocketError.SystemNotReady)
							{
							}
						}
						break;
					case SocketError.ConnectionRefused:
						return new GFaceError(GErrorCode.FatalNetworkError, ex.Message);
					}
					return new GFaceError(GErrorCode.NetworkError, ex.Message);
				}
				if (ex is RemoteRequestAllocationException<IHttpRequest>)
				{
					return new GFaceError(GErrorCode.RequestPoolTimeout, string.Empty);
				}
				return new GFaceError(GErrorCode.NetworkError, ex.Message);
			}
		}

		// Token: 0x06002256 RID: 8790 RVA: 0x0008FC90 File Offset: 0x0008E090
		private static void ThrowUnexpectedResultException(string api, string returnedType)
		{
			GFaceError gfaceError = new GFaceError(GErrorCode.ParserError, string.Format("Unrecognized GFace response tag. api: {0}, returned: {1}", api, (returnedType != null) ? returnedType : "null"));
			gfaceError.Rethrow();
		}

		// Token: 0x06002257 RID: 8791 RVA: 0x0008FCCC File Offset: 0x0008E0CC
		private static void ThrowUnexpectedNumberofResultsException(string api)
		{
			GFaceError gfaceError = new GFaceError(GErrorCode.ParserError, string.Format("Unexpected number of results. api: {0}, expects: {1}", api, 1));
			gfaceError.Rethrow();
		}

		// Token: 0x040010FE RID: 4350
		private readonly IHttpClientBuilder m_webClientBuilder;

		// Token: 0x040010FF RID: 4351
		private readonly IHttpRequestFactory m_webRequestFactory;

		// Token: 0x04001100 RID: 4352
		private IRemoteService<IHttpRequest, IHttpResponse> m_webClient;

		// Token: 0x04001101 RID: 4353
		protected SafeTimer m_gfaceKeepAliveTimer;

		// Token: 0x04001102 RID: 4354
		private object m_token;

		// Token: 0x04001103 RID: 4355
		private int m_loggingInFlag;

		// Token: 0x04001104 RID: 4356
		protected volatile GFaceAPIService.ApiAuth m_apiAuth;

		// Token: 0x04001105 RID: 4357
		protected volatile int m_gfaceKeepAliveTimeout;

		// Token: 0x04001106 RID: 4358
		protected volatile int m_loginRetryIntervalMS;

		// Token: 0x04001107 RID: 4359
		protected volatile int m_loginRetryAttempts;

		// Token: 0x04001108 RID: 4360
		protected volatile int m_sessionRestorationIntervalMS;

		// Token: 0x04001109 RID: 4361
		protected volatile int m_sessionRestorationAttempts;

		// Token: 0x02000649 RID: 1609
		private enum AllocStrategy
		{
			// Token: 0x04001110 RID: 4368
			Wait,
			// Token: 0x04001111 RID: 4369
			Drop
		}

		// Token: 0x0200064A RID: 1610
		protected class ApiAuth
		{
			// Token: 0x06002262 RID: 8802 RVA: 0x0008FE28 File Offset: 0x0008E228
			public ApiAuth(string apiKey, string pwd)
			{
				this.m_apiKey = apiKey;
				this.m_pwd = pwd;
			}

			// Token: 0x06002263 RID: 8803 RVA: 0x0008FE3E File Offset: 0x0008E23E
			public void Get(out string apiKey, out string pwd)
			{
				apiKey = this.m_apiKey;
				pwd = this.m_pwd;
			}

			// Token: 0x04001112 RID: 4370
			private string m_apiKey;

			// Token: 0x04001113 RID: 4371
			private string m_pwd;
		}

		// Token: 0x0200064B RID: 1611
		private class APIAsyncCallState
		{
			// Token: 0x06002264 RID: 8804 RVA: 0x0008FE50 File Offset: 0x0008E250
			public APIAsyncCallState(Func<string, object> parseFunc, APIBundle api, object[] args)
			{
				this.tcs = new TaskCompletionSource<object>();
				this.api = api;
				this.args = args;
				this.parseFunc = parseFunc;
				this.attemptCountDown = 2;
			}

			// Token: 0x04001114 RID: 4372
			public readonly TaskCompletionSource<object> tcs;

			// Token: 0x04001115 RID: 4373
			public readonly APIBundle api;

			// Token: 0x04001116 RID: 4374
			public readonly object[] args;

			// Token: 0x04001117 RID: 4375
			public readonly Func<string, object> parseFunc;

			// Token: 0x04001118 RID: 4376
			public volatile int attemptCountDown;
		}

		// Token: 0x0200064C RID: 1612
		private class SessionRestorationState
		{
			// Token: 0x06002265 RID: 8805 RVA: 0x0008FE81 File Offset: 0x0008E281
			public SessionRestorationState(object expiredToken) : this(expiredToken, 1, -1)
			{
			}

			// Token: 0x06002266 RID: 8806 RVA: 0x0008FE8C File Offset: 0x0008E28C
			public SessionRestorationState(object expiredToken, int attempts, int attemptIntervalMS)
			{
				this.tcs = new TaskCompletionSource<bool>();
				this.expiredToken = expiredToken;
				this.attemptIntervalMS = attemptIntervalMS;
				this.attepmtCountDown = ((attempts >= 1) ? attempts : 1);
			}

			// Token: 0x04001119 RID: 4377
			public readonly TaskCompletionSource<bool> tcs;

			// Token: 0x0400111A RID: 4378
			public readonly object expiredToken;

			// Token: 0x0400111B RID: 4379
			public readonly int attemptIntervalMS;

			// Token: 0x0400111C RID: 4380
			public SafeTimer attemptTimer;

			// Token: 0x0400111D RID: 4381
			public int attepmtCountDown;
		}
	}
}
