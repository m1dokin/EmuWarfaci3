using System;

namespace MasterServer.GameRoom.RoomExtensions.Vote
{
	// Token: 0x020004E3 RID: 1251
	internal class VoteConfig
	{
		// Token: 0x170002B5 RID: 693
		// (get) Token: 0x06001AF4 RID: 6900 RVA: 0x0006E7B8 File Offset: 0x0006CBB8
		// (set) Token: 0x06001AF5 RID: 6901 RVA: 0x0006E7C0 File Offset: 0x0006CBC0
		public float Threshold { get; set; }

		// Token: 0x170002B6 RID: 694
		// (get) Token: 0x06001AF6 RID: 6902 RVA: 0x0006E7C9 File Offset: 0x0006CBC9
		// (set) Token: 0x06001AF7 RID: 6903 RVA: 0x0006E7D1 File Offset: 0x0006CBD1
		public TimeSpan Timeout { get; set; }

		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x06001AF8 RID: 6904 RVA: 0x0006E7DA File Offset: 0x0006CBDA
		// (set) Token: 0x06001AF9 RID: 6905 RVA: 0x0006E7E2 File Offset: 0x0006CBE2
		public TimeSpan Cooldown { get; set; }

		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x06001AFA RID: 6906 RVA: 0x0006E7EB File Offset: 0x0006CBEB
		// (set) Token: 0x06001AFB RID: 6907 RVA: 0x0006E7F3 File Offset: 0x0006CBF3
		public TimeSpan CanBeStartedAfter { get; set; }
	}
}
