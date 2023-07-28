using System;
using HK2Net;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200081A RID: 2074
	[Contract]
	internal interface IGameRoomManagerRegistry
	{
		// Token: 0x06002A97 RID: 10903
		bool ContainsRoom(RoomReference roomRef);

		// Token: 0x06002A98 RID: 10904
		bool RegisterRoom(IGameRoom room);

		// Token: 0x06002A99 RID: 10905
		void UnregisterRoom(IGameRoom room);

		// Token: 0x06002A9A RID: 10906
		void RegisterPlayer(IGameRoom room, ulong profileId);

		// Token: 0x06002A9B RID: 10907
		void UnregisterPlayer(IGameRoom room, ulong profileId);
	}
}
