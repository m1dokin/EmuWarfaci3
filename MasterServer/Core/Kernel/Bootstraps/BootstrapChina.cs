using System;
using MasterServer.Core.Application.Bootstrap;
using MasterServer.Core.Kernel.Scanners;
using MasterServer.MySqlQueries;
using MasterServer.Platform.Nickname;
using MasterServer.Platform.ProfanityCheck;

namespace MasterServer.Core.Kernel.Bootstraps
{
	// Token: 0x0200011A RID: 282
	[BootstrapConfig("china")]
	internal class BootstrapChina : MsBootstrapScanner
	{
		// Token: 0x0600047D RID: 1149 RVA: 0x00013BBA File Offset: 0x00011FBA
		public override void Configure(BootstrapFilter filter)
		{
			base.Configure(filter);
			filter.EnableService<XmppProfanityCheckService>();
			filter.EnableService<UserIdValidator>();
			filter.EnableService<TrunkNicknameProvider>();
			filter.EnableConfigProfiles(MsConfigInfo.CustomRulesConfiguration, "live_season");
		}

		// Token: 0x040001F9 RID: 505
		public const string Name = "china";
	}
}
