using System;
using HK2Net;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020004B6 RID: 1206
	[Contract]
	internal interface IGameRoomActivator
	{
		// Token: 0x060019B7 RID: 6583
		IGameRoom OpenRoom(GameRoomType type, Action<IGameRoom> setup);

		// Token: 0x060019B8 RID: 6584
		IGameRoom OpenRoomByRoomRef(RoomReference roomRef, CreateRoomParam roomParam);
	}
}
