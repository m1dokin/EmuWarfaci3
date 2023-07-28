using System;
using HK2Net;
using MasterServer.GameLogic.CustomRules.Rules;

namespace MasterServer.GameLogic.CustomRules
{
	// Token: 0x020002AF RID: 687
	[Contract]
	public interface IDebugCustomRulesStateStorage
	{
		// Token: 0x06000EC6 RID: 3782
		CustomRuleState SetUpdateTime(ulong profileID, ulong ruleID, DateTime updateTime);
	}
}
