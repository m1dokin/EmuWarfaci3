using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000570 RID: 1392
	[ConsoleCmdAttributes(ArgsSize = 0, CmdName = "dump_progression_rules", Help = "")]
	internal class DumpProgressionRulesCmd : IConsoleCmd
	{
		// Token: 0x06001E07 RID: 7687 RVA: 0x00079BDC File Offset: 0x00077FDC
		public DumpProgressionRulesCmd(IProfileProgressionDebug profileProgressionDebug)
		{
			this.m_profileProgressionDebug = profileProgressionDebug;
		}

		// Token: 0x06001E08 RID: 7688 RVA: 0x00079BEB File Offset: 0x00077FEB
		public void ExecuteCmd(string[] args)
		{
			this.m_profileProgressionDebug.DumpRules();
		}

		// Token: 0x04000E84 RID: 3716
		private readonly IProfileProgressionDebug m_profileProgressionDebug;
	}
}
