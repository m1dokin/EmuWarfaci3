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
	// Token: 0x02000117 RID: 279
	[BootstrapConfig("west_emul")]
	internal class BootstrapWestEmul : MsBootstrapScanner
	{
		// Token: 0x06000477 RID: 1143 RVA: 0x00013A5C File Offset: 0x00011E5C
		public override void Configure(BootstrapFilter filter)
		{
			base.Configure(filter);
			filter.EnableService<GFaceNicknameService>();
			filter.EnableService<GFacePaymentService>();
			filter.EnableService<GFaceProxyService>();
			filter.EnableService<PaymentMetricsTracker>();
			filter.EnableService<CryDirtyExternal>();
			filter.EnableService<CryDirtyProfanityCheckService>();
			filter.EnableService<EmptyUserIdValidator>();
			filter.EnableService<AuthorizationTokenServiceStub>();
			filter.EnableConfigProfiles(MsConfigInfo.OnlineVariablesConfiguration, "external_payment");
			filter.EnableConfigProfiles(MsConfigInfo.OnlineVariablesConfiguration, "web_charge_account");
			filter.EnableConfigProfiles(MsConfigInfo.OnlineVariablesConfiguration, "gameblocks");
			filter.EnableConfigProfiles(MsConfigInfo.OnlineVariablesConfiguration, "external_payment");
			filter.EnableConfigProfiles(MsConfigInfo.OnlineVariablesConfiguration, "web_charge_account");
			filter.EnableConfigProfiles(MsConfigInfo.OnlineVariablesConfiguration, "gameblocks");
			filter.EnableConfigProfiles(MsConfigInfo.CustomRulesConfiguration, "live_season");
			filter.EnableConfigProfiles(MsConfigInfo.OnlineVariablesConfiguration, "external_payment");
			filter.EnableConfigProfiles(MsConfigInfo.OnlineVariablesConfiguration, "web_charge_account");
			filter.EnableConfigProfiles(MsConfigInfo.OnlineVariablesConfiguration, "gameblocks");
		}

		// Token: 0x040001F6 RID: 502
		public const string Name = "west_emul";
	}
}
