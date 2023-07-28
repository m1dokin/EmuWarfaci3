using System;
using MasterServer.Core.Application.Bootstrap;
using MasterServer.Core.Kernel.Scanners;
using MasterServer.MySqlQueries;
using MasterServer.Platform.Nickname;
using MasterServer.Platform.ProfanityCheck;
using MasterServer.Users.AuthorizationTokens;

namespace MasterServer.Core.Kernel.Bootstraps
{
	// Token: 0x02000118 RID: 280
	[BootstrapConfig("russia")]
	internal class BootstrapRussia : MsBootstrapScanner
	{
		// Token: 0x06000479 RID: 1145 RVA: 0x00013B48 File Offset: 0x00011F48
		public override void Configure(BootstrapFilter filter)
		{
			base.Configure(filter);
			filter.EnableService<HttpProfanityCheckService>();
			filter.EnableService<UserIdValidator>();
			filter.EnableService<TrunkNicknameProvider>();
			filter.EnableService<AuthorizationTokenService>();
			filter.EnableConfigProfiles(MsConfigInfo.CustomRulesConfiguration, "live_season");
		}

		// Token: 0x040001F7 RID: 503
		public const string Name = "russia";
	}
}
