using System;
using HK2Net;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200044E RID: 1102
	[Contract]
	internal interface IAutoTeamBalanceLogicFactory
	{
		// Token: 0x06001765 RID: 5989
		IAutoTeamBalanceLogic GetTeamBalancer(GameRoomType roomType);
	}
}
