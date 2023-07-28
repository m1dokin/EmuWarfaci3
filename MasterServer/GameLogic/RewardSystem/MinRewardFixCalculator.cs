using System;
using HK2Net;
using MasterServer.Core.Services.Configuration;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x02000420 RID: 1056
	[Service]
	internal class MinRewardFixCalculator : IRewardCalculatorElement
	{
		// Token: 0x060016C1 RID: 5825 RVA: 0x0005F572 File Offset: 0x0005D972
		public MinRewardFixCalculator(IConfigurationService configurationService)
		{
			this.m_configurationService = configurationService;
		}

		// Token: 0x060016C2 RID: 5826 RVA: 0x0005F581 File Offset: 0x0005D981
		public void Calculate(MissionContext context, RewardInputData inputData, RewardInputData.Team.Player player, string sessionId, ref uint reward, ref RewardOutputData outputData)
		{
			reward = Math.Max(RewardCalculatorHelper.GetMinReward(this.m_configurationService), (player.score > 0) ? reward : 0U);
		}

		// Token: 0x04000AFE RID: 2814
		private readonly IConfigurationService m_configurationService;
	}
}
