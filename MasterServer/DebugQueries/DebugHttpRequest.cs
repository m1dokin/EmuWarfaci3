using System;
using System.IO;
using System.Linq;
using System.Xml;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Web;
using MasterServer.CryOnlineNET;
using Network.Http;
using Network.Http.Builders;
using Network.Interfaces;

namespace MasterServer.DebugQueries
{
	// Token: 0x02000219 RID: 537
	[DebugQuery]
	[QueryAttributes(TagName = "debug_http_request")]
	internal class DebugHttpRequest : BaseQuery
	{
		// Token: 0x06000BAD RID: 2989 RVA: 0x0002C0BC File Offset: 0x0002A4BC
		public DebugHttpRequest(IHttpClientBuilder webClientBuilder, IHttpRequestFactory webRequestFactory)
		{
			ConfigSection section = Resources.XMPPSettings.GetSection("xmpp");
			Uri[] hostUrls = (from s in section.Get("auth_url").Split(new char[]
			{
				','
			})
			select new Uri(s)).ToArray<Uri>();
			this.m_requestUrl = "debug.php";
			this.m_webClient = webClientBuilder.Failover(hostUrls).Build();
			this.m_webRequestFactory = webRequestFactory;
		}

		// Token: 0x06000BAE RID: 2990 RVA: 0x0002C148 File Offset: 0x0002A548
		public override int HandleRequest(SOnlineQuery query, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "DebugHttpRequest"))
			{
				int num = int.Parse(request.GetAttribute("timeout"));
				int num2 = int.Parse(request.GetAttribute("data"));
				IHttpRequest request2 = this.m_webRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.Debug).UrlPath(this.m_requestUrl).QueryParams(new object[]
				{
					"action",
					"sleep",
					"timeout",
					num,
					"data",
					num2
				}).Build();
				using (IHttpResponse httpResponse = this.m_webClient.MakeRequestBlocking(request2))
				{
					string value = new StreamReader(httpResponse.ContentStream).ReadToEnd();
					response.SetAttribute("response", value);
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x06000BAF RID: 2991 RVA: 0x0002C258 File Offset: 0x0002A658
		public override void Dispose()
		{
			if (this.m_webClient != null)
			{
				this.m_webClient.Dispose();
				this.m_webClient = null;
			}
		}

		// Token: 0x04000569 RID: 1385
		private readonly IHttpRequestFactory m_webRequestFactory;

		// Token: 0x0400056A RID: 1386
		private IRemoteService<IHttpRequest, IHttpResponse> m_webClient;

		// Token: 0x0400056B RID: 1387
		private readonly string m_requestUrl;
	}
}
