using System;

namespace MasterServer.GameLogic.CustomRules.Rules
{
	// Token: 0x020002C2 RID: 706
	public class ConsecutiveLoginBonusRewardSetDoesNotContainSingleRewardException : Exception
	{
		// Token: 0x06000F1B RID: 3867 RVA: 0x0003C951 File Offset: 0x0003AD51
		public ConsecutiveLoginBonusRewardSetDoesNotContainSingleRewardException(string rewardSetSrc) : base(string.Format("Set of rewards '{0}' should contain single reward(action).", rewardSetSrc))
		{
		}
	}
}
