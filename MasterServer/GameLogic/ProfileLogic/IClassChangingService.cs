using System;
using HK2Net;
using MasterServer.Users;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000545 RID: 1349
	[Contract]
	internal interface IClassChangingService
	{
		// Token: 0x06001D29 RID: 7465
		ClassChangeStatus ChangePlayersClass(UserInfo.User user, uint classId);
	}
}
