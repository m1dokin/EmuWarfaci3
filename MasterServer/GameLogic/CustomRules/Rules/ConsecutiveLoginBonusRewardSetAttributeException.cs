using System;

namespace MasterServer.GameLogic.CustomRules.Rules
{
	// Token: 0x020002C0 RID: 704
	public class ConsecutiveLoginBonusRewardSetAttributeException : Exception
	{
		// Token: 0x06000F19 RID: 3865 RVA: 0x0003C935 File Offset: 0x0003AD35
		public ConsecutiveLoginBonusRewardSetAttributeException(string rewardSetSrc) : base(string.Format("Set of rewards '{0}' configured improperly", rewardSetSrc))
		{
		}
	}
}
