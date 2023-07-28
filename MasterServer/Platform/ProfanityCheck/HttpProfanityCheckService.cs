using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using HK2Net;
using MasterServer.Core.Configuration;
using MasterServer.Core.Web;
using MasterServer.Platform.ProfanityCheck.Exceptions;
using MasterServer.Telemetry.Metrics;
using Network.Http;
using Network.Http.Builders;
using Network.Interfaces;

namespace MasterServer.Platform.ProfanityCheck
{
	// Token: 0x020006B1 RID: 1713
	[Service]
	[Singleton]
	internal class HttpProfanityCheckService : ProfanityCheckService
	{
		// Token: 0x060023FA RID: 9210 RVA: 0x000961B4 File Offset: 0x000945B4
		public HttpProfanityCheckService(IProfanityMetricsTracker profanityMetricsTracker, IHttpClientBuilder webClientBuilder, IHttpRequestFactory webRequestFactory) : base(profanityMetricsTracker)
		{
			this.m_webClientBuilder = webClientBuilder;
			this.m_webRequestFactory = webRequestFactory;
		}

		// Token: 0x060023FB RID: 9211 RVA: 0x000961CB File Offset: 0x000945CB
		public override void Stop()
		{
			if (this.m_webClient != null)
			{
				this.m_webClient.Dispose();
				this.m_webClient = null;
			}
			base.Stop();
		}

		// Token: 0x060023FC RID: 9212 RVA: 0x000961F0 File Offset: 0x000945F0
		protected override void ReadConfigImpl(ConfigSection configSection)
		{
			this.m_urls = (from s in configSection.Get("url").Split(new char[]
			{
				','
			})
			select new Uri(s)).ToArray<Uri>();
			this.m_roomNameTemplate = configSection.Get("room_name_template");
			this.m_urlRequestTemplate = configSection.Get("url_request_template");
			this.m_webClient = this.m_webClientBuilder.Failover(this.m_urls).Build();
		}

		// Token: 0x060023FD RID: 9213 RVA: 0x00096284 File Offset: 0x00094684
		protected override ProfanityCheckResult CheckImpl(ProfanityCheckService.CheckType checkType, ulong userId, string userNickname, string str)
		{
			if (checkType == ProfanityCheckService.CheckType.ClanDescription)
			{
				throw new InvalidOperationException("Profanity checking is not defined for clan description. Filtering should be used instead.");
			}
			if (checkType != ProfanityCheckService.CheckType.RoomName)
			{
				return this.Check(checkType, userId, str);
			}
			if (!string.IsNullOrEmpty(this.m_roomNameTemplate))
			{
				string strA = string.Format(this.m_roomNameTemplate, userNickname);
				if (string.Compare(strA, str, true, CultureInfo.InvariantCulture) == 0)
				{
					return ProfanityCheckResult.Succeeded;
				}
			}
			return (!string.IsNullOrEmpty(str)) ? this.Check(checkType, userId, str) : ProfanityCheckResult.Failed;
		}

		// Token: 0x060023FE RID: 9214 RVA: 0x00096308 File Offset: 0x00094708
		protected override ProfanityCheckResult FilterImpl(ProfanityCheckService.CheckType checkType, ulong userId, StringBuilder builder)
		{
			return ProfanityCheckResult.Succeeded;
		}

		// Token: 0x060023FF RID: 9215 RVA: 0x0009630C File Offset: 0x0009470C
		private ProfanityCheckResult Check(ProfanityCheckService.CheckType checkType, ulong userId, string str)
		{
			ProfanityCheckResult result;
			using (IHttpResponse httpResponse = this.Request(checkType, userId, str))
			{
				result = this.ParseResponse(httpResponse);
			}
			return result;
		}

		// Token: 0x06002400 RID: 9216 RVA: 0x00096350 File Offset: 0x00094750
		private IHttpResponse Request(ProfanityCheckService.CheckType checkType, ulong userId, string str)
		{
			string[] array = string.Format(this.m_urlRequestTemplate, userId, str, (int)checkType).Split(new char[]
			{
				'?'
			});
			string path = array[0];
			string query = array[1];
			IHttpRequest httpRequest = this.m_webRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.Profanity).Method(RequestMethod.GET).UrlPath(path).Query(query).Build();
			IHttpResponse httpResponse = this.m_webClient.MakeRequestBlocking(httpRequest);
			if (httpResponse.StatusCode != HttpStatusCode.OK)
			{
				throw new ProfanityServiceException(string.Format("Profanity request {0} failed with code {1}", httpRequest.Url, httpResponse.StatusCode));
			}
			return httpResponse;
		}

		// Token: 0x06002401 RID: 9217 RVA: 0x000963FC File Offset: 0x000947FC
		protected ProfanityCheckResult ParseResponse(IHttpResponse httpResponse)
		{
			string fullContent = httpResponse.GetFullContent();
			if (string.IsNullOrEmpty(fullContent))
			{
				return ProfanityCheckResult.Failed;
			}
			string text = fullContent.Trim().ToUpper();
			if (text != null)
			{
				if (text == "OK")
				{
					return ProfanityCheckResult.Succeeded;
				}
				if (text == "RESERVED")
				{
					return ProfanityCheckResult.Reserved;
				}
			}
			return ProfanityCheckResult.Failed;
		}

		// Token: 0x0400120A RID: 4618
		protected IRemoteService<IHttpRequest, IHttpResponse> m_webClient;

		// Token: 0x0400120B RID: 4619
		protected readonly IHttpClientBuilder m_webClientBuilder;

		// Token: 0x0400120C RID: 4620
		protected readonly IHttpRequestFactory m_webRequestFactory;

		// Token: 0x0400120D RID: 4621
		private Uri[] m_urls;

		// Token: 0x0400120E RID: 4622
		private string m_roomNameTemplate;

		// Token: 0x0400120F RID: 4623
		private string m_urlRequestTemplate;
	}
}
