using System;
using MasterServer.Core.Services;

namespace MasterServer.GameLogic.PunishmentSystem.BanReporter
{
	// Token: 0x02000406 RID: 1030
	internal abstract class BanReporter : ServiceModule
	{
		// Token: 0x06001647 RID: 5703 RVA: 0x0005DED7 File Offset: 0x0005C2D7
		protected BanReporter(IPunishmentService punishmentService)
		{
			this.m_punishmentService = punishmentService;
		}

		// Token: 0x06001648 RID: 5704 RVA: 0x0005DEE6 File Offset: 0x0005C2E6
		public override void Init()
		{
			base.Init();
			this.m_punishmentService.PlayerBanned += this.ReportBan;
			this.m_punishmentService.PlayerUnBanned += this.ReportUnban;
		}

		// Token: 0x06001649 RID: 5705 RVA: 0x0005DF1E File Offset: 0x0005C31E
		public override void Stop()
		{
			this.m_punishmentService.PlayerBanned -= this.ReportBan;
			this.m_punishmentService.PlayerUnBanned -= this.ReportUnban;
			base.Stop();
		}

		// Token: 0x0600164A RID: 5706
		protected abstract void ReportBan(ulong userId, DateTime expiresOn, string message);

		// Token: 0x0600164B RID: 5707
		protected abstract void ReportUnban(ulong userId);

		// Token: 0x04000AD2 RID: 2770
		private readonly IPunishmentService m_punishmentService;
	}
}
