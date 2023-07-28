using System;
using HK2Net;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.RewardSystem.RewardCalculators.ClanPointCalculator
{
	// Token: 0x020000E2 RID: 226
	[Contract]
	internal interface IClanPointCalculatorConfigProvider
	{
		// Token: 0x060003B6 RID: 950
		float GetRoomTypeMultiplier(GameRoomType roomType);

		// Token: 0x060003B7 RID: 951
		float GetGroupSizeMultiplier(GameRoomType roomType, int groupSize);
	}
}
