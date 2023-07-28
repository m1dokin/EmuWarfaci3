using System;
using MasterServer.Core.Configuration;

namespace MasterServer.GameLogic.PunishmentSystem.BanReporter
{
	// Token: 0x02000409 RID: 1033
	public class RequestConfig
	{
		// Token: 0x06001655 RID: 5717 RVA: 0x0005E129 File Offset: 0x0005C529
		public RequestConfig(ConfigSection request)
		{
			this.Url = request.Get("url");
			this.Body = request.Get("body");
			this.Type = request.Get("type");
		}

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x06001656 RID: 5718 RVA: 0x0005E164 File Offset: 0x0005C564
		// (set) Token: 0x06001657 RID: 5719 RVA: 0x0005E16C File Offset: 0x0005C56C
		public string Url { get; private set; }

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x06001658 RID: 5720 RVA: 0x0005E175 File Offset: 0x0005C575
		// (set) Token: 0x06001659 RID: 5721 RVA: 0x0005E17D File Offset: 0x0005C57D
		public string Body { get; private set; }

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x0600165A RID: 5722 RVA: 0x0005E186 File Offset: 0x0005C586
		// (set) Token: 0x0600165B RID: 5723 RVA: 0x0005E18E File Offset: 0x0005C58E
		public string Type { get; private set; }
	}
}
