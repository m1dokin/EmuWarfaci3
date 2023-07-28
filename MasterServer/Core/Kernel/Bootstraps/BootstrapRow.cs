using System;
using MasterServer.Core.Application.Bootstrap;
using MasterServer.Core.Kernel.Scanners;
using MasterServer.MySqlQueries;
using MasterServer.Platform.Nickname;
using MasterServer.Platform.ProfanityCheck;
using MasterServer.Users.AuthorizationTokens;

namespace MasterServer.Core.Kernel.Bootstraps
{
	// Token: 0x02000114 RID: 276
	[BootstrapConfig("row")]
	internal class BootstrapRow : MsBootstrapScanner
	{
		// Token: 0x06000471 RID: 1137 RVA: 0x00013972 File Offset: 0x00011D72
		public override void Configure(BootstrapFilter filter)
		{
			base.Configure(filter);
			filter.EnableService<HttpProfanityCheckService>();
			filter.EnableService<UserIdValidator>();
			filter.EnableService<TrunkNicknameProvider>();
			filter.EnableService<AuthorizationTokenServiceStub>();
			filter.EnableConfigProfiles(MsConfigInfo.CustomRulesConfiguration, "live_season");
		}

		// Token: 0x040001F3 RID: 499
		public const string Name = "row";
	}
}
