using System;
using System.Collections.Generic;
using HK2Net;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020004A0 RID: 1184
	[Contract]
	internal interface IAutoTeamBalanceLogic
	{
		// Token: 0x06001934 RID: 6452
		bool CanBalancePlayers(IGameRoom room, IEnumerable<RoomPlayer> players);

		// Token: 0x06001935 RID: 6453
		Dictionary<int, TeamInfo> BalancePlayers(IGameRoom room, IEnumerable<RoomPlayer> players);

		// Token: 0x06001936 RID: 6454
		int ChooseTeam(RoomPlayer player, IGameRoom room, IEnumerable<RoomPlayer> players);
	}
}
