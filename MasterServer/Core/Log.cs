using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using MasterServer.Core.Services;
using MasterServer.ServerInfo;
using MasterServer.Users;
using NLog;
using Util.Common;

namespace MasterServer.Core
{
	// Token: 0x02000822 RID: 2082
	public static class Log
	{
		// Token: 0x140000BE RID: 190
		// (add) Token: 0x06002AD7 RID: 10967 RVA: 0x000B983C File Offset: 0x000B7C3C
		// (remove) Token: 0x06002AD8 RID: 10968 RVA: 0x000B9870 File Offset: 0x000B7C70
		public static event Action<string> OnLogMessage;

		// Token: 0x06002AD9 RID: 10969 RVA: 0x000B98A4 File Offset: 0x000B7CA4
		public static void Error(Exception e)
		{
			Log.Logger.Error<Exception>(e);
		}

		// Token: 0x06002ADA RID: 10970 RVA: 0x000B98B1 File Offset: 0x000B7CB1
		public static void Warning(Exception e)
		{
			Log.Logger.Warn<Exception>(e);
		}

		// Token: 0x06002ADB RID: 10971 RVA: 0x000B98BE File Offset: 0x000B7CBE
		public static void Error(string format)
		{
			Log.Logger.Error(format);
		}

		// Token: 0x06002ADC RID: 10972 RVA: 0x000B98CB File Offset: 0x000B7CCB
		public static void Error<T>(string format, T p)
		{
			Log.Logger.Error<T>(format, p);
		}

		// Token: 0x06002ADD RID: 10973 RVA: 0x000B98D9 File Offset: 0x000B7CD9
		public static void Error<T1, T2>(string format, T1 p1, T2 p2)
		{
			Log.Logger.Error<T1, T2>(format, p1, p2);
		}

		// Token: 0x06002ADE RID: 10974 RVA: 0x000B98E8 File Offset: 0x000B7CE8
		public static void Error<T1, T2, T3>(string format, T1 p1, T2 p2, T3 p3)
		{
			Log.Logger.Error<T1, T2, T3>(format, p1, p2, p3);
		}

		// Token: 0x06002ADF RID: 10975 RVA: 0x000B98F8 File Offset: 0x000B7CF8
		public static void Error(string format, object arg1)
		{
			Log.Logger.Error(format, arg1);
		}

		// Token: 0x06002AE0 RID: 10976 RVA: 0x000B9906 File Offset: 0x000B7D06
		public static void Error(string format, object arg1, object arg2)
		{
			Log.Logger.Error(format, arg1, arg2);
		}

		// Token: 0x06002AE1 RID: 10977 RVA: 0x000B9915 File Offset: 0x000B7D15
		public static void Error(string format, params object[] args)
		{
			Log.Logger.Error(format, args);
		}

		// Token: 0x06002AE2 RID: 10978 RVA: 0x000B9923 File Offset: 0x000B7D23
		public static void Warning(string format)
		{
			Log.Logger.Warn(format);
		}

		// Token: 0x06002AE3 RID: 10979 RVA: 0x000B9930 File Offset: 0x000B7D30
		public static void Warning<T>(string format, T p)
		{
			Log.Logger.Warn<T>(format, p);
		}

		// Token: 0x06002AE4 RID: 10980 RVA: 0x000B993E File Offset: 0x000B7D3E
		public static void Warning<T1, T2>(string format, T1 p1, T2 p2)
		{
			Log.Logger.Warn<T1, T2>(format, p1, p2);
		}

		// Token: 0x06002AE5 RID: 10981 RVA: 0x000B994D File Offset: 0x000B7D4D
		public static void Warning<T1, T2, T3>(string format, T1 p1, T2 p2, T3 p3)
		{
			Log.Logger.Warn<T1, T2, T3>(format, p1, p2, p3);
		}

		// Token: 0x06002AE6 RID: 10982 RVA: 0x000B995D File Offset: 0x000B7D5D
		public static void Warning(string format, object arg1)
		{
			Log.Logger.Warn(format, arg1);
		}

		// Token: 0x06002AE7 RID: 10983 RVA: 0x000B996B File Offset: 0x000B7D6B
		public static void Warning(string format, object arg1, object arg2)
		{
			Log.Logger.Warn(format, arg1, arg2);
		}

		// Token: 0x06002AE8 RID: 10984 RVA: 0x000B997A File Offset: 0x000B7D7A
		public static void Warning(string format, params object[] args)
		{
			Log.Logger.Warn(format, args);
		}

		// Token: 0x06002AE9 RID: 10985 RVA: 0x000B9988 File Offset: 0x000B7D88
		public static void Info(string format)
		{
			Log.Logger.Info(format);
		}

		// Token: 0x06002AEA RID: 10986 RVA: 0x000B9995 File Offset: 0x000B7D95
		public static void Info<T>(string format, T p)
		{
			Log.Logger.Info<T>(format, p);
		}

