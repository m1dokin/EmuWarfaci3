using System;

namespace MasterServer.Core.Commands
{
	// Token: 0x02000030 RID: 48
	[ConsoleCmdAttributes(CmdName = "help", Help = "Gets help about all console commands.")]
	internal class HelpCmd : ConsoleCommand<HelpCmdParams>
	{
		// Token: 0x060000BE RID: 190 RVA: 0x000078F0 File Offset: 0x00005CF0
		protected override void Execute(HelpCmdParams param)
		{
			ConsoleCmdManager.Dump(param.Contains, param.Full);
		}
	}
}
