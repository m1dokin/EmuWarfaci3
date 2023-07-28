using System;
using System.Collections.Generic;
using MasterServer.DAL.CustomRules;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x020001FD RID: 509
	public interface ICustomRulesSystemClient
	{
		// Token: 0x06000A3B RID: 2619
		IEnumerable<CustomRuleInfo> GetRules();

		// Token: 0x06000A3C RID: 2620
		void UpdateRules(IEnumerable<CustomRuleInfo> rules);

		// Token: 0x06000A3D RID: 2621
		IEnumerable<CustomRuleInfo> AddRule(CustomRuleInfo rule);

		// Token: 0x06000A3E RID: 2622
		IEnumerable<CustomRuleInfo> UpdateRule(CustomRuleInfo rule);

		// Token: 0x06000A3F RID: 2623
		IEnumerable<CustomRuleInfo> DeleteRule(ulong ruleId);

		// Token: 0x06000A40 RID: 2624
		CustomRuleRawState GetRuleState(ulong profileId, ulong ruleId);

		// Token: 0x06000A41 RID: 2625
		KeyValuePair<bool, CustomRuleRawState> CompareAndSwapState(CustomRuleRawState rawState);

		// Token: 0x06000A42 RID: 2626
		void CleanupInactiveRulesState(IEnumerable<ulong> activeRuleIDs);

		// Token: 0x06000A43 RID: 2627
		void CleanupRuleState(ulong ruleId);

		// Token: 0x06000A44 RID: 2628
		void SetUpdateTime(ulong profileId, ulong ruleID, DateTime updateTime);
	}
}