		// Token: 0x06002AEB RID: 10987 RVA: 0x000B99A3 File Offset: 0x000B7DA3
		public static void Info<T1, T2>(string format, T1 p1, T2 p2)
		{
			Log.Logger.Info<T1, T2>(format, p1, p2);
		}

		// Token: 0x06002AEC RID: 10988 RVA: 0x000B99B2 File Offset: 0x000B7DB2
		public static void Info<T1, T2, T3>(string format, T1 p1, T2 p2, T3 p3)
		{
			Log.Logger.Info<T1, T2, T3>(format, p1, p2, p3);
		}

		// Token: 0x06002AED RID: 10989 RVA: 0x000B99C2 File Offset: 0x000B7DC2
		public static void Info(string format, object arg1)
		{
			Log.Logger.Info(format, arg1);
		}

		// Token: 0x06002AEE RID: 10990 RVA: 0x000B99D0 File Offset: 0x000B7DD0
		public static void Info(string format, object arg1, object arg2)
		{
			Log.Logger.Info(format, arg1, arg2);
		}

		// Token: 0x06002AEF RID: 10991 RVA: 0x000B99DF File Offset: 0x000B7DDF
		public static void Info(string format, params object[] args)
		{
			Log.Logger.Info(format, args);
		}

		// Token: 0x06002AF0 RID: 10992 RVA: 0x000B99ED File Offset: 0x000B7DED
		public static void Verbose(string format, params object[] args)
		{
			Log.Verbose(Log.Group.Common, format, args);
		}

		// Token: 0x06002AF1 RID: 10993 RVA: 0x000B99F7 File Offset: 0x000B7DF7
		public static void Verbose(Log.Group g, string format, params object[] args)
		{
			if (Log.CheckVerbose(g))
			{
				Log.Logger.Debug(string.Format("[{0}] ", g) + format, args);
			}
		}

		// Token: 0x06002AF2 RID: 10994 RVA: 0x000B9A25 File Offset: 0x000B7E25
		public static void Init()
		{
			if (Log.<>f__mg$cache0 == null)
			{
				Log.<>f__mg$cache0 = new TimerCallback(Log.TimerProc);
			}
			Log.m_keepAliveTimer = new Timer(Log.<>f__mg$cache0);
			Log.m_keepAliveTimer.Change(0, 60000);
		}

		// Token: 0x170003EC RID: 1004
		// (get) Token: 0x06002AF3 RID: 10995 RVA: 0x000B9A5F File Offset: 0x000B7E5F
		// (set) Token: 0x06002AF4 RID: 10996 RVA: 0x000B9A66 File Offset: 0x000B7E66
		public static Log.Group VerboseGroup
		{
			get
			{
				return Log.m_verboseGroup;
			}
			private set
			{
				Log.m_verboseGroup = value;
			}
		}

		// Token: 0x06002AF5 RID: 10997 RVA: 0x000B9A6E File Offset: 0x000B7E6E
		private static bool CheckVerbose(Log.Group g)
		{
			return (Log.m_verboseGroup & g) != Log.Group.None || Log.Logger.IsTraceEnabled;
		}

		// Token: 0x06002AF6 RID: 10998 RVA: 0x000B9A8C File Offset: 0x000B7E8C
		public static void SetVerboseGroup(string groupName)
		{
			Log.Group verboseGroup;
			if (!ReflectionUtils.EnumParseFlags<Log.Group>(groupName, out verboseGroup))
			{
				Log.Warning<string>("Incorrect verbose level {0}", groupName);
				return;
			}
			Log.VerboseGroup = verboseGroup;
		}

		// Token: 0x06002AF7 RID: 10999 RVA: 0x000B9AB8 File Offset: 0x000B7EB8
		public static void RaiseOnLogMessageEvent(string line)
		{
			Log.OnLogMessage.SafeInvoke(line);
		}

		// Token: 0x06002AF8 RID: 11000 RVA: 0x000B9AC8 File Offset: 0x000B7EC8
		private static void TimerProc(object state)
		{
			Log.WriteApplicationMemoryUsageToConsole();
			Process currentProcess = Process.GetCurrentProcess();
			if (Log.MemoryUsagePanic > 0L && currentProcess.VirtualMemorySize64 >= Log.MemoryUsagePanic)
			{
				Log.WriteComponentsMemoryUsageToConsole();
				Log.Info("Trying to collect garbage due to high memory usage");
				GC.Collect();
				GC.WaitForPendingFinalizers();
				Log.WriteApplicationMemoryUsageToConsole();
			}
		}

