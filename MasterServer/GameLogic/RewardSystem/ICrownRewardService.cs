using System;
using HK2Net;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x0200059E RID: 1438
	[Contract]
	internal interface ICrownRewardService
	{
		// Token: 0x06001EF3 RID: 7923
		ProfileCrownReward CalculateCrownReward(ulong profileID, RewardInputData inputData, string sessionId);
	}
}
