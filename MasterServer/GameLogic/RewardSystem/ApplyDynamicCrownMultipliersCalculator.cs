using System;
using System.Collections;
using HK2Net;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x02000419 RID: 1049
	[Service]
	internal class ApplyDynamicCrownMultipliersCalculator : IRewardCalculatorElement
	{
		// Token: 0x060016A5 RID: 5797 RVA: 0x0005EB0C File Offset: 0x0005CF0C
		public void Calculate(MissionContext context, RewardInputData inputData, RewardInputData.Team.Player player, string sessionId, ref uint reward, ref RewardOutputData outputData)
		{
			ProfileCrownReward profileCrownReward = new ProfileCrownReward();
			IEnumerator enumerator = outputData.crownReward.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					ProfileStatCrownReward profileStatCrownReward = (ProfileStatCrownReward)obj;
					ulong reward2 = (ulong)(0.5 + (double)(profileStatCrownReward.Reward * player.dynamicMultiplier.CrownMultiplier));
					profileCrownReward.Add(new ProfileStatCrownReward(profileStatCrownReward.Stat, reward2, profileStatCrownReward.StatValue));
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			outputData.crownReward = profileCrownReward;
		}
	}
}
