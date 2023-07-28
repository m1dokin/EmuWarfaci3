using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
	// Token: 0x0200021A RID: 538
	[DebugQuery]
	[QueryAttributes(TagName = "debug_http_request_async")]
	internal class DebugHttpRequestAsync : BaseQuery
	{
		// Token: 0x06000BB1 RID: 2993 RVA: 0x0002C280 File Offset: 0x0002A680
		public DebugHttpRequestAsync(IHttpClientBuilder webClientBuilder, IHttpRequestFactory webRequestFactory)
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

		// Token: 0x06000BB2 RID: 2994 RVA: 0x0002C30C File Offset: 0x0002A70C
		public override Task<int> HandleRequestAsync(SOnlineQuery query, XmlElement request, XmlElement response)
		{
			Task<int> result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "DebugHttpRequestAsync"))
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
				Task<IHttpResponse> task = this.m_webClient.MakeRequest(request2);
				result = task.ContinueWith<int>(delegate(Task<IHttpResponse> t)
				{
					int result3;
					using (IHttpResponse result2 = t.Result)
					{
						string value = new StreamReader(result2.ContentStream).ReadToEnd();
						response.SetAttribute("response", value);
						result3 = 0;
					}
					return result3;
				});
			}
			return result;
		}

		// Token: 0x06000BB3 RID: 2995 RVA: 0x0002C3FC File Offset: 0x0002A7FC
		public override void Dispose()
		{
			if (this.m_webClient != null)
			{
				this.m_webClient.Dispose();
				this.m_webClient = null;
			}
		}

		// Token: 0x0400056D RID: 1389
		private readonly IHttpRequestFactory m_webRequestFactory;

		// Token: 0x0400056E RID: 1390
		private IRemoteService<IHttpRequest, IHttpResponse> m_webClient;

		// Token: 0x0400056F RID: 1391
		private readonly string m_requestUrl;
	}
}
