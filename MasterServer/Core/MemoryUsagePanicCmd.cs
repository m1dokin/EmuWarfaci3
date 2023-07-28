using System;

namespace MasterServer.Core
{
	// Token: 0x020007AC RID: 1964
	[ConsoleCmdAttributes(CmdName = "memory_usage_panic", ArgsSize = 1)]
	internal class MemoryUsagePanicCmd : IConsoleCmd
	{
		// Token: 0x06002882 RID: 10370 RVA: 0x000AE46C File Offset: 0x000AC86C
		public void ExecuteCmd(string[] args)
		{
			if (args.Length == 1)
			{
				Log.Info<long>("memory_usage_panic = {0}Mb", Log.MemoryUsagePanic / 1024L / 1024L);
				return;
			}
			Log.MemoryUsagePanic = long.Parse(args[1]) * 1024L * 1024L;
		}
	}
}
