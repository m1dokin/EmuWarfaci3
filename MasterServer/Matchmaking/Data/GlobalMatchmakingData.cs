using System;
using System.Collections.Generic;

namespace MasterServer.Matchmaking.Data
{
	// Token: 0x020004F3 RID: 1267
	internal class GlobalMatchmakingData
	{
		// Token: 0x170002BB RID: 699
		// (get) Token: 0x06001B31 RID: 6961 RVA: 0x0006F021 File Offset: 0x0006D421
		// (set) Token: 0x06001B32 RID: 6962 RVA: 0x0006F029 File Offset: 0x0006D429
		public string ReplyTo { get; set; }

		// Token: 0x170002BC RID: 700
		// (get) Token: 0x06001B33 RID: 6963 RVA: 0x0006F032 File Offset: 0x0006D432
		// (set) Token: 0x06001B34 RID: 6964 RVA: 0x0006F03A File Offset: 0x0006D43A
		public double Load { get; set; }

		// Token: 0x170002BD RID: 701
		// (get) Token: 0x06001B35 RID: 6965 RVA: 0x0006F043 File Offset: 0x0006D443
		// (set) Token: 0x06001B36 RID: 6966 RVA: 0x0006F04B File Offset: 0x0006D44B
		public IEnumerable<MMEntityDTO> Entities { get; set; }

		// Token: 0x170002BE RID: 702
		// (get) Token: 0x06001B37 RID: 6967 RVA: 0x0006F054 File Offset: 0x0006D454
		// (set) Token: 0x06001B38 RID: 6968 RVA: 0x0006F05C File Offset: 0x0006D45C
		public IEnumerable<MMRoomDTO> Rooms { get; set; }

		// Token: 0x170002BF RID: 703
		// (get) Token: 0x06001B39 RID: 6969 RVA: 0x0006F065 File Offset: 0x0006D465
		// (set) Token: 0x06001B3A RID: 6970 RVA: 0x0006F06D File Offset: 0x0006D46D
		public IEnumerable<MMMissionDTO> Missions { get; set; }

		// Token: 0x170002C0 RID: 704
		// (get) Token: 0x06001B3B RID: 6971 RVA: 0x0006F076 File Offset: 0x0006D476
		// (set) Token: 0x06001B3C RID: 6972 RVA: 0x0006F07E File Offset: 0x0006D47E
		public IEnumerable<MMRegionDistanceDTO> RegionsDistances { get; set; }
	}
}
