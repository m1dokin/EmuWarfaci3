using System;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameRoom.RoomExtensions.Reconnect
{
	// Token: 0x020004CC RID: 1228
	internal class ReconnectInfo
	{
		// Token: 0x06001A97 RID: 6807 RVA: 0x0006D2EC File Offset: 0x0006B6EC
		public ReconnectInfo(RoomPlayer player)
		{
			this.TeamId = player.TeamID;
			this.LeaveTime = DateTime.Now;
		}

		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x06001A98 RID: 6808 RVA: 0x0006D30B File Offset: 0x0006B70B
		// (set) Token: 0x06001A99 RID: 6809 RVA: 0x0006D313 File Offset: 0x0006B713
		public int TeamId { get; set; }

		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x06001A9A RID: 6810 RVA: 0x0006D31C File Offset: 0x0006B71C
		// (set) Token: 0x06001A9B RID: 6811 RVA: 0x0006D324 File Offset: 0x0006B724
		public DateTime LeaveTime { get; private set; }
	}
}
