using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.CustomRules
{
	// Token: 0x020002A0 RID: 672
	[ConsoleCmdAttributes(CmdName = "custom_rules_disable_all", Help = "Disables all custom rules")]
	internal class DisableAllCustomRulesCmd : IConsoleCmd
	{
		// Token: 0x06000E72 RID: 3698 RVA: 0x0003A45C File Offset: 0x0003885C
		public DisableAllCustomRulesCmd(ICustomRulesService customRulesService)
		{
			this.m_customRulesService = customRulesService;
		}

		// Token: 0x06000E73 RID: 3699 RVA: 0x0003A46B File Offset: 0x0003886B
		public void ExecuteCmd(string[] args)
		{
			this.m_customRulesService.EnableAllCustomRules(false);
			Log.Info("All custom rules disabled!");
		}

		// Token: 0x040006AB RID: 1707
		private readonly ICustomRulesService m_customRulesService;
	}
}
