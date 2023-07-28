using System;

namespace MasterServer.Matchmaking.Data
{
	// Token: 0x020004F7 RID: 1271
	internal class MMResultPlayerInfo
	{
		// Token: 0x170002CF RID: 719
		// (get) Token: 0x06001B62 RID: 7010 RVA: 0x0006F33C File Offset: 0x0006D73C
		// (set) Token: 0x06001B63 RID: 7011 RVA: 0x0006F344 File Offset: 0x0006D744
		public ulong ProfileId { get; set; }

		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x06001B64 RID: 7012 RVA: 0x0006F34D File Offset: 0x0006D74D
		// (set) Token: 0x06001B65 RID: 7013 RVA: 0x0006F355 File Offset: 0x0006D755
		public ulong UserId { get; set; }

		// Token: 0x170002D1 RID: 721
		// (get) Token: 0x06001B66 RID: 7014 RVA: 0x0006F35E File Offset: 0x0006D75E
		// (set) Token: 0x06001B67 RID: 7015 RVA: 0x0006F366 File Offset: 0x0006D766
		public string Nickname { get; set; }

		// Token: 0x170002D2 RID: 722
		// (get) Token: 0x06001B68 RID: 7016 RVA: 0x0006F36F File Offset: 0x0006D76F
		// (set) Token: 0x06001B69 RID: 7017 RVA: 0x0006F377 File Offset: 0x0006D777
		public string OnlineId { get; set; }
	}
}
