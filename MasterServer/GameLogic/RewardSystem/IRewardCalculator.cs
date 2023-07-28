using System;
using System.Collections.Generic;
using HK2Net;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020007B9 RID: 1977
	[Contract]
	internal interface IRewardCalculator
	{
		// Token: 0x06002893 RID: 10387
		IEnumerable<RewardOutputData> Calculate(RewardInputData inputData, string sessionId);
	}
}
