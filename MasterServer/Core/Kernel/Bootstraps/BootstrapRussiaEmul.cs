using System;
using MasterServer.Core.Application.Bootstrap;
using MasterServer.Core.Kernel.Scanners;
using MasterServer.MySqlQueries;
using MasterServer.Platform.Nickname;
using MasterServer.Platform.ProfanityCheck;
using MasterServer.Users.AuthorizationTokens;

namespace MasterServer.Core.Kernel.Bootstraps
{
	// Token: 0x02000119 RID: 281
	[BootstrapConfig("russia_emul")]
	internal class BootstrapRussiaEmul : MsBootstrapScanner
	{
		// Token: 0x0600047B RID: 1147 RVA: 0x00013B81 File Offset: 0x00011F81
		public override void Configure(BootstrapFilter filter)
		{
			base.Configure(filter);
			filter.EnableService<HttpEmulatorProfanityCheckService>();
			filter.EnableService<EmptyUserIdValidator>();
			filter.EnableService<TrunkNicknameProvider>();
			filter.EnableService<AuthorizationTokenService>();
			filter.EnableConfigProfiles(MsConfigInfo.CustomRulesConfiguration, "live_season");
		}

		// Token: 0x040001F8 RID: 504
		public const string Name = "russia_emul";
	}
}
