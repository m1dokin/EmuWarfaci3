using System;
using System.Threading;
using command_console;
using HK2Net;
using MasterServer.Core.Services;
using MasterServer.Core.Services.Jobs;
using MasterServer.Core.Timers;
using MasterServer.CryOnlineNET;
using MasterServer.Database;
using MasterServer.Users;
using Mono.Unix;
using Mono.Unix.Native;
using MySql.Data.MySqlClient;
using Ninject;
using Util.Common;

namespace MasterServer.Core
{
	// Token: 0x02000026 RID: 38
	[Service]
	[Singleton]
	internal class ApplicationService : ServiceModule, IApplicationService
	{
		// Token: 0x06000084 RID: 132 RVA: 0x000069F6 File Offset: 0x00004DF6
		public ApplicationService(IOnlineClient onlineClientSystem, IUserRepository userRepository, IQueryManager queryManager, [Named("NonReentrant")] ITimerFactory timerFactory, IJobSchedulerService jobService)
		{
			this.m_onlineClientSystem = onlineClientSystem;
			this.m_userRepository = userRepository;
			this.m_queryManager = queryManager;
			this.m_timerFactory = timerFactory;
			this.m_jobService = jobService;
		}

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000085 RID: 133 RVA: 0x00006A24 File Offset: 0x00004E24
		// (remove) Token: 0x06000086 RID: 134 RVA: 0x00006A5C File Offset: 0x00004E5C
		public event Action OnShutdownScheduled;

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000087 RID: 135 RVA: 0x00006A92 File Offset: 0x00004E92
		// (set) Token: 0x06000088 RID: 136 RVA: 0x00006A9C File Offset: 0x00004E9C
		public bool IsShutdownScheduled
		{
			get
			{
				return this.m_isShutdownScheduled;
			}
			private set
			{
				this.m_isShutdownScheduled = value;
			}
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00006AA8 File Offset: 0x00004EA8
		public void ScheduleShutdown(TimeSpan timeout)
		{
			if (this.IsShutdownScheduled)
			{
				Log.Warning("Shutdown is already scheduled. Skip this request.");
				return;
			}
			this.IsShutdownScheduled = true;
			this.m_gracefulShutdownContext = new GracefulShutdownContext(this.m_queryManager, this.m_userRepository, this.m_timerFactory, timeout);
			if (this.OnShutdownScheduled != null)
			{
				this.OnShutdownScheduled();
			}
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00006B08 File Offset: 0x00004F08
		public override void Init()
		{
			ServicesManager.OnExecutionPhaseChanged += this.OnExecutionPhaseChanged;
			ThreadPoolProxy.Configure();
			Thread.CurrentThread.Name = "Main";
			Profiler.Init();
			this.RegisterSignalHandler();
			this.m_jobService.AddJob("graceful_auto_shutdown");
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00006B55 File Offset: 0x00004F55
		public override void Stop()
		{
			if (this.m_gracefulShutdownContext != null)
			{
				this.m_gracefulShutdownContext.Dispose();
			}
			Profiler.Close();
			ServicesManager.OnExecutionPhaseChanged -= this.OnExecutionPhaseChanged;
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00006B83 File Offset: 0x00004F83
		private void OnExecutionPhaseChanged(ExecutionPhase executionPhase)
		{
			if (executionPhase == ExecutionPhase.Started)
			{
				this.OnApplicationStarted();
			}
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00006B94 File Offset: 0x00004F94
		private void OnApplicationStarted()
		{
			Log.Info("Resource string used by XMPP: " + Resources.XmppResource);
			Log.Info<int>("Master server id: {0}", Resources.ServerID);
			Log.Info<string>("Master server name: {0}", Resources.ServerName);
			string text = string.Format("Database setup:\n\tMaster: {0} [{1}]", Resources.SqlServerAddr, Resources.SqlDbName);
			foreach (ConnectionPool connectionPool in Resources.SlaveConnectionPools)
			{
				MySqlConnectionStringBuilder mySqlConnectionStringBuilder = new MySqlConnectionStringBuilder(connectionPool.ConnectionConfig.ConnectionString);
				text += string.Format("\n\tSlave: {0} [{1}]", mySqlConnectionStringBuilder.Server, mySqlConnectionStringBuilder.Database);
			}
			Log.Info(text);
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00006C44 File Offset: 0x00005044
		public void Run()
		{
			ConsoleCmdManager.ExecuteCmdLine(Resources.StartupConsoleCmds);
			if (!Resources.IsDaemon)
			{
				IConsole console = ConsoleFactory.Console;
				console.Run(false);
				console.OnCommand += this.InputRoutine;
			}
			this.m_onlineClientSystem.ServiceConnection();
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00006C90 File Offset: 0x00005090
		private void RegisterSignalHandler()
		{
			UnixSignal[] signals = new UnixSignal[]
			{
				new UnixSignal(2),
				new UnixSignal(15)
			};
			new Thread(delegate()
			{
				int num = UnixSignal.WaitAny(signals, -1);
				Signum signum = signals[num].Signum;
				Log.Info<Signum>("Catch signal: {0}", signum);
				ConsoleCmdManager.ExecuteCmd("quit");
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00006CE1 File Offset: 0x000050E1
		private void InputRoutine(string cmd)
		{
			CultureHelpers.SetNeutralThreadCulture();
			ConsoleCmdManager.ExecuteCmd(cmd);
		}

		// Token: 0x0400004C RID: 76
		private readonly IOnlineClient m_onlineClientSystem;

		// Token: 0x0400004D RID: 77
		private readonly IUserRepository m_userRepository;

		// Token: 0x0400004E RID: 78
		private readonly IQueryManager m_queryManager;

		// Token: 0x0400004F RID: 79
		private readonly ITimerFactory m_timerFactory;

		// Token: 0x04000050 RID: 80
		private readonly IJobSchedulerService m_jobService;

		// Token: 0x04000051 RID: 81
		private volatile bool m_isShutdownScheduled;

		// Token: 0x04000052 RID: 82
		private GracefulShutdownContext m_gracefulShutdownContext;
	}
}
