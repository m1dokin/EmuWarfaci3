using System;
using System.Runtime.InteropServices;

namespace MasterServer.Matchmaking.Data
{
	// Token: 0x02000503 RID: 1283
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct MatchmakingConfig
	{
		// Token: 0x170002D9 RID: 729
		// (get) Token: 0x06001BB7 RID: 7095 RVA: 0x000706C4 File Offset: 0x0006EAC4
		// (set) Token: 0x06001BB8 RID: 7096 RVA: 0x000706CC File Offset: 0x0006EACC
		public TimeSpan QueueInterval { get; set; }

		// Token: 0x170002DA RID: 730
		// (get) Token: 0x06001BB9 RID: 7097 RVA: 0x000706D5 File Offset: 0x0006EAD5
		// (set) Token: 0x06001BBA RID: 7098 RVA: 0x000706DD File Offset: 0x0006EADD
		public TimeSpan TimeToMapsResetNotification { get; set; }

		// Token: 0x170002DB RID: 731
		// (get) Token: 0x06001BBB RID: 7099 RVA: 0x000706E6 File Offset: 0x0006EAE6
		// (set) Token: 0x06001BBC RID: 7100 RVA: 0x000706EE File Offset: 0x0006EAEE
		public bool IsAutostartEnabled { get; set; }

		// Token: 0x170002DC RID: 732
		// (get) Token: 0x06001BBD RID: 7101 RVA: 0x000706F7 File Offset: 0x0006EAF7
		// (set) Token: 0x06001BBE RID: 7102 RVA: 0x000706FF File Offset: 0x0006EAFF
		public bool IsPveAutostartEnabled { get; set; }
	}
}
