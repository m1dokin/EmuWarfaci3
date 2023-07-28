using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Configuration;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x0200041C RID: 1052
	[Service]
	internal class BaseRewardCalculator : IRewardCalculatorElement
	{
		// Token: 0x060016AD RID: 5805 RVA: 0x0005EE3E File Offset: 0x0005D23E
		public BaseRewardCalculator(IConfigurationService configurationService)
		{
			this.m_configurationService = configurationService;
		}

		// Token: 0x060016AE RID: 5806 RVA: 0x0005EE50 File Offset: 0x0005D250
		public void Calculate(MissionContext context, RewardInputData inputData, RewardInputData.Team.Player player, string sessionId, ref uint reward, ref RewardOutputData outputData)
		{
			RewardPool rewardPool = new RewardPool();
			if (inputData.passedSubLevelsCount > 0U || inputData.passedCheckpointsCount > 0U)
			{
				rewardPool = this.GetRewardPoolsSum(context, inputData.passedSubLevelsCount, inputData.passedCheckpointsCount);
			}
			SessionOutcome outcome = RewardCalculatorHelper.GetOutcome(context, inputData.winnerTeamId, (int)player.teamId);
			ConfigSection section = this.m_configurationService.GetConfig(MsConfigInfo.RewardsConfiguration).GetSection("Rewards");
			uint byOutcome = rewardPool.GetByOutcome(outcome);
			float num = (float)Math.Max(0, player.score) / Math.Max(1f, (float)inputData.maxSessionScore);
			if (!inputData.incompleteSession)
			{
				reward = (uint)(rewardPool.Score * num);
				float num2 = (!player.inSessionFromStart) ? num : 1f;
				reward += (uint)(byOutcome * num2);
			}
			else
			{
				reward = (uint)(inputData.sessionTime / 60f * uint.Parse(section.Get("IncompleteSessionRewardPerMin")));
				uint val = (uint)((rewardPool.Score + byOutcome) * float.Parse(section.Get("IncompleteSessionRewardCap")));
				reward = Math.Min(reward, val);
				reward = (uint)(reward * num);
			}
		}

		// Token: 0x060016AF RID: 5807 RVA: 0x0005EF88 File Offset: 0x0005D388
		private RewardPool GetRewardPoolsSum(MissionContext missionCtx, uint passedSubLevelsCount, uint passedCheckpointsCount)
		{
			RewardPool pool = (RewardPool)missionCtx.baseLevel.pool;
			RewardPool rewardPool = new RewardPool(pool);
			uint num = 0U;
			foreach (SubLevel subLevel in missionCtx.subLevels)
			{
				if (num > passedSubLevelsCount)
				{
					break;
				}
				if (num == passedSubLevelsCount && passedCheckpointsCount == 0U)
				{
					break;
				}
				RewardPool rewardPool2 = new RewardPool((RewardPool)subLevel.pool);
				if (num == passedSubLevelsCount && passedCheckpointsCount > 0U)
				{
					float lastSublevelPoolMultiplier = this.GetLastSublevelPoolMultiplier(passedCheckpointsCount);
					rewardPool2.Mul(lastSublevelPoolMultiplier, 1f, lastSublevelPoolMultiplier, 1f);
				}
				rewardPool.Plus(rewardPool2);
				num += 1U;
			}
			return rewardPool;
		}

		// Token: 0x060016B0 RID: 5808 RVA: 0x0005F064 File Offset: 0x0005D464
		private float GetLastSublevelPoolMultiplier(uint passedCheckpointsCount)
		{
			if (passedCheckpointsCount == 0U)
			{
				return 0f;
			}
			ConfigSection section = this.m_configurationService.GetConfig(MsConfigInfo.RewardsConfiguration).GetSection("Rewards");
			if (section != null)
			{
				List<string> list = section.GetList("checkpoints_passed_reward_mults");
				if (list != null)
				{
					if ((long)list.Count >= (long)((ulong)passedCheckpointsCount))
					{
						return float.Parse(list[(int)(passedCheckpointsCount - 1U)]);
					}
					Log.Warning<uint>("Unexpected: too many checkpoints passed ({0})", passedCheckpointsCount);
				}
			}
			return 1f;
		}

		// Token: 0x04000AF5 RID: 2805
		private readonly IConfigurationService m_configurationService;
	}
}
