using System;

namespace MasterServer.GameRoom.RoomExtensions.Reconnect
{
	// Token: 0x020004CA RID: 1226
	internal enum ReconnectResult
	{
		// Token: 0x04000CB1 RID: 3249
		Success,
		// Token: 0x04000CB2 RID: 3250
		OtherTeam,
		// Token: 0x04000CB3 RID: 3251
		NoRoom,
		// Token: 0x04000CB4 RID: 3252
		NoFreeSlots,
		// Token: 0x04000CB5 RID: 3253
		InvalidReconnectInfo,
		// Token: 0x04000CB6 RID: 3254
		ReconnectInfoExpired
	}
}
