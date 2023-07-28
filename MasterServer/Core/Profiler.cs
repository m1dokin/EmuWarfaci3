using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using MasterServer.Core.Diagnostics.Threading;

namespace MasterServer.Core
{
	// Token: 0x02000833 RID: 2099
	public static class Profiler
	{
		// Token: 0x06002B70 RID: 11120 RVA: 0x000BBFE4 File Offset: 0x000BA3E4
		public static void Init()
		{
			for (int i = 0; i < 3; i++)
			{
				Profiler.m_modulesCtx[i].module = (Profiler.EModule)i;
				Profiler.m_modulesCtx[i].funcEntity = new Dictionary<string, Profiler.FunctionProfilerEntity>();
			}
			Profiler.m_mode = Profiler.EMode.NONE;
			Profiler.m_freq = 10;
			if (Profiler.<>f__mg$cache0 == null)
			{
				Profiler.<>f__mg$cache0 = new ParameterizedThreadStart(Profiler.Update);
			}
			Profiler.m_profilerThread = new Thread(Profiler.<>f__mg$cache0);
			Profiler.m_profilerThread.Name = "Profiler thread";
			Profiler.m_profilerThread.IsBackground = true;
			Profiler.m_profilerThread.Start();
		}

		// Token: 0x06002B71 RID: 11121 RVA: 0x000BC084 File Offset: 0x000BA484
		public static void Close()
		{
			try
			{
				Profiler.m_profilerThread.Abort();
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06002B72 RID: 11122 RVA: 0x000BC0B8 File Offset: 0x000BA4B8
		public static void FunctionReport(Profiler.EModule module, string funcName, double duration)
		{
			Profiler.ModuleProfilerContext moduleProfilerContext = Profiler.m_modulesCtx[(int)module];
			object thisLock = Profiler.m_thisLock;
			lock (thisLock)
			{
				Profiler.FunctionProfilerEntity functionProfilerEntity;
				if (!moduleProfilerContext.funcEntity.TryGetValue(funcName, out functionProfilerEntity))
				{
					functionProfilerEntity = new Profiler.FunctionProfilerEntity();
					functionProfilerEntity.funcName = funcName;
					moduleProfilerContext.funcEntity.Add(funcName, functionProfilerEntity);
				}
				functionProfilerEntity.callCount++;
				functionProfilerEntity.totalExecutionTime += duration;
				functionProfilerEntity.executionTime = duration;
				if (functionProfilerEntity.executionTime > functionProfilerEntity.maxExecutionTime)
				{
					functionProfilerEntity.maxExecutionTime = functionProfilerEntity.executionTime;
				}
			}
		}

		// Token: 0x170003F5 RID: 1013
		// (get) Token: 0x06002B73 RID: 11123 RVA: 0x000BC174 File Offset: 0x000BA574
		// (set) Token: 0x06002B74 RID: 11124 RVA: 0x000BC17B File Offset: 0x000BA57B
		public static Profiler.EMode Mode
		{
			get
			{
				return Profiler.m_mode;
			}
			set
			{
				Profiler.m_mode = value;
			}
		}

		// Token: 0x170003F6 RID: 1014
		// (get) Token: 0x06002B75 RID: 11125 RVA: 0x000BC183 File Offset: 0x000BA583
		// (set) Token: 0x06002B76 RID: 11126 RVA: 0x000BC18A File Offset: 0x000BA58A
		public static int Freq
		{
			get
			{
				return Profiler.m_freq;
			}
			set
			{
				Profiler.m_freq = value;
			}
		}

		// Token: 0x06002B77 RID: 11127 RVA: 0x000BC194 File Offset: 0x000BA594
		private static void Update(object state)
		{
			for (;;)
			{
				Thread.Sleep(1000 / Profiler.m_freq);
				if (Profiler.m_mode != Profiler.EMode.NONE)
				{
					using (new ThreadTracker.Tracker())
					{
						Console.Clear();
						Console.CursorTop = 0;
						Console.WriteLine("Profiler mode: {0}", Enum.GetName(typeof(Profiler.EMode), Profiler.m_mode));
						Console.WriteLine(string.Empty);
						for (int i = 0; i < 3; i++)
						{
							Profiler.ModuleProfilerContext moduleProfilerContext = Profiler.m_modulesCtx[i];
							Console.ForegroundColor = ConsoleColor.White;
							Console.WriteLine("Module: {0}", Enum.GetName(typeof(Profiler.EModule), moduleProfilerContext.module));
							Console.ResetColor();
							object thisLock = Profiler.m_thisLock;
							List<Profiler.FunctionProfilerEntity> list;
							lock (thisLock)
							{
								list = new List<Profiler.FunctionProfilerEntity>(moduleProfilerContext.funcEntity.Values);
							}
							switch (Profiler.m_mode)
							{
							case Profiler.EMode.TOP_FUNCTION_EXECUTION:
								list.Sort(Profiler.FunctionProfilerEntity.ExecutionComparison);
								break;
							case Profiler.EMode.TOP_CALLS:
								list.Sort(Profiler.FunctionProfilerEntity.CallCountComparison);
								break;
							case Profiler.EMode.TOP_TOTAL_EXECUTION:
								list.Sort(Profiler.FunctionProfilerEntity.TotalExecutionComparison);
								break;
							default:
								list.Sort(Profiler.FunctionProfilerEntity.ExecutionComparison);
								break;
							}
							foreach (Profiler.FunctionProfilerEntity functionProfilerEntity in list)
							{
								Console.CursorLeft = 0;
								Console.Write("{0}", functionProfilerEntity.funcName);
								Console.CursorLeft = 27;
								Console.Write("call: {0}", functionProfilerEntity.callCount);
								Console.CursorLeft = 37;
								Console.Write("time: {0:N2}", functionProfilerEntity.executionTime);
								Console.CursorLeft = 52;
								Console.Write("max: {0:N2}", functionProfilerEntity.maxExecutionTime);
								Console.CursorLeft = 65;
								Console.WriteLine("total: {0:N2}", functionProfilerEntity.totalExecutionTime);
							}
							Console.WriteLine(string.Empty);
						}
						Profiler.SystemInfoUpdate();
					}
				}
			}
		}

		// Token: 0x06002B78 RID: 11128 RVA: 0x000BC41C File Offset: 0x000BA81C
		private static void SystemInfoUpdate()
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("System information");
			Console.ResetColor();
			int num;
			int num2;
			ThreadPool.GetMaxThreads(out num, out num2);
			Console.WriteLine("Thread pool max worker jobs: {0}", num);
			ThreadPool.GetMinThreads(out num, out num2);
			Console.WriteLine("Thread pool min worker jobs: {0}", num);
			ThreadPool.GetAvailableThreads(out num, out num2);
			Console.WriteLine("Thread pool available jobs: {0}", num);
		}

		// Token: 0x0400172C RID: 5932
		private static Thread m_profilerThread;

		// Token: 0x0400172D RID: 5933
		private static Profiler.EMode m_mode;

		// Token: 0x0400172E RID: 5934
		private static int m_freq;

		// Token: 0x0400172F RID: 5935
		private static Profiler.ModuleProfilerContext[] m_modulesCtx = new Profiler.ModuleProfilerContext[3];

		// Token: 0x04001730 RID: 5936
		private static object m_thisLock = new object();

		// Token: 0x04001731 RID: 5937
		[CompilerGenerated]
		private static ParameterizedThreadStart <>f__mg$cache0;

		// Token: 0x02000834 RID: 2100
		public enum EModule
		{
			// Token: 0x04001733 RID: 5939
			BASE_QUERY,
			// Token: 0x04001734 RID: 5940
			PROGRAM,
			// Token: 0x04001735 RID: 5941
			TELEMETRY,
			// Token: 0x04001736 RID: 5942
			MODULE_LAST
		}

		// Token: 0x02000835 RID: 2101
		public enum EMode
		{
			// Token: 0x04001738 RID: 5944
			NONE,
			// Token: 0x04001739 RID: 5945
			TOP_FUNCTION_EXECUTION,
			// Token: 0x0400173A RID: 5946
			TOP_CALLS,
			// Token: 0x0400173B RID: 5947
			TOP_TOTAL_EXECUTION
		}

		// Token: 0x02000836 RID: 2102
		private struct ModuleProfilerContext
		{
			// Token: 0x0400173C RID: 5948
			public Profiler.EModule module;

			// Token: 0x0400173D RID: 5949
			public Dictionary<string, Profiler.FunctionProfilerEntity> funcEntity;
		}

		// Token: 0x02000837 RID: 2103
		private class FunctionProfilerEntity : IComparable<Profiler.FunctionProfilerEntity>
		{
			// Token: 0x06002B7B RID: 11131 RVA: 0x000BC4A9 File Offset: 0x000BA8A9
			public int CompareTo(Profiler.FunctionProfilerEntity other)
			{
				return this.executionTime.CompareTo(other.executionTime);
			}

			// Token: 0x0400173E RID: 5950
			public string funcName;

			// Token: 0x0400173F RID: 5951
			public int callCount;

			// Token: 0x04001740 RID: 5952
			public double totalExecutionTime;

			// Token: 0x04001741 RID: 5953
			public double executionTime;

			// Token: 0x04001742 RID: 5954
			public double maxExecutionTime;

			// Token: 0x04001743 RID: 5955
			public static Comparison<Profiler.FunctionProfilerEntity> CallCountComparison = (Profiler.FunctionProfilerEntity p1, Profiler.FunctionProfilerEntity p2) => p2.callCount.CompareTo(p1.callCount);

			// Token: 0x04001744 RID: 5956
			public static Comparison<Profiler.FunctionProfilerEntity> ExecutionComparison = (Profiler.FunctionProfilerEntity p1, Profiler.FunctionProfilerEntity p2) => p2.executionTime.CompareTo(p1.executionTime);

			// Token: 0x04001745 RID: 5957
			public static Comparison<Profiler.FunctionProfilerEntity> TotalExecutionComparison = (Profiler.FunctionProfilerEntity p1, Profiler.FunctionProfilerEntity p2) => p2.totalExecutionTime.CompareTo(p1.totalExecutionTime);
		}
	}
}
