using System;
using System.Collections.Generic;

namespace MasterServer.DAL.CustomRules
{
	// Token: 0x0200002C RID: 44
	public interface ICustomRulesSystem
	{
		// Token: 0x0600006E RID: 110
		DALResultMulti<CustomRuleInfo> GetRules();

		// Token: 0x0600006F RID: 111
		DALResultVoid UpdateRules(IEnumerable<CustomRuleInfo> rules);

		// Token: 0x06000070 RID: 112
		DALResultMulti<CustomRuleInfo> AddRule(CustomRuleInfo rule);

		// Token: 0x06000071 RID: 113
		DALResultMulti<CustomRuleInfo> UpdateRule(CustomRuleInfo rule);

		// Token: 0x06000072 RID: 114
		DALResultMulti<CustomRuleInfo> DeleteRule(ulong ruleID);

		// Token: 0x06000073 RID: 115
		DALResultMulti<CustomRuleRawState> GetRulesState(ulong profileId);

		// Token: 0x06000074 RID: 116
		DALResult<KeyValuePair<bool, CustomRuleRawState>> CompareAndSwapState(CustomRuleRawState rawState);

		// Token: 0x06000075 RID: 117
		DALResultVoid CleanupInactiveRulesState(IEnumerable<ulong> activeRuleIDs);

		// Token: 0x06000076 RID: 118
		DALResultVoid CleanupRuleState(ulong ruleID);

		// Token: 0x06000077 RID: 119
		DALResultVoid SetUpdateTime(ulong profileId, ulong ruleID, DateTime updateTime);
	}
}
