using System;
using MasterServer.Core.Application.Bootstrap;
using MasterServer.Core.Kernel.Scanners;
using MasterServer.MySqlQueries;
using MasterServer.Platform.Nickname;
using MasterServer.Platform.Payment;
using MasterServer.Platform.ProfanityCheck;
using MasterServer.Telemetry.Metrics;
using MasterServer.Users.AuthorizationTokens;

namespace MasterServer.Core.Kernel.Bootstraps
{
	// Token: 0x0200011C RID: 284
	[BootstrapConfig("trunk")]
	internal class BootstrapTrunk : MsBootstrapScanner
	{
		// Token: 0x06000481 RID: 1153 RVA: 0x00013C20 File Offset: 0x00012020
		public override void Configure(BootstrapFilter filter)
		{
			base.Configure(filter);
			filter.EnableService<EmulatorPaymentService>();
			filter.EnableService<PaymentMetricsTracker>();
			filter.EnableService<HttpWalletEmulatorService>();
			filter.EnableService<CryDirtyExternal>();
			filter.EnableService<CryDirtyProfanityCheckService>();
			filter.EnableService<TrunkNicknameProvider>();
			filter.EnableService<EmptyUserIdValidator>();
			filter.EnableService<AuthorizationTokenServiceStub>();
			filter.EnableConfigProfiles(MsConfigInfo.OnlineVariablesConfiguration, "external_payment");
			filter.EnableConfigProfiles(MsConfigInfo.OnlineVariablesConfiguration, "gameblocks");
			filter.EnableConfigProfiles(MsConfigInfo.CustomRulesConfiguration, "test_season");
		}

		// Token: 0x040001FB RID: 507
		public const string Name = "trunk";
	}
}
