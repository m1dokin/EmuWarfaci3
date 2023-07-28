using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using command_console;
using HK2Net;
using HK2Net.Scanners;
using MasterServer.Core;
using MasterServer.Core.Application.Bootstrap;
using MasterServer.Core.Configs;
using MasterServer.Core.Kernel.Scanners;
using MasterServer.Core.Logging.Targets;
using MasterServer.Core.Services.Configuration;
using NLog;
using NLog.Config;
using Util.CommandLine;
using Util.Common;

namespace MasterServer
{
	// Token: 0x02000838 RID: 2104
	internal static class Program
	{
		// Token: 0x06002B80 RID: 11136 RVA: 0x000BC52C File Offset: 0x000BA92C
		private static List<string> ParseCommandLine(string[] args)
		{
			ConsoleCmdLine consoleCmdLine = new ConsoleCmdLine();
			Program.cp_help = consoleCmdLine.GetParameter("help");
			Program.cp_bootstrap = new CmdLineParameter("bts", "bootstrap", false, "bootstrap configuration name");
			Program.cp_devmode = new CmdLineParameter("dm", "devmode", false, "use development environment rules to locate resources", true);
			Program.cp_version = new CmdLineParameter("version", false, "override default version");
			Program.cp_xmpp_resource = new CmdLineParameter("xmpp_resource", false, "override xmpp resource");
			Program.cp_forceupdatedb = new CmdLineParameter("f", "forceupdatedb", false, "recreate DB structure anew", true);
			Program.cp_daemon = new CmdLineParameter("d", "daemon", false, "indicates that MS is run in daemon mode", true);
			Program.cp_server_id = new CmdLineParameter("server_id", false, "sequential server identifier");
			Program.cp_server_name = new CmdLineParameter("server", false, "server name");
			Program.cp_channel = new CmdLineParameter("channel", false, "server channel type");
			Program.cp_rconport = new CmdLineParameter("rconport", false, "override rcon port");
			Program.cp_giport = new CmdLineParameter("gi_port", false, "start GI service on specified port");
			Program.cp_exec_recorder = new CmdLineParameter("e", "exec_recorder", false, "records all execution for stress-testing", true);
			Program.cp_stress_test = new CmdLineParameter("p", "stress_test", false, "stress-testing mode", true);
			Program.cp_db_updater = new CmdLineParameter("db", "db_updater", false, "gives server the right to update DB", true);
			Program.cp_realm_db_updater = new CmdLineParameter("rlm", "realm_db_updater", false, "gives server the right to update realm shared data", true);
			Program.cp_telem_agg = new CmdLineParameter("agg", "telem_agg", false, "enable telemetry aggregation tasks", true);
			Program.cp_enable_debug_queries = new CmdLineParameter("dbg", "enable_debug_queries", false, "Enables the usage of debug queries", true);
			Program.cp_log_rotate_mode = new CmdLineParameter("lr", "log_rotate_mode", false, "Enable log rotate mode for log system", true);
			Program.cp_db_updater_reset = new CmdLineParameter("dbr", "db_updater_reset", false, "Reset db updater table lock", true);
			Program.cp_db_ecat_updater_reset = new CmdLineParameter("dbecr", "db_ecat_updater_reset", false, "Reset db updater table lock for ecatalog", true);
			Program.cp_use_realm_missions = new CmdLineParameter("urm", "use_realm_missions", false, "use realm storage for missions");
			Program.cp_metrics_enabled = new CmdLineParameter("mtx", "metrics_enabled", false, "overrides metrics enabled/disabled settings");
			Program.cp_dbg_cache_services = new CmdLineParameter("chs", "dbg_cache_services", false, "enables/disables service resolution cache", true);
			Program.cp_verbose_level = new CmdLineParameter("vl", "verbose_level", false, "Set initial verbose level", true);
			Program.cp_enable_test_content = new CmdLineParameter("tstcnt", "enable_test_content", false, "Enables the usage of test content", true);
			Program.cp_use_mono_rw_lock = new CmdLineParameter("use_mono_rw_lock", false, "Enables the usage of Mono ReaderWriterLockSlim implementation, otherwise use MS one.", true);
			consoleCmdLine.RegisterParameter(new CmdLineParameter[]
			{
				Program.cp_devmode,
				Program.cp_bootstrap,
				Program.cp_version,
				Program.cp_xmpp_resource,
				Program.cp_forceupdatedb,
				Program.cp_daemon,
				Program.cp_server_id,
				Program.cp_server_name,
				Program.cp_channel,
				Program.cp_rconport,
				Program.cp_giport,
				Program.cp_exec_recorder,
				Program.cp_stress_test,
				Program.cp_db_updater,
				Program.cp_realm_db_updater,
				Program.cp_telem_agg,
				Program.cp_enable_debug_queries,
				Program.cp_log_rotate_mode,
				Program.cp_db_updater_reset,
				Program.cp_db_ecat_updater_reset,
				Program.cp_use_realm_missions,
				Program.cp_metrics_enabled,
				Program.cp_dbg_cache_services,
				Program.cp_verbose_level,
				Program.cp_enable_test_content,
				Program.cp_use_mono_rw_lock
			});
			List<CmdLine.AgrumentError> list = consoleCmdLine.Parse(args, false);
			if (Program.cp_verbose_level.Exists)
			{
				Log.SetVerboseGroup(Program.cp_verbose_level.Value);
			}
			List<string> list2 = new List<string>();
			foreach (CmdLine.AgrumentError agrumentError in list)
			{
				list2.Add(agrumentError.parameter);
			}
			return list2;
		}

