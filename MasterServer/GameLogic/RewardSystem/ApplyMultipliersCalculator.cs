using System;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Configuration;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x0200041B RID: 1051
	[Service]
	internal class ApplyMultipliersCalculator : IRewardCalculatorElement
	{
		// Token: 0x060016A8 RID: 5800 RVA: 0x0005ECC6 File Offset: 0x0005D0C6
		public ApplyMultipliersCalculator(IConfigurationService configurationService)
		{
			this.m_configurationService = configurationService;
			ApplyMultipliersCalculator.ValidateMultipliers(this.m_configurationService);
		}

		// Token: 0x060016A9 RID: 5801 RVA: 0x0005ECE0 File Offset: 0x0005D0E0
		public void Calculate(MissionContext context, RewardInputData inputData, RewardInputData.Team.Player player, string sessionId, ref uint reward, ref RewardOutputData outputData)
		{
			SRewardMultiplier missionTypeMultiplier = ApplyMultipliersCalculator.GetMissionTypeMultiplier(this.m_configurationService, context.missionType.Name);
			outputData.gainedExp = (uint)(0.5 + (double)(reward * missionTypeMultiplier.ExperienceMultiplier));
			outputData.gainedMoney = (uint)(0.5 + (double)(reward * missionTypeMultiplier.MoneyMultiplier));
			outputData.gainedSponsorPoints = (uint)(0.5 + (double)(reward * missionTypeMultiplier.SponsorPointsMultiplier));
		}

		// Token: 0x060016AA RID: 5802 RVA: 0x0005ED64 File Offset: 0x0005D164
		public static SRewardMultiplier GetMissionTypeMultiplier(IConfigurationService configurationService, string missionType)
		{
			return new SRewardMultiplier
			{
				ExperienceMultiplier = ApplyMultipliersCalculator.ParseRewardMultiplier(configurationService, "ExperienceMultiplier", missionType),
				MoneyMultiplier = ApplyMultipliersCalculator.ParseRewardMultiplier(configurationService, "MoneyMultiplier", missionType),
				SponsorPointsMultiplier = ApplyMultipliersCalculator.ParseRewardMultiplier(configurationService, "SponsorPointsMultiplier", missionType)
			};
		}

		// Token: 0x060016AB RID: 5803 RVA: 0x0005EDB4 File Offset: 0x0005D1B4
		private static float ParseRewardMultiplier(IConfigurationService configurationService, string kind, string missionType)
		{
			Config config = configurationService.GetConfig(MsConfigInfo.RewardsConfiguration);
			ConfigSection section = config.GetSection("Rewards");
			ConfigSection section2 = section.GetSection(kind);
			float result;
			if (section2.HasValue(missionType))
			{
				section2.Get(missionType, out result);
			}
			else
			{
				section2.Get("default", out result);
			}
			return result;
		}

		// Token: 0x060016AC RID: 5804 RVA: 0x0005EE09 File Offset: 0x0005D209
		public static void ValidateMultipliers(IConfigurationService configurationService)
		{
			ApplyMultipliersCalculator.ParseRewardMultiplier(configurationService, "ExperienceMultiplier", "default");
			ApplyMultipliersCalculator.ParseRewardMultiplier(configurationService, "MoneyMultiplier", "default");
			ApplyMultipliersCalculator.ParseRewardMultiplier(configurationService, "SponsorPointsMultiplier", "default");
		}

		// Token: 0x04000AF4 RID: 2804
		private readonly IConfigurationService m_configurationService;
	}
}
