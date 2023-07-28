using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.CustomRules
{
	// Token: 0x020002A5 RID: 677
	[ConsoleCmdAttributes(CmdName = "custom_rules_reload", Help = "Trigger custom rules reload")]
	internal class CustomRulesReloadCommand : IConsoleCmd
	{
		// Token: 0x06000E7E RID: 3710 RVA: 0x0003A6F9 File Offset: 0x00038AF9
		public CustomRulesReloadCommand(ICustomRulesService customRulesService)
		{
			this.m_customRulesService = customRulesService;
		}

		// Token: 0x06000E7F RID: 3711 RVA: 0x0003A708 File Offset: 0x00038B08
		public void ExecuteCmd(string[] args)
		{
			this.m_customRulesService.ReloadRules();
		}

		// Token: 0x040006B1 RID: 1713
		private readonly ICustomRulesService m_customRulesService;
	}
}
