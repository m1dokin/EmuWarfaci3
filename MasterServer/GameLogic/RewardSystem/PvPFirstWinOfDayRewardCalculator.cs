using System;
using HK2Net;
using MasterServer.Core.Configs;
using MasterServer.Core.Services.Configuration;
using MasterServer.GameLogic.FirstWinOfDayByModeSystem;
using MasterServer.GameLogic.FirstWinOfDayByModeSystem.Configs.GameModeFirstWinOfDayBonusConfig;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020000E7 RID: 231
	[Service]
	internal class PvPFirstWinOfDayRewardCalculator : IRewardCalculatorElement
	{
		// Token: 0x060003CC RID: 972 RVA: 0x00010A35 File Offset: 0x0000EE35
		public PvPFirstWinOfDayRewardCalculator(IFirstWinOfDayByModeService firstWinOfDayByModeService, IConfigurationService configurationService, IConfigProvider<GameModeFirstWinOfDayBonusConfig> gameModeFirstWinOfDayBonusConfigProvider)
		{
			this.m_firstWinOfDayByModeService = firstWinOfDayByModeService;
			this.m_gameModeFirstWinOfDayBonusConfigProvider = gameModeFirstWinOfDayBonusConfigProvider;
			this.m_configurationService = configurationService;
		}

		// Token: 0x060003CD RID: 973 RVA: 0x00010A54 File Offset: 0x0000EE54
		public void Calculate(MissionContext context, RewardInputData inputData, RewardInputData.Team.Player player, string sessionId, ref uint reward, ref RewardOutputData outputData)
		{
			if (this.IsWinner(inputData, player) || this.IsFreeForAllMode(player))
			{
				GameModeFirstWinOfDayBonusConfig gameModeFirstWinOfDayBonusConfig = this.m_gameModeFirstWinOfDayBonusConfigProvider.Get();
				GameModeBonus gameModeBonus;
				if (gameModeFirstWinOfDayBonusConfig.ModesBonus.TryGetValue(context.gameMode, out gameModeBonus))
				{
					bool flag = this.m_firstWinOfDayByModeService.SetPvpModeFirstWin(player.profileId, context.gameMode);
					if (flag)
					{
						reward += gameModeBonus.Bonus;
						SRewardMultiplier missionTypeMultiplier = ApplyMultipliersCalculator.GetMissionTypeMultiplier(this.m_configurationService, context.missionType.Name);
						outputData.bonusExp = (uint)(0.5 + (double)(gameModeBonus.Bonus * missionTypeMultiplier.ExperienceMultiplier));
						outputData.bonusMoney = (uint)(0.5 + (double)(gameModeBonus.Bonus * missionTypeMultiplier.MoneyMultiplier));
						outputData.bonusSponsorPoints = (uint)(0.5 + (double)(gameModeBonus.Bonus * missionTypeMultiplier.SponsorPointsMultiplier));
						outputData.firstWin = true;
					}
				}
			}
		}

		// Token: 0x060003CE RID: 974 RVA: 0x00010B55 File Offset: 0x0000EF55
		private bool IsWinner(RewardInputData inputData, RewardInputData.Team.Player player)
		{
			return inputData.winnerTeamId == (int)player.teamId;
		}

		// Token: 0x060003CF RID: 975 RVA: 0x00010B65 File Offset: 0x0000EF65
		private bool IsFreeForAllMode(RewardInputData.Team.Player player)
		{
			return player.teamId == 0;
		}

		// Token: 0x04000196 RID: 406
		private readonly IFirstWinOfDayByModeService m_firstWinOfDayByModeService;

		// Token: 0x04000197 RID: 407
		private readonly IConfigurationService m_configurationService;

		// Token: 0x04000198 RID: 408
		private readonly IConfigProvider<GameModeFirstWinOfDayBonusConfig> m_gameModeFirstWinOfDayBonusConfigProvider;
	}
}
