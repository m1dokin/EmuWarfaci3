using System;
using System.Runtime.InteropServices;

namespace MasterServer.GameLogic.CustomRules.Rules.RatingSeason
{
	// Token: 0x02000097 RID: 151
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct RatingSeasonRuleConfig
	{
		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000249 RID: 585 RVA: 0x0000C3D7 File Offset: 0x0000A7D7
		// (set) Token: 0x0600024A RID: 586 RVA: 0x0000C3DF File Offset: 0x0000A7DF
		public string SeasonIdTemplate { get; set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x0600024B RID: 587 RVA: 0x0000C3E8 File Offset: 0x0000A7E8
		// (set) Token: 0x0600024C RID: 588 RVA: 0x0000C3F0 File Offset: 0x0000A7F0
		public DateTime AnnouncementEndDate { get; set; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x0600024D RID: 589 RVA: 0x0000C3F9 File Offset: 0x0000A7F9
		// (set) Token: 0x0600024E RID: 590 RVA: 0x0000C401 File Offset: 0x0000A801
		public DateTime GamesEndDate { get; set; }
	}
}
