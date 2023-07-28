using System;

namespace MasterServer.Core
{
	// Token: 0x020007AD RID: 1965
	[ConsoleCmdAttributes(CmdName = "write_memory_usage", ArgsSize = 0)]
	internal class WriteMemoryUsageCmd : IConsoleCmd
	{
		// Token: 0x06002884 RID: 10372 RVA: 0x000AE4C3 File Offset: 0x000AC8C3
		public void ExecuteCmd(string[] args)
		{
			Log.WriteComponentsMemoryUsageToConsole();
		}
	}
}
