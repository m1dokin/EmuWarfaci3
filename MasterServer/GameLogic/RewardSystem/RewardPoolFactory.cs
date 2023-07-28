using System;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020005B7 RID: 1463
	internal class RewardPoolFactory
	{
		// Token: 0x06001F6D RID: 8045 RVA: 0x0007FC70 File Offset: 0x0007E070
		public static IRewardPool CreateRewardPool(MissionType missionType)
		{
			if (missionType.IsSurvival())
			{
				return new SurvivalRewardPool();
			}
			return new RewardPool();
		}
	}
}
