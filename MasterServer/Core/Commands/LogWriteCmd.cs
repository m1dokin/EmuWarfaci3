using System;

namespace MasterServer.Core.Commands
{
	// Token: 0x02000032 RID: 50
	[ConsoleCmdAttributes(CmdName = "log_write")]
	internal class LogWriteCmd : ConsoleCommand<LogWriteCmdParams>
	{
		// Token: 0x060000C5 RID: 197 RVA: 0x00007935 File Offset: 0x00005D35
		protected override void Execute(LogWriteCmdParams param)
		{
			Log.Info(param.Text);
		}
	}
}
