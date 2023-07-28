using System;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameRoom.RoomExtensions
{
	// Token: 0x020004C6 RID: 1222
	[RoomState(new Type[]
	{
		typeof(RegionRoomMasterExtension),
		typeof(RegionAutostartExtension)
	})]
	internal class RegionState : RoomStateBase
	{
		// Token: 0x06001A7C RID: 6780 RVA: 0x0006CDAB File Offset: 0x0006B1AB
		public RegionState()
		{
			this.RegionId = "global";
		}

		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x06001A7D RID: 6781 RVA: 0x0006CDBE File Offset: 0x0006B1BE
		// (set) Token: 0x06001A7E RID: 6782 RVA: 0x0006CDC6 File Offset: 0x0006B1C6
		public string RegionId { get; set; }
	}
}
