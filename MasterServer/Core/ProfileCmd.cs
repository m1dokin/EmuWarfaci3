using System;

namespace MasterServer.Core
{
	// Token: 0x020007A2 RID: 1954
	[ConsoleCmdAttributes(CmdName = "profile", ArgsSize = 1)]
	internal class ProfileCmd : IConsoleCmd
	{
		// Token: 0x0600286F RID: 10351 RVA: 0x000AE0CC File Offset: 0x000AC4CC
		public void ExecuteCmd(string[] args)
		{
			if (args.Length > 1)
			{
				Profiler.Mode = (Profiler.EMode)int.Parse(args[1]);
			}
		}
	}
}
