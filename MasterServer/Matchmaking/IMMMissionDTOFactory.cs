using System;
using HK2Net;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameRoomSystem;
using MasterServer.Matchmaking.Data;

namespace MasterServer.Matchmaking
{
	// Token: 0x02000513 RID: 1299
	[Contract]
	internal interface IMMMissionDTOFactory
	{
		// Token: 0x06001C31 RID: 7217
		MMMissionDTO Create(MissionContextBase mission, GameRoomType roomType);
	}
}
