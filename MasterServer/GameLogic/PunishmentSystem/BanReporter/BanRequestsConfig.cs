using System;
using MasterServer.Core;
using MasterServer.Core.Configuration;

namespace MasterServer.GameLogic.PunishmentSystem.BanReporter
{
	// Token: 0x02000407 RID: 1031
	public class BanRequestsConfig
	{
		// Token: 0x0600164C RID: 5708 RVA: 0x0005DF58 File Offset: 0x0005C358
		public BanRequestsConfig()
		{
			Config banRequestsConfig = Resources.BanRequestsConfig;
			this.BanRequest = new RequestConfig(banRequestsConfig.GetSection("ban_request"));
			this.UnbanRequest = new RequestConfig(banRequestsConfig.GetSection("unban_request"));
		}

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x0600164D RID: 5709 RVA: 0x0005DF9D File Offset: 0x0005C39D
		// (set) Token: 0x0600164E RID: 5710 RVA: 0x0005DFA5 File Offset: 0x0005C3A5
		public RequestConfig BanRequest { get; private set; }

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x0600164F RID: 5711 RVA: 0x0005DFAE File Offset: 0x0005C3AE
		// (set) Token: 0x06001650 RID: 5712 RVA: 0x0005DFB6 File Offset: 0x0005C3B6
		public RequestConfig UnbanRequest { get; private set; }
	}
}
