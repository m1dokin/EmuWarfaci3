using System;
using MasterServer.Core.Application.Bootstrap;
using MasterServer.Core.Kernel.Scanners;
using MasterServer.MySqlQueries;
using MasterServer.Platform.Nickname;
using MasterServer.Platform.ProfanityCheck;
using MasterServer.Users.AuthorizationTokens;

namespace MasterServer.Core.Kernel.Bootstraps
{
	// Token: 0x02000115 RID: 277
	[BootstrapConfig("row_emul")]
	internal class BootstrapRowEmul : MsBootstrapScanner
	{
		// Token: 0x06000473 RID: 1139 RVA: 0x000139AB File Offset: 0x00011DAB
		public override void Configure(BootstrapFilter filter)
		{
			base.Configure(filter);
			filter.EnableService<HttpEmulatorProfanityCheckService>();
			filter.EnableService<EmptyUserIdValidator>();
			filter.EnableService<TrunkNicknameProvider>();
			filter.EnableService<AuthorizationTokenServiceStub>();
			filter.EnableConfigProfiles(MsConfigInfo.CustomRulesConfiguration, "live_season");
		}

		// Token: 0x040001F4 RID: 500
		public const string Name = "row_emul";
	}
}
