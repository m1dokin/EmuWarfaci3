using System;
using MasterServer.Core;
using MasterServer.Core.Services.Configuration;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x02000422 RID: 1058
	internal static class RewardCalculatorHelper
	{
		// Token: 0x060016C6 RID: 5830 RVA: 0x0005F668 File Offset: 0x0005DA68
		public static uint GetMinReward(IConfigurationService configurationService)
		{
			return uint.Parse(configurationService.GetConfig(MsConfigInfo.RewardsConfiguration).GetSection("Rewards").Get("MinReward"));
		}

		// Token: 0x060016C7 RID: 5831 RVA: 0x0005F68E File Offset: 0x0005DA8E
		public static SessionOutcome GetOutcome(MissionContext missionContext, int winnerTeamId, int playerTeamId)
		{
			if (missionContext.IsPvPMode() && missionContext.noTeamsMode)
			{
				return SessionOutcome.Draw;
			}
			if (winnerTeamId == -1)
			{
				return SessionOutcome.Stopped;
			}
			return (winnerTeamId != playerTeamId) ? SessionOutcome.Lost : SessionOutcome.Won;
		}
	}
}
