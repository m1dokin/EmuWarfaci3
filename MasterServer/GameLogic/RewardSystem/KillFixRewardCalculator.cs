using System;
using HK2Net;
using MasterServer.Core.Services.Configuration;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x0200041F RID: 1055
	[Service]
	internal class KillFixRewardCalculator : IRewardCalculatorElement
	{
		// Token: 0x060016BF RID: 5823 RVA: 0x0005F4AF File Offset: 0x0005D8AF
		public KillFixRewardCalculator(IConfigurationService configurationService)
		{
			this.m_configurationService = configurationService;
		}

		// Token: 0x060016C0 RID: 5824 RVA: 0x0005F4C0 File Offset: 0x0005D8C0
		public void Calculate(MissionContext context, RewardInputData inputData, RewardInputData.Team.Player player, string sessionId, ref uint reward, ref RewardOutputData outputData)
		{
			if (context.killDependencyInfo.IsValid())
			{
				if (inputData.sessionKillCount <= context.killDependencyInfo.min)
				{
					reward = RewardCalculatorHelper.GetMinReward(this.m_configurationService);
				}
				else if (inputData.sessionKillCount > context.killDependencyInfo.min && inputData.sessionKillCount < context.killDependencyInfo.full)
				{
					reward = (uint)(0.5 + reward * (inputData.sessionKillCount - context.killDependencyInfo.min) / (context.killDependencyInfo.full - context.killDependencyInfo.min));
				}
			}
		}

		// Token: 0x04000AFD RID: 2813
		private readonly IConfigurationService m_configurationService;
	}
}
