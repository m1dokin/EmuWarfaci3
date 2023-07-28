using System;
using MasterServer.GameRoomSystem;

namespace MasterServer.Matchmaking.Data
{
	// Token: 0x02000596 RID: 1430
	internal class MMSettings
	{
		// Token: 0x17000327 RID: 807
		// (get) Token: 0x06001EC8 RID: 7880 RVA: 0x0007D1E8 File Offset: 0x0007B5E8
		// (set) Token: 0x06001EC9 RID: 7881 RVA: 0x0007D1F0 File Offset: 0x0007B5F0
		public GameRoomType RoomType { get; set; }

		// Token: 0x17000328 RID: 808
		// (get) Token: 0x06001ECA RID: 7882 RVA: 0x0007D1F9 File Offset: 0x0007B5F9
		// (set) Token: 0x06001ECB RID: 7883 RVA: 0x0007D201 File Offset: 0x0007B601
		public string RoomName { get; set; }

		// Token: 0x17000329 RID: 809
		// (get) Token: 0x06001ECC RID: 7884 RVA: 0x0007D20A File Offset: 0x0007B60A
		// (set) Token: 0x06001ECD RID: 7885 RVA: 0x0007D212 File Offset: 0x0007B612
		public string MissionId { get; set; }

		// Token: 0x1700032A RID: 810
		// (get) Token: 0x06001ECE RID: 7886 RVA: 0x0007D21B File Offset: 0x0007B61B
		// (set) Token: 0x06001ECF RID: 7887 RVA: 0x0007D223 File Offset: 0x0007B623
		public string GameMode { get; set; }

		// Token: 0x1700032B RID: 811
		// (get) Token: 0x06001ED0 RID: 7888 RVA: 0x0007D22C File Offset: 0x0007B62C
		// (set) Token: 0x06001ED1 RID: 7889 RVA: 0x0007D234 File Offset: 0x0007B634
		public string MissionType { get; set; }

		// Token: 0x1700032C RID: 812
		// (get) Token: 0x06001ED2 RID: 7890 RVA: 0x0007D23D File Offset: 0x0007B63D
		// (set) Token: 0x06001ED3 RID: 7891 RVA: 0x0007D245 File Offset: 0x0007B645
		public int TeamId { get; set; }

		// Token: 0x1700032D RID: 813
		// (get) Token: 0x06001ED4 RID: 7892 RVA: 0x0007D24E File Offset: 0x0007B64E
		// (set) Token: 0x06001ED5 RID: 7893 RVA: 0x0007D256 File Offset: 0x0007B656
		public int ClassId { get; set; }

		// Token: 0x1700032E RID: 814
		// (get) Token: 0x06001ED6 RID: 7894 RVA: 0x0007D25F File Offset: 0x0007B65F
		// (set) Token: 0x06001ED7 RID: 7895 RVA: 0x0007D267 File Offset: 0x0007B667
		public DateTime StartTimeUtc { get; set; }
	}
}
