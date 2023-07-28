using System;
using System.Text;
using MasterServer.Core.Web;
using Network.Http;
using Network.Http.Builders;

namespace MasterServer.GameLogic.PunishmentSystem.BanReporter
{
	// Token: 0x02000408 RID: 1032
	public class BanRequestsFormatter
	{
		// Token: 0x06001651 RID: 5713 RVA: 0x0005DFBF File Offset: 0x0005C3BF
		public BanRequestsFormatter(IHttpRequestFactory webRequestFactory)
		{
			this.m_webRequestFactory = webRequestFactory;
			this.m_banRequestConfig = new BanRequestsConfig();
		}

		// Token: 0x06001652 RID: 5714 RVA: 0x0005DFDC File Offset: 0x0005C3DC
		public IHttpRequest CreateBanRequest(ulong userId, TimeSpan time, string message)
		{
			string uriString = this.Format(this.m_banRequestConfig.BanRequest.Url, userId, time, message);
			IHttpRequestBuilder httpRequestBuilder = this.m_webRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.ExternalBanService).Method(RequestMethod.POST).Url(new Uri(uriString));
			string text = this.Format(this.m_banRequestConfig.BanRequest.Body, userId, time, message);
			if (!string.IsNullOrEmpty(text))
			{
				string type = this.m_banRequestConfig.BanRequest.Type;
				httpRequestBuilder.Content(type, text, Encoding.ASCII);
			}
			return httpRequestBuilder.Build();
		}

		// Token: 0x06001653 RID: 5715 RVA: 0x0005E070 File Offset: 0x0005C470
		public IHttpRequest CreateUnbanRequest(ulong userId)
		{
			string uriString = this.m_banRequestConfig.UnbanRequest.Url.Replace("%user_id%", userId.ToString());
			return this.m_webRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.ExternalBanService).Method(RequestMethod.POST).Url(new Uri(uriString)).Build();
		}

		// Token: 0x06001654 RID: 5716 RVA: 0x0005E0D0 File Offset: 0x0005C4D0
		private string Format(string template, ulong userId, TimeSpan time, string message)
		{
			template = template.Replace("%user_id%", userId.ToString());
			template = template.Replace("%time%", time.TotalSeconds.ToString());
			template = template.Replace("%message%", message);
			return template;
		}

		// Token: 0x04000AD5 RID: 2773
		public const string UserIdTemplate = "%user_id%";

		// Token: 0x04000AD6 RID: 2774
		public const string TimeTemplate = "%time%";

		// Token: 0x04000AD7 RID: 2775
		public const string MessageTemplate = "%message%";

		// Token: 0x04000AD8 RID: 2776
		private readonly IHttpRequestFactory m_webRequestFactory;

		// Token: 0x04000AD9 RID: 2777
		private BanRequestsConfig m_banRequestConfig;
	}
}
