using System;
using HK2Net;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x0200041E RID: 1054
	[Service]
	internal class CrownRewardCalculator : IRewardCalculatorElement
	{
		// Token: 0x060016BD RID: 5821 RVA: 0x0005F483 File Offset: 0x0005D883
		public CrownRewardCalculator(ICrownRewardService crownRewardService)
		{
			this.m_crownRewardService = crownRewardService;
		}

		// Token: 0x060016BE RID: 5822 RVA: 0x0005F492 File Offset: 0x0005D892
		public void Calculate(MissionContext context, RewardInputData inputData, RewardInputData.Team.Player player, string sessionId, ref uint reward, ref RewardOutputData outputData)
		{
			outputData.crownReward = this.m_crownRewardService.CalculateCrownReward(player.profileId, inputData, sessionId);
		}

		// Token: 0x04000AFC RID: 2812
		private readonly ICrownRewardService m_crownRewardService;
	}
}
