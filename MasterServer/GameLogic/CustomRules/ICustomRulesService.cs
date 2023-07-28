using System;
using System.Collections.Generic;
using HK2Net;

namespace MasterServer.GameLogic.CustomRules
{
	// Token: 0x020002A7 RID: 679
	[Contract]
	public interface ICustomRulesService
	{
		// Token: 0x14000033 RID: 51
		// (add) Token: 0x06000E84 RID: 3716
		// (remove) Token: 0x06000E85 RID: 3717
		event RuleSetUpdatedDeleg RuleSetUpdated;

		// Token: 0x06000E86 RID: 3718
		IEnumerable<ICustomRule> GetActiveRules();

		// Token: 0x06000E87 RID: 3719
		IEnumerable<ICustomRule> GetDisabledRules();

		// Token: 0x06000E88 RID: 3720
		ICustomRule AddRule(string config, bool enabled);

		// Token: 0x06000E89 RID: 3721
		ICustomRule GetRule(ulong ruleId);

		// Token: 0x06000E8A RID: 3722
		void EnableRule(ulong ruleId, bool enabled);

		// Token: 0x06000E8B RID: 3723
		void EnableAllCustomRules(bool enabled);

		// Token: 0x06000E8C RID: 3724
		void RemoveRule(ulong ruleId);

		// Token: 0x06000E8D RID: 3725
		void CleanRuleState(ulong ruleId);

		// Token: 0x06000E8E RID: 3726
		void ReloadRules();
	}
}