		// Token: 0x06002AF9 RID: 11001 RVA: 0x000B9B1C File Offset: 0x000B7F1C
		private static int GetOnlineUserCount()
		{
			if (ServicesManager.ExecutionPhase == ExecutionPhase.Started)
			{
				IUserRepository service = ServicesManager.GetService<IUserRepository>();
				return service.GetOnlineUsersCount();
			}
			return 0;
		}

		// Token: 0x06002AFA RID: 11002 RVA: 0x000B9B44 File Offset: 0x000B7F44
		private static long GetSentLogEventsCount()
		{
			if (ServicesManager.ExecutionPhase == ExecutionPhase.Started)
			{
				ILogService service = ServicesManager.GetService<ILogService>();
				return service.EventCount;
			}
			return 0L;
		}

		// Token: 0x06002AFB RID: 11003 RVA: 0x000B9B6C File Offset: 0x000B7F6C
		private static void WriteApplicationMemoryUsageToConsole()
		{
			Process currentProcess = Process.GetCurrentProcess();
			StringBuilder stringBuilder = new StringBuilder(200);
			stringBuilder.AppendFormat("TimerProc: ServerTpl is alive, Working set {0}Mb Paged memory size {1}Mb Virtual memory size {2}Mb GC memory {3}Mb(gc:{4}) Online users {5} Sent log events {6} Mono version: {7}", new object[]
			{
				currentProcess.WorkingSet64 / 1024L / 1024L,
				currentProcess.PagedMemorySize64 / 1024L / 1024L,
				currentProcess.VirtualMemorySize64 / 1024L / 1024L,
				GC.GetTotalMemory(false) / 1024L / 1024L,
				GC.CollectionCount(0),
				Log.GetOnlineUserCount(),
				Log.GetSentLogEventsCount(),
				Resources.MonoVersion
			});
			string message = stringBuilder.ToString();
			Log.Logger.Info(message);
		}

		// Token: 0x06002AFC RID: 11004 RVA: 0x000B9C50 File Offset: 0x000B8050
		public static void WriteComponentsMemoryUsageToConsole()
		{
			StringBuilder stringBuilder = new StringBuilder();
			IMemoryUsageCollector service = ServicesManager.GetService<IMemoryUsageCollector>();
			foreach (IServiceModule o in ServicesManager.GetAllServices())
			{
				string memoryUsageInfo = service.GetMemoryUsageInfo(o);
				if (!string.IsNullOrEmpty(memoryUsageInfo))
				{
					stringBuilder.AppendFormat("\n{0}", memoryUsageInfo);
				}
			}
			Log.Info(stringBuilder.ToString());
		}

		// Token: 0x040016DB RID: 5851
		public static long MemoryUsagePanic = 0L;

		// Token: 0x040016DC RID: 5852
		private const int KEEP_ALIVE_INTERVAL_MSEC = 60000;

		// Token: 0x040016DD RID: 5853
		private static Timer m_keepAliveTimer;

		// Token: 0x040016DE RID: 5854
		private static Log.Group m_verboseGroup;

		// Token: 0x040016E0 RID: 5856
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		// Token: 0x040016E1 RID: 5857
		[CompilerGenerated]
		private static TimerCallback <>f__mg$cache0;

		// Token: 0x02000823 RID: 2083
		[Flags]
		public enum Group
		{
			// Token: 0x040016E3 RID: 5859
			None = 0,
			// Token: 0x040016E4 RID: 5860
			Common = 1,
			// Token: 0x040016E5 RID: 5861
			GameRoom = 2,
			// Token: 0x040016E6 RID: 5862
			P2P = 4,
			// Token: 0x040016E7 RID: 5863
			Ping = 8,
			// Token: 0x040016E8 RID: 5864
			LogServer = 16,
			// Token: 0x040016E9 RID: 5865
			SessionTelemetry = 32,
			// Token: 0x040016EA RID: 5866
			Telemetry = 64,
			// Token: 0x040016EB RID: 5867
			SessionStorage = 128,
			// Token: 0x040016EC RID: 5868
			ColdStorage = 256,
			// Token: 0x040016ED RID: 5869
			Missions = 512,
			// Token: 0x040016EE RID: 5870
			Metrics = 1024,
			// Token: 0x040016EF RID: 5871
			QueryContext = 2048,
			// Token: 0x040016F0 RID: 5872
			Matchmaking = 4096,
			// Token: 0x040016F1 RID: 5873
			CustomRules = 8192,
			// Token: 0x040016F2 RID: 5874
			GFaceAPI = 16384,
			// Token: 0x040016F3 RID: 5875
			Invitation = 32768,
			// Token: 0x040016F4 RID: 5876
			Network = 65536,
			// Token: 0x040016F5 RID: 5877
			GameRoomDiagnostics = 131072,
			// Token: 0x040016F6 RID: 5878
			SessionResults = 262144,
			// Token: 0x040016F7 RID: 5879
			All = 1048575
		}
	}
}
