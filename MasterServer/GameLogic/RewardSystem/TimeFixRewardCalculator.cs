using System;
using HK2Net;
using MasterServer.Core.Services.Configuration;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x02000426 RID: 1062
	[Service]
	internal class TimeFixRewardCalculator : IRewardCalculatorElement
	{
		// Token: 0x060016CF RID: 5839 RVA: 0x0005FA00 File Offset: 0x0005DE00
		public TimeFixRewardCalculator(IConfigurationService configurationService)
		{
			this.m_configurationService = configurationService;
		}

		// Token: 0x060016D0 RID: 5840 RVA: 0x0005FA10 File Offset: 0x0005DE10
		public void Calculate(MissionContext context, RewardInputData inputData, RewardInputData.Team.Player player, string sessionId, ref uint reward, ref RewardOutputData outputData)
		{
			if (context.timeDependencyInfo.IsValid())
			{
				if (inputData.sessionTime <= context.timeDependencyInfo.min)
				{
					reward = RewardCalculatorHelper.GetMinReward(this.m_configurationService);
				}
				else if (inputData.sessionTime > context.timeDependencyInfo.min && inputData.sessionTime < context.timeDependencyInfo.full)
				{
					reward = (uint)(0.5 + (double)(reward * (inputData.sessionTime - context.timeDependencyInfo.min)) / (context.timeDependencyInfo.full - context.timeDependencyInfo.min));
				}
			}
		}

		// Token: 0x04000B05 RID: 2821
		private readonly IConfigurationService m_configurationService;
	}
}
