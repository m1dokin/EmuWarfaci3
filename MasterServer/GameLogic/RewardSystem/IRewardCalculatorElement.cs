using System;
using HK2Net;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020005A9 RID: 1449
	[Contract]
	internal interface IRewardCalculatorElement
	{
		// Token: 0x06001F1A RID: 7962
		void Calculate(MissionContext context, RewardInputData inputData, RewardInputData.Team.Player player, string sessionId, ref uint reward, ref RewardOutputData outputData);
	}
}
