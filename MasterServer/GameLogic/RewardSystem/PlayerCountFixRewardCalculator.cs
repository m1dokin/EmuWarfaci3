using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Configuration;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x02000421 RID: 1057
	[Service]
	internal class PlayerCountFixRewardCalculator : IRewardCalculatorElement
	{
		// Token: 0x060016C3 RID: 5827 RVA: 0x0005F5AB File Offset: 0x0005D9AB
		public PlayerCountFixRewardCalculator(IConfigurationService configurationService)
		{
			this.m_configurationService = configurationService;
		}

		// Token: 0x060016C4 RID: 5828 RVA: 0x0005F5BC File Offset: 0x0005D9BC
		public void Calculate(MissionContext context, RewardInputData inputData, RewardInputData.Team.Player player, string sessionId, ref uint reward, ref RewardOutputData outputData)
		{
			ConfigSection section = this.m_configurationService.GetConfig(MsConfigInfo.RewardsConfiguration).GetSection("Rewards");
			int num = inputData.teams.Values.Sum((RewardInputData.Team team) => team.playerScores.Count);
			if (section != null)
			{
				List<string> list = section.GetList("player_count_reward_mults");
				if (list != null && list.Count >= num)
				{
					float num2 = float.Parse(list[num - 1]);
					reward = (uint)(0.5 + (double)(reward * num2));
				}
			}
		}

		// Token: 0x04000AFF RID: 2815
		private readonly IConfigurationService m_configurationService;
	}
}
