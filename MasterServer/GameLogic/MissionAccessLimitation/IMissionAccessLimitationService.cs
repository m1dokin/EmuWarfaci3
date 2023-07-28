using System;
using HK2Net;

namespace MasterServer.GameLogic.MissionAccessLimitation
{
	// Token: 0x0200039C RID: 924
	[Contract]
	internal interface IMissionAccessLimitationService
	{
		// Token: 0x06001483 RID: 5251
		bool CanJoinMission(ulong profileId, string missionType);
	}
}
