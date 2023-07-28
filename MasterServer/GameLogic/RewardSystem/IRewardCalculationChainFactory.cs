using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020005B3 RID: 1459
	[Contract]
	internal interface IRewardCalculationChainFactory
	{
		// Token: 0x06001F54 RID: 8020
		List<IRewardCalculatorElement> CreateRewardCalculationChain(GameRoomType roomType, MissionType missionType);
	}
}
