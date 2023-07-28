using System;

namespace MasterServer.Core
{
	// Token: 0x02000153 RID: 339
	[ConsoleCmdAttributes(CmdName = "status", ArgsSize = 0, Help = "Report current execution phase status")]
	internal class ServicesStatusCmd : IConsoleCmd
	{
		// Token: 0x060005E1 RID: 1505 RVA: 0x00017890 File Offset: 0x00015C90
		public void ExecuteCmd(string[] args)
		{
			Log.Info<ExecutionPhase>("{0}", ServicesManager.ExecutionPhase);
		}
	}
}
