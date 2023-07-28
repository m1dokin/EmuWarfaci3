using System;
using System.Text;
using MasterServer.Core;

namespace MasterServer.GameLogic.CustomRules
{
	// Token: 0x020002A3 RID: 675
	[ConsoleCmdAttributes(CmdName = "custom_rule_list", ArgsSize = 0, Help = "Lists all active custom rules")]
	internal class CustomRulesListCmd : IConsoleCmd
	{
		// Token: 0x06000E79 RID: 3705 RVA: 0x0003A4CA File Offset: 0x000388CA
		public CustomRulesListCmd(ICustomRulesService customRulesService)
		{
			this.m_customRulesService = customRulesService;
		}

		// Token: 0x06000E7A RID: 3706 RVA: 0x0003A4DC File Offset: 0x000388DC
		public void ExecuteCmd(string[] args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("\nActive rules:");
			int num = 0;
			foreach (ICustomRule arg in this.m_customRulesService.GetActiveRules())
			{
				stringBuilder.AppendFormat("  {0}: {1}\n", num++, arg);
			}
			stringBuilder.AppendLine("\nDisabled rules:");
			num = 0;
			foreach (ICustomRule arg2 in this.m_customRulesService.GetDisabledRules())
			{
				stringBuilder.AppendFormat("  {0}: {1}\n", num++, arg2);
			}
			Log.Info<string>("{0}", stringBuilder.ToString());
		}

		// Token: 0x040006AE RID: 1710
		private readonly ICustomRulesService m_customRulesService;
	}
}
