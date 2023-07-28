using System;
using HK2Net;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x02000425 RID: 1061
	[Service]
	internal class SurvivalRewardCalculator : IRewardCalculatorElement
	{
		// Token: 0x060016CE RID: 5838 RVA: 0x0005F910 File Offset: 0x0005DD10
		public void Calculate(MissionContext context, RewardInputData inputData, RewardInputData.Team.Player player, string sessionId, ref uint reward, ref RewardOutputData outputData)
		{
			if (player.firstCheckpoint > player.lastCheckpoint)
			{
				throw new RewardCalculatorElementException(string.Format("Incorrect stages first stage {0}, last stage {1}", player.firstCheckpoint, player.lastCheckpoint));
			}
			outputData.completedStages = player.lastCheckpoint - player.firstCheckpoint;
			SurvivalRewardPool survivalRewardPool = new SurvivalRewardPool();
			foreach (SubLevel subLevel in context.subLevels)
			{
				SurvivalRewardPool pool = (SurvivalRewardPool)subLevel.pool;
				survivalRewardPool.Combine(pool);
			}
			SessionOutcome outcome = RewardCalculatorHelper.GetOutcome(context, inputData.winnerTeamId, (int)player.teamId);
			reward = survivalRewardPool.CalculateReward(player.firstCheckpoint, player.lastCheckpoint, context.missionType.Name, outcome);
		}
	}
}
