using System;
using HK2Net;
using MasterServer.Users;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020004EE RID: 1262
	[Contract]
	internal interface IRoomPlayerFactory
	{
		// Token: 0x06001B26 RID: 6950
		RoomPlayer GetRoomPlayer(UserInfo.User user, GameRoomType roomType);
	}
}
