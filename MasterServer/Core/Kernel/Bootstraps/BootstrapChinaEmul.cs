using System;
using MasterServer.Core.Application.Bootstrap;
using MasterServer.Core.Kernel.Scanners;
using MasterServer.MySqlQueries;
using MasterServer.Platform.Nickname;
using MasterServer.Platform.ProfanityCheck;

namespace MasterServer.Core.Kernel.Bootstraps
{
	// Token: 0x0200011B RID: 283
	[BootstrapConfig("china_emul")]
	internal class BootstrapChinaEmul : MsBootstrapScanner
	{
		// Token: 0x0600047F RID: 1151 RVA: 0x00013BED File Offset: 0x00011FED
		public override void Configure(BootstrapFilter filter)
		{
			base.Configure(filter);
			filter.EnableService<XmppProfanityCheckService>();
			filter.EnableService<EmptyUserIdValidator>();
			filter.EnableService<TrunkNicknameProvider>();
			filter.EnableConfigProfiles(MsConfigInfo.CustomRulesConfiguration, "live_season");
		}

		// Token: 0x040001FA RID: 506
		public const string Name = "china_emul";
	}
}
