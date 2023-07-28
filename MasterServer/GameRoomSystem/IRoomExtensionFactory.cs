using System;
using HK2Net;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020004BB RID: 1211
	[Contract]
	internal interface IRoomExtensionFactory
	{
		// Token: 0x06001A39 RID: 6713
		RoomExtensionsData GetRoomExtensions(GameRoomType type);
	}
}
