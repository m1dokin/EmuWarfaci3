using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.CustomRules.Rules
{
	// Token: 0x020002C7 RID: 711
	[ConsoleCmdAttributes(CmdName = "custom_rule_get_state", Help = "Get state of rule (args:profile_id, rule_id)")]
	internal class CustomRuleStateGetCommand : IConsoleCmd
	{
		// Token: 0x06000F2A RID: 3882 RVA: 0x0003CE8F File Offset: 0x0003B28F
		public CustomRuleStateGetCommand(ICustomRulesStateStorage customRulesStateStorage)
		{
			this.m_customRulesStateStorage = customRulesStateStorage;
		}

		// Token: 0x06000F2B RID: 3883 RVA: 0x0003CEA0 File Offset: 0x0003B2A0
		public void ExecuteCmd(string[] args)
		{
			ulong profileID = ulong.Parse(args[1]);
			ulong ruleID = ulong.Parse(args[2]);
			CustomRuleState state = this.m_customRulesStateStorage.GetState(profileID, ruleID);
			Log.Info(state.ToString());
		}

		// Token: 0x04000700 RID: 1792
		private readonly ICustomRulesStateStorage m_customRulesStateStorage;
	}
}
