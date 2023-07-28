using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Configuration;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020000E8 RID: 232
	[Service]
	internal class RoundLimitFixRewardCalculator : IRewardCalculatorElement
	{
		// Token: 0x060003D0 RID: 976 RVA: 0x00010B70 File Offset: 0x0000EF70
		public RoundLimitFixRewardCalculator(IConfigurationService configurationService)
		{
			this.m_configurationService = configurationService;
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x00010B80 File Offset: 0x0000EF80
		public void Calculate(MissionContext context, RewardInputData inputData, RewardInputData.Team.Player player, string sessionId, ref uint reward, ref RewardOutputData outputData)
		{
			ConfigSection section = this.m_configurationService.GetConfig(MsConfigInfo.RewardsConfiguration).GetSection("Rewards");
			List<ConfigSection> sections = section.GetSection("round_limit_reward_mults").GetSections("RoundMultiplier");
			foreach (ConfigSection configSection in sections)
			{
				int num = int.Parse(configSection.Get("round_limit"));
				if (num == inputData.maxRoundLimit)
				{
					float num2 = float.Parse(configSection.Get("value"));
					reward = (uint)(0.5f + reward * num2);
					break;
				}
			}
		}

		// Token: 0x04000199 RID: 409
		private readonly IConfigurationService m_configurationService;
	}
}
