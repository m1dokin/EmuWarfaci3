using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.CustomRules
{
	// Token: 0x020002A1 RID: 673
	[ConsoleCmdAttributes(CmdName = "custom_rules_enable_all", Help = "Enables all custom rules")]
	internal class EnableAllCustomRulesCmd : IConsoleCmd
	{
		// Token: 0x06000E74 RID: 3700 RVA: 0x0003A483 File Offset: 0x00038883
		public EnableAllCustomRulesCmd(ICustomRulesService customRulesService)
		{
			this.m_customRulesService = customRulesService;
		}

		// Token: 0x06000E75 RID: 3701 RVA: 0x0003A492 File Offset: 0x00038892
		public void ExecuteCmd(string[] args)
		{
			this.m_customRulesService.EnableAllCustomRules(true);
			Log.Info("All custom rules enabled!");
		}

		// Token: 0x040006AC RID: 1708
		private readonly ICustomRulesService m_customRulesService;
	}
}
