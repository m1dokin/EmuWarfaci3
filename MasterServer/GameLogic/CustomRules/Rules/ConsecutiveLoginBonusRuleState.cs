using System;

namespace MasterServer.GameLogic.CustomRules.Rules
{
	// Token: 0x020002C5 RID: 709
	internal class ConsecutiveLoginBonusRuleState : CustomRuleState
	{
		// Token: 0x040006FC RID: 1788
		public const byte TYPE_ID = 2;

		// Token: 0x040006FD RID: 1789
		public const int DATA_VERSION = 0;

		// Token: 0x040006FE RID: 1790
		public int PrevStreak;

		// Token: 0x040006FF RID: 1791
		public int PrevReward = -1;
	}
}
