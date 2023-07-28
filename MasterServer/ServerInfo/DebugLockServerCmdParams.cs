using System;
using CommandLine;

namespace MasterServer.ServerInfo
{
	// Token: 0x020006BB RID: 1723
	internal class DebugLockServerCmdParams
	{
		// Token: 0x1700037A RID: 890
		// (get) Token: 0x0600241E RID: 9246 RVA: 0x000971AC File Offset: 0x000955AC
		// (set) Token: 0x0600241F RID: 9247 RVA: 0x000971B4 File Offset: 0x000955B4
		[Option('m', "mode", DefaultValue = "pvp", HelpText = "LDS mode (pvp - for pure PvP, pve - for PvP-PvE)")]
		public string Mode { get; set; }

		// Token: 0x1700037B RID: 891
		// (get) Token: 0x06002420 RID: 9248 RVA: 0x000971BD File Offset: 0x000955BD
		// (set) Token: 0x06002421 RID: 9249 RVA: 0x000971C5 File Offset: 0x000955C5
		[Option('t', "build_type", DefaultValue = "profile", HelpText = "LDS build type (profile/release)")]
		public string BuildType { get; set; }

		// Token: 0x1700037C RID: 892
		// (get) Token: 0x06002422 RID: 9250 RVA: 0x000971CE File Offset: 0x000955CE
		// (set) Token: 0x06002423 RID: 9251 RVA: 0x000971D6 File Offset: 0x000955D6
		[Option('r', "region_id", HelpText = "LDS region id", DefaultValue = "global", Required = false)]
		public string RegionId { get; set; }
	}
}
