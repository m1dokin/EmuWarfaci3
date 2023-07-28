using System;
using System.Runtime.CompilerServices;
using System.Threading;
using MasterServer.Core.Configuration;
using MasterServer.Core.Diagnostics.Profiler;
using Util.Common;

namespace MasterServer.Core
{
	// Token: 0x0200015E RID: 350
	internal class ThreadPoolProxy
	{
		// Token: 0x0600061C RID: 1564 RVA: 0x00018D13 File Offset: 0x00017113
		public static bool QueueUserWorkItem(WaitCallback callBack)
		{
			return ThreadPoolProxy.QueueUserWorkItem(callBack, null, false);
		}

		// Token: 0x0600061D RID: 1565 RVA: 0x00018D20 File Offset: 0x00017120
		public static bool QueueUserWorkItem(WaitCallback callBack, object state, bool suppressFlow = false)
		{
			bool flag = false;
			bool result;
			try
			{
				if (suppressFlow && !ExecutionContext.IsFlowSuppressed())
				{
					ExecutionContext.SuppressFlow();
					flag = true;
				}
				if (ThreadPoolProxy.<>f__mg$cache0 == null)
				{
					ThreadPoolProxy.<>f__mg$cache0 = new WaitCallback(ThreadPoolProxy.Dispatch);
				}
				result = ThreadPool.QueueUserWorkItem(ThreadPoolProxy.<>f__mg$cache0, new ThreadPoolProxy.DispatchInfo(callBack, state));
			}
			finally
			{
				if (flag)
				{
					ExecutionContext.RestoreFlow();
				}
			}
			return result;
		}

		// Token: 0x0600061E RID: 1566 RVA: 0x00018D94 File Offset: 0x00017194
		public static ThreadPoolProxy.ThreadsInfo GetWorkersInfo()
		{
			ThreadPoolProxy.ThreadsInfo result = default(ThreadPoolProxy.ThreadsInfo);
			int num;
			int num2;
			ThreadPool.GetAvailableThreads(out num, out num2);
			int num3;
			int num4;
			ThreadPool.GetMaxThreads(out num3, out num4);
			result.TotalWorkers = num3;
			result.UsedWorkers = num3 - num;
			return result;
		}

		// Token: 0x0600061F RID: 1567 RVA: 0x00018DD0 File Offset: 0x000171D0
		public static void Configure()
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("ThreadPool");
			int num;
			section.Get("MinThread", out num);
			int num2;
			int.TryParse(section.Get("MaxThread"), out num2);
			int num3;
			int completionPortThreads;
			ThreadPool.GetMaxThreads(out num3, out completionPortThreads);
			int num4;
			int completionPortThreads2;
			ThreadPool.GetMinThreads(out num4, out completionPortThreads2);
			if (num > 0 && num > num4)
			{
				ThreadPool.SetMinThreads(num, completionPortThreads2);
			}
			if (num2 > 0 && num2 < num3)
			{
				ThreadPool.SetMaxThreads(num2, completionPortThreads);
			}
			ThreadPool.GetMaxThreads(out num3, out completionPortThreads);
			ThreadPool.GetMinThreads(out num4, out completionPortThreads2);
			Log.Info<int>("ThreadPool min threads {0}", num4);
			Log.Info<int>("ThreadPool max threads {0}", num3);
		}

