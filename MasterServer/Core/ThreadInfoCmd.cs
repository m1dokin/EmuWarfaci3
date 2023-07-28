using System;
using System.Text;
using MasterServer.Core.Diagnostics.Threading;

namespace MasterServer.Core
{
	// Token: 0x0200015B RID: 347
	[ConsoleCmdAttributes(CmdName = "threadinfo", ArgsSize = 0)]
	public class ThreadInfoCmd : IConsoleCmd
	{
		// Token: 0x06000615 RID: 1557 RVA: 0x00018778 File Offset: 0x00016B78
		public void ExecuteCmd(string[] args)
		{
			ThreadTracker.ThreadEntry[] threadStats = ThreadTracker.GetThreadStats();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Thread info:");
			stringBuilder.AppendLine(string.Format("{0,-4}{1,-20}{2,-15}{3,-3}{4,-18}{5}", new object[]
			{
				"ID",
				"Name",
				"State",
				"TR",
				"LastLoop",
				"TotalTime"
			}));
			foreach (ThreadTracker.ThreadEntry threadEntry in threadStats)
			{
				stringBuilder.AppendLine(string.Format("{0,-4}{1,-20}{2,-15}{3,-3}{4,-18}{5}", new object[]
				{
					threadEntry.Thread.ManagedThreadId,
					(!threadEntry.Thread.IsThreadPoolThread) ? threadEntry.Thread.Name : ("[TP]" + threadEntry.Thread.Name),
					threadEntry.Thread.ThreadState,
					(!threadEntry.IsActive) ? "-" : "A",
					threadEntry.GetLastTimeSlice(),
					threadEntry.TotalTime
				}));
			}
			Log.Info(stringBuilder.ToString());
		}
	}
}
