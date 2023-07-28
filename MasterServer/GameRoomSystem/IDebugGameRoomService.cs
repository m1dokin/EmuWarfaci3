using System;
using HK2Net;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200081B RID: 2075
	[Contract]
	internal interface IDebugGameRoomService
	{
		// Token: 0x06002A9C RID: 10908
		void SetPlayerTeam(ulong roomId, ulong profileId, int team);

		// Token: 0x06002A9D RID: 10909
		void SetTeamForAllPalyers(ulong roomId, int team);
	}
}
