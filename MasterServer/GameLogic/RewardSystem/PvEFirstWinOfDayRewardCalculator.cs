using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Configuration;
using MasterServer.Database;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x02000423 RID: 1059
	[Service]
	internal class PvEFirstWinOfDayRewardCalculator : IRewardCalculatorElement
	{
		// Token: 0x060016C8 RID: 5832 RVA: 0x0005F6C0 File Offset: 0x0005DAC0
		public PvEFirstWinOfDayRewardCalculator(IDALService dalService, IConfigurationService configurationService)
		{
			this.m_dalService = dalService;
			this.m_configurationService = configurationService;
			this.m_rewardsPerMissionType = new Dictionary<string, uint>();
			Config config = this.m_configurationService.GetConfig(MsConfigInfo.RewardsConfiguration);
			ConfigSection section = config.GetSection("Rewards");
			List<ConfigSection> sections = section.GetSections("BonusRewardPool");
			foreach (ConfigSection configSection in sections)
			{
				this.m_rewardsPerMissionType[configSection.Get("mission_type")] = uint.Parse(configSection.Get("value"));
			}
			ApplyMultipliersCalculator.ValidateMultipliers(this.m_configurationService);
		}

		// Token: 0x060016C9 RID: 5833 RVA: 0x0005F78C File Offset: 0x0005DB8C
		public void Calculate(MissionContext context, RewardInputData inputData, RewardInputData.Team.Player player, string sessionId, ref uint reward, ref RewardOutputData outputData)
		{
			SessionOutcome outcome = RewardCalculatorHelper.GetOutcome(context, inputData.winnerTeamId, (int)player.teamId);
			if (outcome == SessionOutcome.Won)
			{
				bool flag = this.m_dalService.PerformanceSystem.SetMissionProfileWin(new Guid(inputData.missionId), player.profileId);
				if (flag)
				{
					uint bonusReward = this.GetBonusReward(context.missionType.Name);
					reward += bonusReward;
					SRewardMultiplier missionTypeMultiplier = ApplyMultipliersCalculator.GetMissionTypeMultiplier(this.m_configurationService, context.missionType.Name);
					outputData.bonusExp = (uint)(0.5 + (double)(bonusReward * missionTypeMultiplier.ExperienceMultiplier));
					outputData.bonusMoney = (uint)(0.5 + (double)(bonusReward * missionTypeMultiplier.MoneyMultiplier));
					outputData.bonusSponsorPoints = (uint)(0.5 + (double)(bonusReward * missionTypeMultiplier.SponsorPointsMultiplier));
					outputData.firstWin = true;
				}
			}
		}

		// Token: 0x060016CA RID: 5834 RVA: 0x0005F870 File Offset: 0x0005DC70
		private uint GetBonusReward(string missionType)
		{
			uint result = 0U;
			this.m_rewardsPerMissionType.TryGetValue(missionType, out result);
			return result;
		}

		// Token: 0x04000B01 RID: 2817
		private readonly IDALService m_dalService;

		// Token: 0x04000B02 RID: 2818
		private readonly IConfigurationService m_configurationService;

		// Token: 0x04000B03 RID: 2819
		private readonly Dictionary<string, uint> m_rewardsPerMissionType;
	}
}