		// Token: 0x06002B81 RID: 11137 RVA: 0x000BC948 File Offset: 0x000BAD48
		private static void InitResources()
		{
			Resources.IsDevMode = Program.cp_devmode.Exists;
			Resources.IsForceUpdateDatabase = Program.cp_forceupdatedb.Exists;
			Resources.IsDaemon = Program.cp_daemon.Exists;
			Resources.RecordExecution = Program.cp_exec_recorder.Exists;
			Resources.StressTestMode = Program.cp_stress_test.Exists;
			Resources.DBUpdaterPermission = Program.cp_db_updater.Exists;
			Resources.RealmDBUpdaterPermission = Program.cp_realm_db_updater.Exists;
			Resources.AggregationEnabled = Program.cp_telem_agg.Exists;
			Resources.DebugQueriesEnabled = Program.cp_enable_debug_queries.Exists;
			Resources.DebugGameModeSettingsEnabled = Program.cp_enable_debug_queries.Exists;
			Resources.DBUpdaterPermissionReset = Program.cp_db_updater_reset.Exists;
			Resources.DBUpdaterEcatPermissionReset = Program.cp_db_ecat_updater_reset.Exists;
			Resources.UseRealmMissions = (!Program.cp_use_realm_missions.Exists || Program.cp_use_realm_missions.Value != "0");
			Resources.DebugContentEnabled = Program.cp_enable_test_content.Exists;
			if (Program.cp_version.Exists)
			{
				Resources.MasterVersion = Program.cp_version.Value;
			}
			Resources.MonoVersion = AppUtils.GetRuntimeVersion();
			if (Program.cp_server_name.Exists)
			{
				Resources.ServerName = Program.cp_server_name.Value;
				Resources.XmppResource = ((!Program.cp_xmpp_resource.Exists) ? Program.cp_server_name.Value : Program.cp_xmpp_resource.Value);
			}
			if (Program.cp_rconport.Exists)
			{
				Resources.RConPort.Init(Program.cp_rconport.Value);
			}
			if (Program.cp_giport.Exists)
			{
				Resources.GIPort.Init(Program.cp_giport.Value);
			}
			if (Program.cp_channel.Exists)
			{
				Resources.Channel = ReflectionUtils.EnumParse<Resources.ChannelType>(Program.cp_channel.Value);
			}
			if (Program.cp_server_id.Exists)
			{
				Resources.ServerID = int.Parse(Program.cp_server_id.Value);
			}
			else
			{
				Match match = Regex.Match(Resources.ServerName, ".*_([0-9]+)$");
				if (!string.IsNullOrEmpty(match.Groups[1].Value))
				{
					int num = int.Parse(match.Groups[1].Value);
					Resources.ServerID = (int)(Resources.Channel * (Resources.ChannelType)100 + num);
				}
			}
			if (Program.cp_metrics_enabled.Exists)
			{
				Resources.MetricsEnabled = int.Parse(Program.cp_metrics_enabled.Value);
			}
			Resources.DbgCacheServices = Program.cp_dbg_cache_services.Exists;
			Resources.UseMonoRWLock = Program.cp_use_mono_rw_lock.Exists;
		}