		// Token: 0x06000620 RID: 1568 RVA: 0x00018E7C File Offset: 0x0001727C
		private static void Dispatch(object ctx)
		{
			try
			{
				ThreadPoolProxy.DispatchInfo dispatchInfo = (ThreadPoolProxy.DispatchInfo)ctx;
				CultureHelpers.SetNeutralThreadCulture();
				ThreadPoolProxy.m_stats.PreDispatch(dispatchInfo.watch.Stop());
				ThreadPoolProxy.CheckSaturation();
				dispatchInfo.callback(dispatchInfo.state);
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
			finally
			{
				ThreadPoolProxy.m_stats.PostDispatch();
			}
		}

		// Token: 0x06000621 RID: 1569 RVA: 0x00018EF8 File Offset: 0x000172F8
		public static ThreadPoolProxy.Stats GetStats(bool resetPeak)
		{
			ThreadPoolProxy.Stats stats = ThreadPoolProxy.m_stats;
			if (resetPeak)
			{
				Interlocked.Exchange(ref ThreadPoolProxy.m_stats.TimeInQueuePeakMs, 0);
			}
			return stats;
		}

		// Token: 0x06000622 RID: 1570 RVA: 0x00018F24 File Offset: 0x00017324
		private static void CheckSaturation()
		{
			int num;
			int num2;
			ThreadPool.GetMaxThreads(out num, out num2);
			int num3;
			int num4;
			ThreadPool.GetAvailableThreads(out num3, out num4);
			float num5 = 1f - (float)num3 / (float)num;
			if (num5 >= 0.9f && DateTime.Now - ThreadPoolProxy.m_last_alert >= ThreadPoolProxy.THREAD_SATURATION_ALERT_COOLDOWN)
			{
				ThreadPoolProxy.m_last_alert = DateTime.Now;
				Log.Warning<int, int>("Thread pool reached critical saturation ({0} / {1}), dumping backtraces:", num - num3, num);
				ThreadBTraceCmd threadBTraceCmd = new ThreadBTraceCmd();
				threadBTraceCmd.ExecuteCmd(new string[0]);
			}
		}

		// Token: 0x040003DF RID: 991
		private const float THREAD_SATURATION_ALERT = 0.9f;

		// Token: 0x040003E0 RID: 992
		private static readonly TimeSpan THREAD_SATURATION_ALERT_COOLDOWN = new TimeSpan(0, 2, 0);

		// Token: 0x040003E1 RID: 993
		private static ThreadPoolProxy.Stats m_stats;

		// Token: 0x040003E2 RID: 994
		private static DateTime m_last_alert = DateTime.Now;

		// Token: 0x040003E3 RID: 995
		[CompilerGenerated]
		private static WaitCallback <>f__mg$cache0;

		// Token: 0x0200015F RID: 351
		public struct Stats
		{
			// Token: 0x06000624 RID: 1572 RVA: 0x00018FC0 File Offset: 0x000173C0
			public void PreDispatch(TimeSpan in_queue)
			{
				Interlocked.Increment(ref this.ItemsRunning);
				Interlocked.Increment(ref this.ItemsDispatched);
				int num = (int)in_queue.TotalMilliseconds;
				Interlocked.Add(ref this.TimeInQueueTotalMs, num);
				int timeInQueuePeakMs;
				int num2;
				do
				{
					timeInQueuePeakMs = this.TimeInQueuePeakMs;
					num2 = ((num <= timeInQueuePeakMs) ? timeInQueuePeakMs : num);
				}
				while (num2 != Interlocked.CompareExchange(ref this.TimeInQueuePeakMs, num2, timeInQueuePeakMs));
			}

			// Token: 0x06000625 RID: 1573 RVA: 0x00019024 File Offset: 0x00017424
			public void PostDispatch()
			{
				Interlocked.Decrement(ref this.ItemsRunning);
			}

			// Token: 0x040003E4 RID: 996
			public int ItemsDispatched;

			// Token: 0x040003E5 RID: 997
			public int ItemsRunning;

			// Token: 0x040003E6 RID: 998
			public int TimeInQueueTotalMs;

			// Token: 0x040003E7 RID: 999
			public int TimeInQueuePeakMs;
		}

		// Token: 0x02000160 RID: 352
		public struct ThreadsInfo
		{
			// Token: 0x040003E8 RID: 1000
			public int TotalWorkers;

			// Token: 0x040003E9 RID: 1001
			public int UsedWorkers;
		}

		// Token: 0x02000161 RID: 353
		private class DispatchInfo
		{
			// Token: 0x06000626 RID: 1574 RVA: 0x00019032 File Offset: 0x00017432
			public DispatchInfo(WaitCallback callback, object state)
			{
				this.callback = callback;
				this.state = state;
				this.watch = new TimeExecution();
			}

			// Token: 0x040003EA RID: 1002
			public TimeExecution watch;

			// Token: 0x040003EB RID: 1003
			public WaitCallback callback;

			// Token: 0x040003EC RID: 1004
			public object state;
		}
	}
}
