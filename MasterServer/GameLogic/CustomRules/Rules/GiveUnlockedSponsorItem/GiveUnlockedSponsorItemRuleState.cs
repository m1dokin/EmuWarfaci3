using System;

namespace MasterServer.GameLogic.CustomRules.Rules.GiveUnlockedSponsorItem
{
	// Token: 0x020002CC RID: 716
	internal class GiveUnlockedSponsorItemRuleState : CustomRuleState
	{
		// Token: 0x06000F4E RID: 3918 RVA: 0x0003D72D File Offset: 0x0003BB2D
		internal GiveUnlockedSponsorItemRuleState()
		{
			base.LastActivationTime = DateTime.MinValue;
		}

		// Token: 0x0400071A RID: 1818
		public const byte TYPE_ID = 3;

		// Token: 0x0400071B RID: 1819
		public const int DATA_VERSION = 0;
	}
}
