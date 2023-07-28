using System;
using HK2Net;
using MasterServer.GameLogic.CustomRules.Rules;

namespace MasterServer.GameLogic.CustomRules
{
	// Token: 0x020002AE RID: 686
	[Contract]
	public interface ICustomRulesStateStorage
	{
		// Token: 0x06000EC3 RID: 3779
		CustomRuleState GetState(ulong profileID, ulong ruleID);

		// Token: 0x06000EC4 RID: 3780
		CustomRuleState GetState(ulong profileID, ICustomRule rule);

		// Token: 0x06000EC5 RID: 3781
		bool UpdateState(ulong profileID, ICustomRule rule, Func<CustomRuleState, bool> updateFunc);
	}
}
