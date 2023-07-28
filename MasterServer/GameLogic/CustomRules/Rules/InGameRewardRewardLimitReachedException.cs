using System;
using System.Runtime.Serialization;

namespace MasterServer.GameLogic.CustomRules.Rules
{
	// Token: 0x020002CE RID: 718
	[Serializable]
	public class InGameRewardRewardLimitReachedException : Exception
	{
		// Token: 0x06000F52 RID: 3922 RVA: 0x0003D8AC File Offset: 0x0003BCAC
		public InGameRewardRewardLimitReachedException(ulong profileId, int rewardLimit) : base(string.Format("In game reward limit '{0}' for profile {1} is reached.", rewardLimit, profileId))
		{
		}

		// Token: 0x06000F53 RID: 3923 RVA: 0x0003D8CA File Offset: 0x0003BCCA
		public InGameRewardRewardLimitReachedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
