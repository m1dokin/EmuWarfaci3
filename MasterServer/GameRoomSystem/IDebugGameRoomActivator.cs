using System;
using HK2Net;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameRoom.Commands.Debug;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020004B7 RID: 1207
	[Contract]
	internal interface IDebugGameRoomActivator
	{
		// Token: 0x060019B9 RID: 6585
		IGameRoom OpenRoomWithFakePlayers(GameRoomType roomType, string missionId, int startId, DebugRoomPlayersParams debugPlayerParams, IProfileProgressionService profileProgressionService);

		// Token: 0x060019BA RID: 6586
		void CloseRooms(params IGameRoom[] rooms);
	}
}