		// Token: 0x06002B82 RID: 11138 RVA: 0x000BCBDC File Offset: 0x000BAFDC
		private static bool CheckParams()
		{
			if (Resources.IsDaemon)
			{
				if (Resources.IsForceUpdateDatabase)
				{
					Log.Warning("Database update mode isn't allowed in daemon mode. Run again as live application. Quit.");
					return false;
				}
				Log.Info("Master server application started as a daemon");
			}
			else
			{
				Log.Info("Master server application started. Type \"quit\" to quit it");
			}
			if (Resources.RealmDBUpdaterPermission && !Resources.DBUpdaterPermission)
			{
				Log.Warning("Realm DB updater cannot be started without DB updater permission. Quit.");
				return false;
			}
			if (Resources.IsDevMode)
			{
				Log.Info("Master server is run in developer mode");
			}
			else
			{
				Log.Info("Master server is run in user mode");
			}
			Log.Info<string>("Using resources from folder '{0}'", Resources.GetResourcesDirectory());
			if (!Resources.CheckResources())
			{
				if (Resources.IsDevMode)
				{
					Log.Error("Failed to verify MasterServerApp resources. Please sync with source control. Quit.");
				}
				else
				{
					Log.Error("Failed to verify MasterServerApp resources. Please reinstall application. Quit.");
				}
				return false;
			}
			Log.Info<string>("Command line: {0}", Environment.CommandLine);
			return true;
		}

		// Token: 0x06002B83 RID: 11139 RVA: 0x000BCCB4 File Offset: 0x000BB0B4
		public static void Main(string[] args)
		{
			CultureHelpers.SetNeutralThreadCulture();
			string applicationVersion = AppUtils.GetApplicationVersion();
			Console.Title = "MasterServer " + applicationVersion;
			List<string> startupConsoleCmds = Program.ParseCommandLine(args);
			if (Program.cp_help.Exists)
			{
				return;
			}
			IConsole console = ConsoleFactory.Create((!Program.cp_devmode.Exists) ? ConsoleFactory.Type.Proxy : ConsoleFactory.Type.Command);
			console.Init(ConsoleColor.Green);
			EventTarget eventTarget = new EventTarget();
			eventTarget.Name = "event";
			eventTarget.Layout = LogManager.Configuration.Variables["log-format"].Text;
			LogManager.Configuration.AddTarget("event", eventTarget);
			LoggingRule item = new LoggingRule("*", LogLevel.Info, eventTarget);
			LogManager.Configuration.LoggingRules.Add(item);
			LogManager.Configuration.Reload();
			Resources.XmppResource = applicationVersion;
			Resources.MasterVersion = applicationVersion;
			Program.InitResources();
			Resources.StartupConsoleCmds = startupConsoleCmds;
			Resources.Init(Resources.IsDevMode);
			AppDomain currentDomain = AppDomain.CurrentDomain;
			if (Program.<>f__mg$cache0 == null)
			{
				Program.<>f__mg$cache0 = new UnhandledExceptionEventHandler(Program.OnUnhandledException);
			}
			currentDomain.UnhandledException += Program.<>f__mg$cache0;
			if (Program.<>f__mg$cache1 == null)
			{
				Program.<>f__mg$cache1 = new EventHandler<UnobservedTaskExceptionEventArgs>(Program.OnUnhandledException);
			}
			TaskScheduler.UnobservedTaskException += Program.<>f__mg$cache1;
			Log.Init();
			Log.Info("Log started");
			Log.Info<string>("Mono version: {0}", Resources.MonoVersion);
			Resources.InitBootstrap(Program.cp_bootstrap.Exists, Program.cp_bootstrap.Value);
			BootstrapFilter bootstrapFilter = new BootstrapFilter(Resources.BootstrapName);
			BootstrapScanner bootstrap = BootstrapScanner.GetBootstrap(Resources.BootstrapName);
			bootstrap.Configure(bootstrapFilter);
			if (!Program.CheckParams())
			{
				return;
			}
			IScanner[] scanners = new IScanner[]
			{
				new AppDomainAttributeScanner(),
				new MsAssemblyScanner(),
				new QueryHandlerScanner()
			};
			Action<IHabitat> register = delegate(IHabitat h)
			{
				h.AddServiceByContract<IConfigProfileProvider, ConfigProfileProvider>(new ConfigProfileProvider(bootstrapFilter.ConfigProfiles));
			};
			Action<IHabitat> postRegister = delegate(IHabitat h)
			{
				Resources.Configure(h.GetByContract<IConfigurationService>());
			};
			IHabitat habitat = HabitatFactory.NewHabitat(scanners, new IFilter[]
			{
				bootstrapFilter
			}, register, postRegister);
			Log.Info<string, int>("MasterServerTpl version {0} realm id {1}", Resources.MasterVersion, Resources.RealmId);
			ServicesManager servicesManager = new ServicesManager(habitat);
			servicesManager.Init();
		}

