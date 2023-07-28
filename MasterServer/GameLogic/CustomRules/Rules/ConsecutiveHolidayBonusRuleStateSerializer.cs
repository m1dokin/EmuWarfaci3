using System;

namespace MasterServer.GameLogic.CustomRules.Rules
{
	// Token: 0x020002B6 RID: 694
	[CustomRuleStateSerializer("consecutive_login_bonus", 2, typeof(ConsecutiveLoginBonusHolidayRule))]
	internal class ConsecutiveHolidayBonusRuleStateSerializer : ConsecutiveLoginBonusRuleStateSerializer
	{
	}
}
