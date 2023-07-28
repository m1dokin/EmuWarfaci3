using System;
using HK2Net;
using MasterServer.Core.Configuration;
using Network.Http.Builders;

namespace MasterServer.Core.Web
{
	// Token: 0x0200016B RID: 363
	[Service]
	[Singleton]
	internal class MSHttpRequestFactory : IHttpRequestFactory
	{
		// Token: 0x0600067F RID: 1663 RVA: 0x0001A5C8 File Offset: 0x000189C8
		public MSHttpRequestFactory()
		{
			this.m_cfg = this.ReadConfig();
			ConfigSection section = Resources.ModuleSettings.GetSection("WebRequestPool");
			section.OnConfigChanged += this.OnConfigChanged;
		}

		// Token: 0x06000680 RID: 1664 RVA: 0x0001A60C File Offset: 0x00018A0C
		private MSWebRequestConfig ReadConfig()
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("WebRequestPool");
			return new MSWebRequestConfig(section);
		}

		// Token: 0x06000681 RID: 1665 RVA: 0x0001A62F File Offset: 0x00018A2F
		public IHttpRequestBuilder NewRequest()
		{
			return new HttpRequestBuilder().KeepAlive(this.m_cfg.KeepAlive).Timeout(this.m_cfg.DefaultTimeout);
		}

		// Token: 0x06000682 RID: 1666 RVA: 0x0001A656 File Offset: 0x00018A56
		private void OnConfigChanged(ConfigEventArgs args)
		{
			this.m_cfg = this.ReadConfig();
		}

		// Token: 0x04000403 RID: 1027
		private const string CFG_SECTION = "WebRequestPool";

		// Token: 0x04000404 RID: 1028
		private MSWebRequestConfig m_cfg;
	}
}
