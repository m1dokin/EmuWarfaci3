using System;
using System.Diagnostics;

namespace MasterServer.Core
{
	// Token: 0x0200015D RID: 349
	[ConsoleCmdAttributes(CmdName = "gc_collect", ArgsSize = 0)]
	public class GCCollectCmd : IConsoleCmd
	{
		// Token: 0x0600061A RID: 1562 RVA: 0x00018BC8 File Offset: 0x00016FC8
		public void ExecuteCmd(string[] args)
		{
			Process currentProcess = Process.GetCurrentProcess();
			Log.Info(string.Format("Memory before GC: Working set {0}Mb Paged memory size {1}Mb Virtual memory size {2}Mb GC memory {3}Mb(gc:{4})", new object[]
			{
				currentProcess.WorkingSet64 / 1024L / 1024L,
				currentProcess.PagedMemorySize64 / 1024L / 1024L,
				currentProcess.VirtualMemorySize64 / 1024L / 1024L,
				GC.GetTotalMemory(false) / 1024L / 1024L,
				GC.CollectionCount(0)
			}));
			GC.Collect();
			GC.WaitForPendingFinalizers();
			Log.Info(string.Format("Memory after GC: Working set {0}Mb Paged memory size {1}Mb Virtual memory size {2}Mb GC memory {3}Mb(gc:{4})", new object[]
			{
				currentProcess.WorkingSet64 / 1024L / 1024L,
				currentProcess.PagedMemorySize64 / 1024L / 1024L,
				currentProcess.VirtualMemorySize64 / 1024L / 1024L,
				GC.GetTotalMemory(false) / 1024L / 1024L,
				GC.CollectionCount(0)
			}));
		}
	}
}
