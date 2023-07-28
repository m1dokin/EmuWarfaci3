using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.CustomRules
{
	// Token: 0x020002B5 RID: 693
	[ConsoleCmdAttributes(CmdName = "custom_rule_remove_static", ArgsSize = 1, Help = "Remove static custom rule from DB (args:ruleId)")]
	internal class RemoveStaticRuleCmd : IConsoleCmd
	{
		// Token: 0x06000EE3 RID: 3811 RVA: 0x0003BA07 File Offset: 0x00039E07
		public RemoveStaticRuleCmd(ICustomRulesService customRulesService, IDebugCustomRulesService debugCustomRulesService)
		{
			this.m_customRulesService = customRulesService;
			this.m_debugCustomRulesService = debugCustomRulesService;
		}

		// Token: 0x06000EE4 RID: 3812 RVA: 0x0003BA20 File Offset: 0x00039E20
		public void ExecuteCmd(string[] args)
		{
			ulong ruleId = ulong.Parse(args[1]);
			this.m_debugCustomRulesService.RemoveStaticRule(ruleId);
			this.m_customRulesService.ReloadRules();
			Log.Info("DONE!");
		}

		// Token: 0x040006CF RID: 1743
		private readonly ICustomRulesService m_customRulesService;

		// Token: 0x040006D0 RID: 1744
		private readonly IDebugCustomRulesService m_debugCustomRulesService;
	}
}
