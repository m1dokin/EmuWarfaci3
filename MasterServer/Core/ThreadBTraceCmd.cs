using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using MasterServer.Core.Diagnostics.Threading;

namespace MasterServer.Core
{
	// Token: 0x0200015C RID: 348
	[ConsoleCmdAttributes(CmdName = "backtrace", ArgsSize = 0)]
	public class ThreadBTraceCmd : IConsoleCmd
	{
		// Token: 0x06000617 RID: 1559 RVA: 0x000188C4 File Offset: 0x00016CC4
		public void ExecuteCmd(string[] args)
		{
			ThreadTracker.ThreadEntry[] threadStats = ThreadTracker.GetThreadStats();
			foreach (ThreadTracker.ThreadEntry threadEntry in threadStats)
			{
				if (threadEntry.Thread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
				{
					try
					{
						threadEntry.Thread.Suspend();
					}
					catch
					{
					}
				}
			}
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("Backtrace:");
				foreach (ThreadTracker.ThreadEntry threadEntry2 in threadStats)
				{
					stringBuilder.AppendFormat("=== Callstack for thread [{0}] <{1}> {2} ===\n", threadEntry2.Thread.ManagedThreadId, (!threadEntry2.IsActive) ? "-" : "A", threadEntry2.Thread.Name);
					stringBuilder.AppendFormat("LastLoop: {0}\n", threadEntry2.GetLastTimeSlice());
					stringBuilder.AppendFormat("TotalTime: {0}\n", threadEntry2.TotalTime);
					StackTrace stackTrace = null;
					try
					{
						stackTrace = new StackTrace(threadEntry2.Thread, true);
					}
					catch (NotImplementedException)
					{
						stringBuilder.AppendLine("* Using Linux fallback *");
						if (!string.IsNullOrEmpty(threadEntry2.Hint))
						{
							stringBuilder.Append("Trace hint: ");
							stringBuilder.AppendLine(threadEntry2.Hint);
						}
						stackTrace = threadEntry2.StackTrace;
					}
					if (stackTrace != null)
					{
						for (int num = 0; num != stackTrace.FrameCount; num++)
						{
							this.PrintFrame(num, stackTrace.GetFrame(num), stringBuilder);
						}
						stringBuilder.AppendLine();
					}
				}
				Log.Info(stringBuilder.ToString());
			}
			finally
			{
				foreach (ThreadTracker.ThreadEntry threadEntry3 in threadStats)
				{
					if (threadEntry3.Thread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
					{
						try
						{
							threadEntry3.Thread.Resume();
						}
						catch
						{
						}
					}
				}
			}
		}

		// Token: 0x06000618 RID: 1560 RVA: 0x00018B40 File Offset: 0x00016F40
		private void PrintFrame(int id, StackFrame frame, StringBuilder sb)
		{
			MethodBase method = frame.GetMethod();
			sb.Append(id);
			sb.Append("  ");
			if (method != null)
			{
				sb.AppendLine(string.Format("{0}::{1}", method.DeclaringType, method.Name));
			}
			if (frame.GetFileName() != null)
			{
				sb.AppendLine(string.Format(" at {0}:{1}", frame.GetFileName(), frame.GetFileLineNumber()));
			}
		}
	}
}
