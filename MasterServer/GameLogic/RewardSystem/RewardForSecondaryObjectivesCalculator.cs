using System;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Configuration;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x02000424 RID: 1060
	[Service]
	internal class RewardForSecondaryObjectivesCalculator : IRewardCalculatorElement
	{
		// Token: 0x060016CB RID: 5835 RVA: 0x0005F88F File Offset: 0x0005DC8F
		public RewardForSecondaryObjectivesCalculator(IConfigurationService configurationService)
		{
			this.m_configurationService = configurationService;
		}

		// Token: 0x060016CC RID: 5836 RVA: 0x0005F8A0 File Offset: 0x0005DCA0
		public void Calculate(MissionContext context, RewardInputData inputData, RewardInputData.Team.Player player, string sessionId, ref uint reward, ref RewardOutputData outputData)
		{
			ConfigSection section = this.m_configurationService.GetConfig(MsConfigInfo.RewardsConfiguration).GetSection("Rewards");
			float num = float.Parse(section.Get("SecondaryObjectiveBonus"));
			float num2 = 1f + (float)inputData.secondaryObjectivesCompleted * num / 100f;
			reward = (uint)((double)(reward * num2) + 0.5);
		}

		// Token: 0x04000B04 RID: 2820
		private readonly IConfigurationService m_configurationService;
	}
}
