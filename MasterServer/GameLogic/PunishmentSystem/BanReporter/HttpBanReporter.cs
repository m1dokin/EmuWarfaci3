using System;
using HK2Net;
using HK2Net.Attributes.Bootstrap;
using Network.Http;
using Network.Http.Builders;
using Network.Interfaces;

namespace MasterServer.GameLogic.PunishmentSystem.BanReporter
{
	// Token: 0x0200040A RID: 1034
	[OrphanService]
	[Singleton]
	[BootstrapSpecific("trunk")]
	internal class HttpBanReporter : BanReporter
	{
		// Token: 0x0600165C RID: 5724 RVA: 0x0005E197 File Offset: 0x0005C597
		public HttpBanReporter(IPunishmentService punishmentService, IHttpClientBuilder webClientBuilder, IHttpRequestFactory webRequestFactory) : base(punishmentService)
		{
			this.m_webClientBuilder = webClientBuilder;
			this.m_banRequestsFormatter = new BanRequestsFormatter(webRequestFactory);
		}

		// Token: 0x0600165D RID: 5725 RVA: 0x0005E1B3 File Offset: 0x0005C5B3
		public override void Init()
		{
			base.Init();
			this.m_webClient = this.m_webClientBuilder.Build();
		}

		// Token: 0x0600165E RID: 5726 RVA: 0x0005E1CC File Offset: 0x0005C5CC
		public override void Stop()
		{
			if (this.m_webClient != null)
			{
				this.m_webClient.Dispose();
				this.m_webClient = null;
			}
			base.Stop();
		}

		// Token: 0x0600165F RID: 5727 RVA: 0x0005E1F4 File Offset: 0x0005C5F4
		protected override void ReportBan(ulong userId, DateTime expiresOn, string message)
		{
			IHttpRequest request = this.m_banRequestsFormatter.CreateBanRequest(userId, expiresOn - DateTime.Now, message);
			IHttpResponse httpResponse = this.m_webClient.MakeRequestBlocking(request);
			httpResponse.Dispose();
		}

		// Token: 0x06001660 RID: 5728 RVA: 0x0005E230 File Offset: 0x0005C630
		protected override void ReportUnban(ulong userId)
		{
			IHttpRequest request = this.m_banRequestsFormatter.CreateUnbanRequest(userId);
			IHttpResponse httpResponse = this.m_webClient.MakeRequestBlocking(request);
			httpResponse.Dispose();
		}

		// Token: 0x04000ADD RID: 2781
		private readonly IHttpClientBuilder m_webClientBuilder;

		// Token: 0x04000ADE RID: 2782
		private readonly BanRequestsFormatter m_banRequestsFormatter;

		// Token: 0x04000ADF RID: 2783
		private IRemoteService<IHttpRequest, IHttpResponse> m_webClient;
	}
}
