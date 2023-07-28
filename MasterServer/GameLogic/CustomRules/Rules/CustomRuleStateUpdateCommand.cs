using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.CustomRules.Rules
{
	// Token: 0x020002C8 RID: 712
	[ConsoleCmdAttributes(CmdName = "custom_rule_state_update", Help = "Update state of rule (args:profile_id, rule_id, last_activation_time)")]
	internal class CustomRuleStateUpdateCommand : IConsoleCmd
	{
		// Token: 0x06000F2C RID: 3884 RVA: 0x0003CED8 File Offset: 0x0003B2D8
		public CustomRuleStateUpdateCommand(IDebugCustomRulesStateStorage debugCustomRulesStateStorage)
		{
			this.m_debugCustomRulesStateStorage = debugCustomRulesStateStorage;
		}

		// Token: 0x06000F2D RID: 3885 RVA: 0x0003CEE8 File Offset: 0x0003B2E8
		public void ExecuteCmd(string[] args)
		{
			ulong profileID = ulong.Parse(args[1]);
			ulong num = ulong.Parse(args[2]);
			DateTime updateTime = DateTime.Parse(args[3]);
			try
			{
				CustomRuleState p = this.m_debugCustomRulesStateStorage.SetUpdateTime(profileID, num, updateTime);
				Log.Info<ulong, CustomRuleState>("State for rule {0} update was successful.\n{1}", num, p);
			}
			catch (Exception)
			{
				Log.Info<ulong>("State update failed for rule {0}", num);
				throw;
			}
		}

		// Token: 0x04000701 RID: 1793
		private readonly IDebugCustomRulesStateStorage m_debugCustomRulesStateStorage;
	}
}
