using System;
using HK2Net;
using HK2Net.Attributes.Bootstrap;
using MasterServer.Core;

namespace MasterServer.GameLogic.PunishmentSystem.BanReporter
{
	// Token: 0x0200040B RID: 1035
	[OrphanService]
	[Singleton]
	[BootstrapSpecific("west_emul")]
	internal class LogBanReporter : BanReporter
	{
		// Token: 0x06001661 RID: 5729 RVA: 0x0005E25D File Offset: 0x0005C65D
		public LogBanReporter(IPunishmentService punishmentService) : base(punishmentService)
		{
		}

		// Token: 0x06001662 RID: 5730 RVA: 0x0005E266 File Offset: 0x0005C666
		protected override void ReportBan(ulong userId, DateTime expiresOn, string message)
		{
			Log.Info(string.Format("[LogBanReporter] user {0} was reported as banned till {1}. Reason: {2}", userId, expiresOn, message));
		}

		// Token: 0x06001663 RID: 5731 RVA: 0x0005E284 File Offset: 0x0005C684
		protected override void ReportUnban(ulong userId)
		{
			Log.Info(string.Format("[LogBanReporter] user {0} was reported as unbanned", userId));
		}
	}
}
