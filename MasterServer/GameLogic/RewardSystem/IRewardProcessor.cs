using System;
using HK2Net;
using MasterServer.Core;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020005BB RID: 1467
	[Contract]
	internal interface IRewardProcessor
	{
		// Token: 0x06001F79 RID: 8057
		RewardOutputData ProcessRewardData(ulong userId, RewardProcessorState state, MissionContext missionContext, RewardOutputData aggRewardData, ILogGroup logGroup);
	}
}
