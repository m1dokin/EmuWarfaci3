using System;
using MasterServer.Core.Application.Bootstrap;
using MasterServer.Core.Kernel.Scanners;
using MasterServer.GFace.Services;
using MasterServer.MySqlQueries;
using MasterServer.Platform.ProfanityCheck;
using MasterServer.Telemetry.Metrics;
using MasterServer.Users.AuthorizationTokens;

namespace MasterServer.Core.Kernel.Bootstraps
{
	// Token: 0x02000116 RID: 278
	[BootstrapConfig("west")]
	internal class BootstrapWest : MsBootstrapScanner
	{
		// Token: 0x06000475 RID: 1141 RVA: 0x000139E4 File Offset: 0x00011DE4
		public override void Configure(BootstrapFilter filter)
		{
			base.Configure(filter);
			filter.EnableService<GFaceNicknameService>();
			filter.EnableService<GFacePaymentService>();
			filter.EnableService<PaymentMetricsTracker>();
			filter.EnableService<CryDirtyExternal>();
			filter.EnableService<CryDirtyProfanityCheckService>();
			filter.EnableService<UserIdValidator>();
			filter.EnableService<AuthorizationTokenServiceStub>();
			filter.EnableConfigProfiles(MsConfigInfo.OnlineVariablesConfiguration, "external_payment");
			filter.EnableConfigProfiles(MsConfigInfo.OnlineVariablesConfiguration, "gameblocks");
			filter.EnableConfigProfiles(MsConfigInfo.CustomRulesConfiguration, "live_season");
		}

		// Token: 0x040001F5 RID: 501
		public const string Name = "west";
	}
}