		// Token: 0x06002B84 RID: 11140 RVA: 0x000BCEF8 File Offset: 0x000BB2F8
		private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Log.Error("Unhandled exception, terminating MasterServer");
			Log.Error(e.ExceptionObject as Exception);
			if (e.ExceptionObject is ReflectionTypeLoadException)
			{
				ReflectionTypeLoadException ex = (ReflectionTypeLoadException)e.ExceptionObject;
				foreach (Exception e2 in ex.LoaderExceptions)
				{
					Log.Error(e2);
				}
			}
		}

		// Token: 0x06002B85 RID: 11141 RVA: 0x000BCF60 File Offset: 0x000BB360
		private static void OnUnhandledException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
		{
			unobservedTaskExceptionEventArgs.SetObserved();
			Log.Error("Unhandled task exception.");
			Log.Error(unobservedTaskExceptionEventArgs.Exception);
		}

		// Token: 0x04001746 RID: 5958
		private static CmdLineParameter cp_help;

		// Token: 0x04001747 RID: 5959
		private static CmdLineParameter cp_bootstrap;

		// Token: 0x04001748 RID: 5960
		private static CmdLineParameter cp_devmode;

		// Token: 0x04001749 RID: 5961
		private static CmdLineParameter cp_xmpp_resource;

		// Token: 0x0400174A RID: 5962
		private static CmdLineParameter cp_version;

		// Token: 0x0400174B RID: 5963
		private static CmdLineParameter cp_forceupdatedb;

		// Token: 0x0400174C RID: 5964
		private static CmdLineParameter cp_daemon;

		// Token: 0x0400174D RID: 5965
		private static CmdLineParameter cp_server_id;

		// Token: 0x0400174E RID: 5966
		private static CmdLineParameter cp_server_name;

		// Token: 0x0400174F RID: 5967
		private static CmdLineParameter cp_channel;

		// Token: 0x04001750 RID: 5968
		private static CmdLineParameter cp_rconport;

		// Token: 0x04001751 RID: 5969
		private static CmdLineParameter cp_giport;

		// Token: 0x04001752 RID: 5970
		private static CmdLineParameter cp_exec_recorder;

		// Token: 0x04001753 RID: 5971
		private static CmdLineParameter cp_stress_test;

		// Token: 0x04001754 RID: 5972
		private static CmdLineParameter cp_db_updater;

		// Token: 0x04001755 RID: 5973
		private static CmdLineParameter cp_realm_db_updater;

		// Token: 0x04001756 RID: 5974
		private static CmdLineParameter cp_telem_agg;

		// Token: 0x04001757 RID: 5975
		private static CmdLineParameter cp_enable_debug_queries;

		// Token: 0x04001758 RID: 5976
		private static CmdLineParameter cp_log_rotate_mode;

		// Token: 0x04001759 RID: 5977
		private static CmdLineParameter cp_db_updater_reset;

		// Token: 0x0400175A RID: 5978
		private static CmdLineParameter cp_db_ecat_updater_reset;

		// Token: 0x0400175B RID: 5979
		private static CmdLineParameter cp_use_realm_missions;

		// Token: 0x0400175C RID: 5980
		private static CmdLineParameter cp_metrics_enabled;

		// Token: 0x0400175D RID: 5981
		private static CmdLineParameter cp_verbose_level;

		// Token: 0x0400175E RID: 5982
		private static CmdLineParameter cp_enable_test_content;

		// Token: 0x0400175F RID: 5983
		private static CmdLineParameter cp_use_mono_rw_lock;

		// Token: 0x04001760 RID: 5984
		private static CmdLineParameter cp_dbg_cache_services;

		// Token: 0x04001761 RID: 5985
		[CompilerGenerated]
		private static UnhandledExceptionEventHandler <>f__mg$cache0;

		// Token: 0x04001762 RID: 5986
		[CompilerGenerated]
		private static EventHandler<UnobservedTaskExceptionEventArgs> <>f__mg$cache1;
	}
}
