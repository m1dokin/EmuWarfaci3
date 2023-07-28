using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using MasterServer.Common;
using MasterServer.Core.Configs;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Amqp;
using MasterServer.Core.Services.Configuration;
using MasterServer.CryOnlineNET;
using MasterServer.DAL.Utils;
using MasterServer.Database;

namespace MasterServer.Core
{
	// Token: 0x02000839 RID: 2105
	public class Resources
	{
		// Token: 0x06002B88 RID: 11144 RVA: 0x000BCFB4 File Offset: 0x000BB3B4
		public static void Init(bool isDevMode)
		{
			string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			Directory.SetCurrentDirectory(directoryName);
			if (Resources.m_serverName == string.Empty)
			{
				Resources.m_serverName = "msid";
			}
			string path = "../../";
			Resources.m_resPath = Path.Combine(path, "Resources");
			Resources.m_overrideResPath = Path.Combine(Resources.m_resPath, "override");
			Resources.m_root = Path.Combine(path, Resources.m_serverName);
			Resources.m_logDir = Path.Combine(Resources.m_root, "Logs");
			Resources.m_receivedDir = Path.Combine(Resources.m_root, "Received");
			if (!isDevMode)
			{
				path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				path = Path.Combine(path, "MasterServer");
				Resources.m_resPath = "Resources";
				Resources.m_root = Path.Combine(path, Resources.m_serverName);
				Resources.m_overrideResPath = Path.Combine(Resources.m_root, "override");
				Resources.m_logDir = Path.Combine(Resources.m_root, "Logs");
				Resources.m_receivedDir = Path.Combine(Resources.m_root, "Received");
			}
			Directory.CreateDirectory(Resources.m_root);
			Directory.CreateDirectory(Resources.m_logDir);
			Directory.CreateDirectory(Resources.m_receivedDir);
			Resources.m_resUpdatePath = Path.Combine(Resources.m_resPath, "Update");
			Resources.m_statsDir = Path.Combine(Resources.m_receivedDir, "Stats");
			Resources.m_sessionSummaryDir = Path.Combine(Resources.m_receivedDir, "SessionSummaries");
			Resources.m_sessionSummaryHistoryDir = Path.Combine(Resources.m_sessionSummaryDir, "history");
			Resources.m_telemStreamDir = Path.Combine(Resources.m_receivedDir, "Streaming");
			Resources.m_recorderDir = Path.Combine(Resources.m_receivedDir, "ExecutionRecords");
			Directory.CreateDirectory(Resources.m_statsDir);
			Directory.CreateDirectory(Resources.m_sessionSummaryDir);
			Directory.CreateDirectory(Resources.m_sessionSummaryHistoryDir);
			Directory.CreateDirectory(Resources.m_telemStreamDir);
			Directory.CreateDirectory(Resources.m_recorderDir);
			Resources.m_telemResources = Path.Combine(Resources.m_resPath, "../telemetry_resources");
			Resources.m_statsFormat = Path.Combine(Resources.m_telemResources, "stats_format.xml");
			Resources.m_statsPreprocDefFile = Path.Combine(Resources.m_telemResources, "stats_preproc.txt");
			Resources.m_statsDataAccDefFile = Path.Combine(Resources.m_telemResources, "stats_acc.txt");
			Resources.m_telemSchemaCfg = Path.Combine(Resources.m_telemResources, "schema.xml");
			Resources.m_telemStatsMap = Path.Combine(Resources.m_telemResources, "stats_map.xml");
			Resources.m_supportedClientVersionsFile = Path.Combine(Resources.m_resPath, "client_versions.txt");
			Resources.m_devClientVersionsFile = Path.Combine(Resources.m_resPath, "dev_client_versions.txt");
		}

		// Token: 0x06002B89 RID: 11145 RVA: 0x000BD238 File Offset: 0x000BB638
		public static void InitBootstrap(bool isBootstrapMode, string bootstrap)
		{
			Config config = new Config(Resources.GetResourceFullPath(Resources.ResFiles.MODULES_CONFIG));
			string text = config.GetSection("Bootstrap").Get("default");
			Resources.BootstrapMode = isBootstrapMode;
			Resources.BootstrapName = ((!isBootstrapMode) ? text : bootstrap);
		}

		// Token: 0x06002B8A RID: 11146 RVA: 0x000BD280 File Offset: 0x000BB680
		private static void InitConnectionPools()
		{
			List<MasterConnectionPool> list = new List<MasterConnectionPool>();
			ConfigSection section = Resources.DBMasterSettings.GetSection("Pooling");
			ConfigSection section2 = Resources.DBSlaveSettings.GetSection("Pooling");
			ConfigSection section3 = Resources.DBMasterSettings.GetSection("Master");
			MasterConnectionPool masterConnectionPool = new MasterConnectionPool();
			masterConnectionPool.Init(section3, section, false);
			Resources.m_masterConnPool = masterConnectionPool;
			Resources.SqlDbName = section3.Get("Database");
			Resources.SqlServerAddr = section3.Get("Server");
			Resources.SqlLogin = section3.Get("User");
			Resources.SqlPassword = section3.Get("Password");
			List<ConfigSection> sections = Resources.DBSlaveSettings.GetSections("Slave");
			foreach (ConfigSection configSection in sections)
			{
				if (!string.IsNullOrEmpty(configSection.Get("Server")))
				{
					try
					{
						MasterConnectionPool masterConnectionPool2 = new MasterConnectionPool();
						masterConnectionPool2.Init(configSection, section2, false);
						list.Add(masterConnectionPool2);
					}
					catch (ApplicationException ex)
					{
						Log.Error<string, string>("Unable to connect to slave DB node {0}, skipping:\n{1}", configSection.Get("Server"), ex.Message);
					}
				}
			}
			Resources.m_slaveConnPools = list.ToArray();
		}

		// Token: 0x06002B8B RID: 11147 RVA: 0x000BD3E8 File Offset: 0x000BB7E8
		public static void Configure(IConfigurationService configurationService)
		{
			Resources.m_configurationService = configurationService;
			Resources.m_configModules = null;
			Resources.m_configCommon = null;
			Resources.m_configECatalog = null;
			Resources.m_configXMPP = null;
			Resources.m_configDBMaster = null;
			Resources.m_configDBSlave = null;
			Resources.m_configRewards = null;
			Resources.m_configQoS = null;
			Resources.m_configLB = null;
			Resources.m_configOV = null;
			Resources.m_configAntiCheat = null;
			Resources.m_abuseManager = null;
			Resources.m_profileProgression = null;
			Resources.m_configSpecialReward = null;
			Resources.m_configQuickplay = null;
			Resources.m_configCustomRules = null;
			Resources.m_configGFace = null;
			Resources.m_configAmqp = null;
			Resources.m_configAmqpQoS = null;
			Resources.m_banRequests = null;
			Resources.m_ratingCurve = null;
			Resources.InitConnectionPools();
		}

		// Token: 0x06002B8C RID: 11148 RVA: 0x000BD480 File Offset: 0x000BB880
		public static bool CheckResources()
		{
			foreach (Resources.ResourceElement resourceElement in Resources.m_resFiles)
			{
				if (resourceElement.severity == Resources.ResSeverity.MANDATORY)
				{
					string resourceFullPath = Resources.GetResourceFullPath(resourceElement.type);
					if (!File.Exists(resourceFullPath))
					{
						Log.Error<string>("Mandatory resource '{0}' not found", resourceFullPath);
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06002B8D RID: 11149 RVA: 0x000BD4E6 File Offset: 0x000BB8E6
		internal static void SetSlaveConnectionPools(ConnectionPool[] pools)
		{
			Resources.m_slaveConnPools = pools;
		}

		// Token: 0x06002B8E RID: 11150 RVA: 0x000BD4EE File Offset: 0x000BB8EE
		public static string GetResourcesDirectory()
		{
			return Resources.m_resPath;
		}

		// Token: 0x06002B8F RID: 11151 RVA: 0x000BD4F5 File Offset: 0x000BB8F5
		public static string GetUpdateDirectory()
		{
			return Resources.m_resUpdatePath;
		}

		// Token: 0x06002B90 RID: 11152 RVA: 0x000BD4FC File Offset: 0x000BB8FC
		public static string GetReceivedDirectory()
		{
			return Resources.m_receivedDir;
		}

		// Token: 0x06002B91 RID: 11153 RVA: 0x000BD504 File Offset: 0x000BB904
		public static string GetResourceFullPath(Resources.ResFiles resID)
		{
			Resources.ResourceElement resourceElement = Array.Find<Resources.ResourceElement>(Resources.m_resFiles, (Resources.ResourceElement r) => r.type == resID);
			return Resources.GetResourceFullPath(resourceElement.filename, resourceElement.res_path);
		}

		// Token: 0x06002B92 RID: 11154 RVA: 0x000BD548 File Offset: 0x000BB948
		public static string GetResourceFullPath(string filename)
		{
			return Resources.GetResourceFullPath(filename, string.Empty);
		}

		// Token: 0x06002B93 RID: 11155 RVA: 0x000BD558 File Offset: 0x000BB958
		public static string GetResourceFullPath(string filename, string res_dir)
		{
			string text = Path.Combine(Resources.m_overrideResPath, filename);
			string text2 = Path.Combine(Path.Combine(Resources.m_resPath, res_dir), filename);
			return (!File.Exists(text)) ? text2 : text;
		}

		// Token: 0x170003F7 RID: 1015
		// (get) Token: 0x06002B94 RID: 11156 RVA: 0x000BD595 File Offset: 0x000BB995
		// (set) Token: 0x06002B95 RID: 11157 RVA: 0x000BD59C File Offset: 0x000BB99C
		public static bool BootstrapMode { get; set; }

		// Token: 0x170003F8 RID: 1016
		// (get) Token: 0x06002B96 RID: 11158 RVA: 0x000BD5A4 File Offset: 0x000BB9A4
		// (set) Token: 0x06002B97 RID: 11159 RVA: 0x000BD5AB File Offset: 0x000BB9AB
		public static string BootstrapName { get; set; }

		// Token: 0x170003F9 RID: 1017
		// (get) Token: 0x06002B98 RID: 11160 RVA: 0x000BD5B3 File Offset: 0x000BB9B3
		public static string LogName
		{
			get
			{
				return "MasterServer.log";
			}
		}

		// Token: 0x170003FA RID: 1018
		// (get) Token: 0x06002B99 RID: 11161 RVA: 0x000BD5BA File Offset: 0x000BB9BA
		public static string LogsDir
		{
			get
			{
				return Resources.m_logDir;
			}
		}

		// Token: 0x170003FB RID: 1019
		// (get) Token: 0x06002B9A RID: 11162 RVA: 0x000BD5C1 File Offset: 0x000BB9C1
		public static string RootDir
		{
			get
			{
				return Resources.m_root;
			}
		}

		// Token: 0x170003FC RID: 1020
		// (get) Token: 0x06002B9B RID: 11163 RVA: 0x000BD5C8 File Offset: 0x000BB9C8
		// (set) Token: 0x06002B9C RID: 11164 RVA: 0x000BD5CF File Offset: 0x000BB9CF
		public static string SqlServerAddr
		{
			get
			{
				return Resources.m_sqlServerAddr;
			}
			set
			{
				Resources.m_sqlServerAddr = value;
			}
		}

		// Token: 0x170003FD RID: 1021
		// (get) Token: 0x06002B9D RID: 11165 RVA: 0x000BD5D7 File Offset: 0x000BB9D7
		// (set) Token: 0x06002B9E RID: 11166 RVA: 0x000BD5DE File Offset: 0x000BB9DE
		public static string SqlLogin
		{
			get
			{
				return Resources.m_sqlLogin;
			}
			set
			{
				Resources.m_sqlLogin = value;
			}
		}

		// Token: 0x170003FE RID: 1022
		// (get) Token: 0x06002B9F RID: 11167 RVA: 0x000BD5E6 File Offset: 0x000BB9E6
		// (set) Token: 0x06002BA0 RID: 11168 RVA: 0x000BD5ED File Offset: 0x000BB9ED
		public static string SqlPassword
		{
			get
			{
				return Resources.m_sqlPassword;
			}
			set
			{
				Resources.m_sqlPassword = value;
			}
		}

		// Token: 0x170003FF RID: 1023
		// (get) Token: 0x06002BA1 RID: 11169 RVA: 0x000BD5F5 File Offset: 0x000BB9F5
		// (set) Token: 0x06002BA2 RID: 11170 RVA: 0x000BD5FC File Offset: 0x000BB9FC
		public static string SqlDbName
		{
			get
			{
				return Resources.m_sqlDbName;
			}
			set
			{
				Resources.m_sqlDbName = value;
			}
		}

		// Token: 0x17000400 RID: 1024
		// (get) Token: 0x06002BA3 RID: 11171 RVA: 0x000BD604 File Offset: 0x000BBA04
		public static DBVersion SqlDbVersion
		{
			get
			{
				return Resources.m_sqlDbSchema.LatestVersion;
			}
		}

		// Token: 0x17000401 RID: 1025
		// (get) Token: 0x06002BA4 RID: 11172 RVA: 0x000BD610 File Offset: 0x000BBA10
		public static DBVersion LatestDbUpdateVersion
		{
			get
			{
				return Resources.m_latestDbUpdateSchema.LatestVersion;
			}
		}

		// Token: 0x17000402 RID: 1026
		// (get) Token: 0x06002BA5 RID: 11173 RVA: 0x000BD61C File Offset: 0x000BBA1C
		// (set) Token: 0x06002BA6 RID: 11174 RVA: 0x000BD623 File Offset: 0x000BBA23
		public static DBSchema SqlDbSchema
		{
			get
			{
				return Resources.m_sqlDbSchema;
			}
			set
			{
				Resources.m_sqlDbSchema = value;
			}
		}

		// Token: 0x17000403 RID: 1027
		// (get) Token: 0x06002BA7 RID: 11175 RVA: 0x000BD62B File Offset: 0x000BBA2B
		// (set) Token: 0x06002BA8 RID: 11176 RVA: 0x000BD632 File Offset: 0x000BBA32
		public static DBSchema DbUpdateSchema
		{
			get
			{
				return Resources.m_latestDbUpdateSchema;
			}
			set
			{
				Resources.m_latestDbUpdateSchema = value;
			}
		}

		// Token: 0x17000404 RID: 1028
		// (get) Token: 0x06002BA9 RID: 11177 RVA: 0x000BD63A File Offset: 0x000BBA3A
		public static Resources.PortData RConPort
		{
			get
			{
				return Resources.m_rconPort;
			}
		}

		// Token: 0x17000405 RID: 1029
		// (get) Token: 0x06002BAA RID: 11178 RVA: 0x000BD641 File Offset: 0x000BBA41
		public static Resources.PortData GIPort
		{
			get
			{
				return Resources.m_giPort;
			}
		}

		// Token: 0x17000406 RID: 1030
		// (get) Token: 0x06002BAB RID: 11179 RVA: 0x000BD648 File Offset: 0x000BBA48
		// (set) Token: 0x06002BAC RID: 11180 RVA: 0x000BD64F File Offset: 0x000BBA4F
		public static bool RecordExecution
		{
			get
			{
				return Resources.m_recordExec;
			}
			set
			{
				Resources.m_recordExec = value;
			}
		}

		// Token: 0x17000407 RID: 1031
		// (get) Token: 0x06002BAD RID: 11181 RVA: 0x000BD657 File Offset: 0x000BBA57
		// (set) Token: 0x06002BAE RID: 11182 RVA: 0x000BD65E File Offset: 0x000BBA5E
		public static bool StressTestMode
		{
			get
			{
				return Resources.m_stressTest;
			}
			set
			{
				Resources.m_stressTest = value;
			}
		}

		// Token: 0x17000408 RID: 1032
		// (get) Token: 0x06002BAF RID: 11183 RVA: 0x000BD666 File Offset: 0x000BBA66
		// (set) Token: 0x06002BB0 RID: 11184 RVA: 0x000BD66D File Offset: 0x000BBA6D
		public static bool DBUpdaterPermission { get; set; }

		// Token: 0x17000409 RID: 1033
		// (get) Token: 0x06002BB1 RID: 11185 RVA: 0x000BD675 File Offset: 0x000BBA75
		// (set) Token: 0x06002BB2 RID: 11186 RVA: 0x000BD67C File Offset: 0x000BBA7C
		public static bool RealmDBUpdaterPermission { get; set; }

		// Token: 0x1700040A RID: 1034
		// (get) Token: 0x06002BB3 RID: 11187 RVA: 0x000BD684 File Offset: 0x000BBA84
		// (set) Token: 0x06002BB4 RID: 11188 RVA: 0x000BD68B File Offset: 0x000BBA8B
		public static bool UseRealmMissions { get; set; }

		// Token: 0x1700040B RID: 1035
		// (get) Token: 0x06002BB5 RID: 11189 RVA: 0x000BD693 File Offset: 0x000BBA93
		// (set) Token: 0x06002BB6 RID: 11190 RVA: 0x000BD69A File Offset: 0x000BBA9A
		public static int MetricsEnabled
		{
			get
			{
				return Resources.m_metricsEnabled;
			}
			set
			{
				Resources.m_metricsEnabled = value;
			}
		}

		// Token: 0x1700040C RID: 1036
		// (get) Token: 0x06002BB7 RID: 11191 RVA: 0x000BD6A2 File Offset: 0x000BBAA2
		// (set) Token: 0x06002BB8 RID: 11192 RVA: 0x000BD6A9 File Offset: 0x000BBAA9
		public static bool DBUpdaterPermissionReset
		{
			get
			{
				return Resources.m_dbUpdaterPermissionReset;
			}
			set
			{
				Resources.m_dbUpdaterPermissionReset = value;
			}
		}

		// Token: 0x1700040D RID: 1037
		// (get) Token: 0x06002BB9 RID: 11193 RVA: 0x000BD6B1 File Offset: 0x000BBAB1
		// (set) Token: 0x06002BBA RID: 11194 RVA: 0x000BD6B8 File Offset: 0x000BBAB8
		public static bool DBUpdaterEcatPermissionReset
		{
			get
			{
				return Resources.m_dbUpdaterEcatPermissionReset;
			}
			set
			{
				Resources.m_dbUpdaterEcatPermissionReset = value;
			}
		}

		// Token: 0x1700040E RID: 1038
		// (get) Token: 0x06002BBB RID: 11195 RVA: 0x000BD6C0 File Offset: 0x000BBAC0
		// (set) Token: 0x06002BBC RID: 11196 RVA: 0x000BD6C7 File Offset: 0x000BBAC7
		public static bool AggregationEnabled
		{
			get
			{
				return Resources.m_aggEnabled;
			}
			set
			{
				Resources.m_aggEnabled = value;
			}
		}

		// Token: 0x1700040F RID: 1039
		// (get) Token: 0x06002BBD RID: 11197 RVA: 0x000BD6CF File Offset: 0x000BBACF
		// (set) Token: 0x06002BBE RID: 11198 RVA: 0x000BD6D6 File Offset: 0x000BBAD6
		public static bool DebugQueriesEnabled
		{
			get
			{
				return Resources.m_dbgEnabled;
			}
			set
			{
				Resources.m_dbgEnabled = value;
			}
		}

		// Token: 0x17000410 RID: 1040
		// (get) Token: 0x06002BBF RID: 11199 RVA: 0x000BD6DE File Offset: 0x000BBADE
		// (set) Token: 0x06002BC0 RID: 11200 RVA: 0x000BD6E5 File Offset: 0x000BBAE5
		public static bool DebugGameModeSettingsEnabled
		{
			get
			{
				return Resources.m_dbgGameModeSettingsEnabled;
			}
			set
			{
				Resources.m_dbgGameModeSettingsEnabled = value;
			}
		}

		// Token: 0x17000411 RID: 1041
		// (get) Token: 0x06002BC1 RID: 11201 RVA: 0x000BD6ED File Offset: 0x000BBAED
		// (set) Token: 0x06002BC2 RID: 11202 RVA: 0x000BD6F4 File Offset: 0x000BBAF4
		public static bool DebugContentEnabled
		{
			get
			{
				return Resources.m_debugContentEnabled;
			}
			set
			{
				Resources.m_debugContentEnabled = value;
			}
		}

		// Token: 0x17000412 RID: 1042
		// (get) Token: 0x06002BC3 RID: 11203 RVA: 0x000BD6FC File Offset: 0x000BBAFC
		// (set) Token: 0x06002BC4 RID: 11204 RVA: 0x000BD703 File Offset: 0x000BBB03
		public static bool StoreStatsInDB
		{
			get
			{
				return Resources.m_storeStatsDB;
			}
			set
			{
				Resources.m_storeStatsDB = value;
			}
		}

		// Token: 0x17000413 RID: 1043
		// (get) Token: 0x06002BC5 RID: 11205 RVA: 0x000BD70B File Offset: 0x000BBB0B
		public static string XmppOnlineDomain
		{
			get
			{
				return "masterserver";
			}
		}

		// Token: 0x17000414 RID: 1044
		// (get) Token: 0x06002BC6 RID: 11206 RVA: 0x000BD712 File Offset: 0x000BBB12
		// (set) Token: 0x06002BC7 RID: 11207 RVA: 0x000BD719 File Offset: 0x000BBB19
		public static string XmppResource
		{
			get
			{
				return Resources.m_xmpp_resource;
			}
			set
			{
				Resources.m_xmpp_resource = value;
			}
		}

		// Token: 0x17000415 RID: 1045
		// (get) Token: 0x06002BC8 RID: 11208 RVA: 0x000BD721 File Offset: 0x000BBB21
		// (set) Token: 0x06002BC9 RID: 11209 RVA: 0x000BD728 File Offset: 0x000BBB28
		public static string MasterVersion
		{
			get
			{
				return Resources.m_version;
			}
			set
			{
				Resources.m_version = value;
			}
		}

		// Token: 0x17000416 RID: 1046
		// (get) Token: 0x06002BCA RID: 11210 RVA: 0x000BD730 File Offset: 0x000BBB30
		// (set) Token: 0x06002BCB RID: 11211 RVA: 0x000BD737 File Offset: 0x000BBB37
		public static string MonoVersion
		{
			get
			{
				return Resources.m_monoVersion;
			}
			set
			{
				Resources.m_monoVersion = value;
			}
		}

		// Token: 0x17000417 RID: 1047
		// (get) Token: 0x06002BCC RID: 11212 RVA: 0x000BD73F File Offset: 0x000BBB3F
		// (set) Token: 0x06002BCD RID: 11213 RVA: 0x000BD746 File Offset: 0x000BBB46
		public static bool IsDevMode
		{
			get
			{
				return Resources.m_isDevMode;
			}
			set
			{
				Resources.m_isDevMode = value;
			}
		}

		// Token: 0x17000418 RID: 1048
		// (get) Token: 0x06002BCE RID: 11214 RVA: 0x000BD74E File Offset: 0x000BBB4E
		// (set) Token: 0x06002BCF RID: 11215 RVA: 0x000BD755 File Offset: 0x000BBB55
		public static bool IsDaemon
		{
			get
			{
				return Resources.m_isDaemon;
			}
			set
			{
				Resources.m_isDaemon = value;
			}
		}

		// Token: 0x17000419 RID: 1049
		// (get) Token: 0x06002BD0 RID: 11216 RVA: 0x000BD75D File Offset: 0x000BBB5D
		// (set) Token: 0x06002BD1 RID: 11217 RVA: 0x000BD764 File Offset: 0x000BBB64
		public static bool IsForceUpdateDatabase
		{
			get
			{
				return Resources.m_isForceUpdateDatabase;
			}
			set
			{
				Resources.m_isForceUpdateDatabase = value;
			}
		}

		// Token: 0x1700041A RID: 1050
		// (get) Token: 0x06002BD2 RID: 11218 RVA: 0x000BD76C File Offset: 0x000BBB6C
		// (set) Token: 0x06002BD3 RID: 11219 RVA: 0x000BD773 File Offset: 0x000BBB73
		public static bool DebugUpdateItemsAllow
		{
			get
			{
				return Resources.m_debugUpdateItemsAllow;
			}
			set
			{
				Resources.m_debugUpdateItemsAllow = value;
			}
		}

		// Token: 0x1700041B RID: 1051
		// (get) Token: 0x06002BD4 RID: 11220 RVA: 0x000BD77B File Offset: 0x000BBB7B
		// (set) Token: 0x06002BD5 RID: 11221 RVA: 0x000BD782 File Offset: 0x000BBB82
		public static bool UseMonoRWLock { get; set; }

		// Token: 0x1700041C RID: 1052
		// (get) Token: 0x06002BD6 RID: 11222 RVA: 0x000BD78C File Offset: 0x000BBB8C
		// (set) Token: 0x06002BD7 RID: 11223 RVA: 0x000BD7D3 File Offset: 0x000BBBD3
		public static string Jid
		{
			get
			{
				if (string.IsNullOrEmpty(Resources.m_jid))
				{
					IOnlineClient service = ServicesManager.GetService<IOnlineClient>();
					Resources.m_jid = new Jid(service.OnlineID, service.XmppHost, Resources.XmppResource).ToString();
				}
				return Resources.m_jid;
			}
			set
			{
				Resources.m_jid = value;
			}
		}

		// Token: 0x1700041D RID: 1053
		// (get) Token: 0x06002BD8 RID: 11224 RVA: 0x000BD7DB File Offset: 0x000BBBDB
		public static ConnectionPool MasterConnectionPool
		{
			get
			{
				return Resources.m_masterConnPool;
			}
		}

		// Token: 0x1700041E RID: 1054
		// (get) Token: 0x06002BD9 RID: 11225 RVA: 0x000BD7E2 File Offset: 0x000BBBE2
		public static ConnectionPool[] SlaveConnectionPools
		{
			get
			{
				return Resources.m_slaveConnPools;
			}
		}

		// Token: 0x1700041F RID: 1055
		// (get) Token: 0x06002BDA RID: 11226 RVA: 0x000BD7E9 File Offset: 0x000BBBE9
		public static string StatsDirectory
		{
			get
			{
				return Resources.m_statsDir;
			}
		}

		// Token: 0x17000420 RID: 1056
		// (get) Token: 0x06002BDB RID: 11227 RVA: 0x000BD7F0 File Offset: 0x000BBBF0
		public static string SessionSummariesDirectory
		{
			get
			{
				return Resources.m_sessionSummaryDir;
			}
		}

		// Token: 0x17000421 RID: 1057
		// (get) Token: 0x06002BDC RID: 11228 RVA: 0x000BD7F7 File Offset: 0x000BBBF7
		public static string SessionSummariesHistoryDirectory
		{
			get
			{
				return Resources.m_sessionSummaryHistoryDir;
			}
		}

		// Token: 0x17000422 RID: 1058
		// (get) Token: 0x06002BDD RID: 11229 RVA: 0x000BD7FE File Offset: 0x000BBBFE
		public static string TelemStreamDir
		{
			get
			{
				return Resources.m_telemStreamDir;
			}
		}

		// Token: 0x17000423 RID: 1059
		// (get) Token: 0x06002BDE RID: 11230 RVA: 0x000BD805 File Offset: 0x000BBC05
		public static string ExecRecorderDir
		{
			get
			{
				return Resources.m_recorderDir;
			}
		}

		// Token: 0x17000424 RID: 1060
		// (get) Token: 0x06002BDF RID: 11231 RVA: 0x000BD80C File Offset: 0x000BBC0C
		public static string TelemetryResourceDir
		{
			get
			{
				return Resources.m_telemResources;
			}
		}

		// Token: 0x17000425 RID: 1061
		// (get) Token: 0x06002BE0 RID: 11232 RVA: 0x000BD813 File Offset: 0x000BBC13
		public static string StatsFormatFile
		{
			get
			{
				return Resources.m_statsFormat;
			}
		}

		// Token: 0x17000426 RID: 1062
		// (get) Token: 0x06002BE1 RID: 11233 RVA: 0x000BD81A File Offset: 0x000BBC1A
		public static string StatsPreprocDefFile
		{
			get
			{
				return Resources.m_statsPreprocDefFile;
			}
		}

		// Token: 0x17000427 RID: 1063
		// (get) Token: 0x06002BE2 RID: 11234 RVA: 0x000BD821 File Offset: 0x000BBC21
		public static string StatsDataAccDefFile
		{
			get
			{
				return Resources.m_statsDataAccDefFile;
			}
		}

		// Token: 0x17000428 RID: 1064
		// (get) Token: 0x06002BE3 RID: 11235 RVA: 0x000BD828 File Offset: 0x000BBC28
		public static string TelemetryConnectionCfg
		{
			get
			{
				return Resources.GetResourceFullPath(Resources.ResFiles.TELEM_CONNECTION);
			}
		}

		// Token: 0x17000429 RID: 1065
		// (get) Token: 0x06002BE4 RID: 11236 RVA: 0x000BD831 File Offset: 0x000BBC31
		public static string TelemetrySchemaCfg
		{
			get
			{
				return Resources.m_telemSchemaCfg;
			}
		}

		// Token: 0x1700042A RID: 1066
		// (get) Token: 0x06002BE5 RID: 11237 RVA: 0x000BD838 File Offset: 0x000BBC38
		public static string TelemetryStatsMap
		{
			get
			{
				return Resources.m_telemStatsMap;
			}
		}

		// Token: 0x1700042B RID: 1067
		// (get) Token: 0x06002BE6 RID: 11238 RVA: 0x000BD83F File Offset: 0x000BBC3F
		public static IEnumerable<string> SupportedClientVersionsFiles
		{
			get
			{
				string[] result;
				if (Resources.DebugQueriesEnabled)
				{
					string[] array = new string[2];
					array[0] = Resources.m_supportedClientVersionsFile;
					result = array;
					array[1] = Resources.m_devClientVersionsFile;
				}
				else
				{
					(result = new string[1])[0] = Resources.m_supportedClientVersionsFile;
				}
				return result;
			}
		}

		// Token: 0x1700042C RID: 1068
		// (get) Token: 0x06002BE7 RID: 11239 RVA: 0x000BD874 File Offset: 0x000BBC74
		public static string DevClientVersionsFile
		{
			get
			{
				return Resources.m_devClientVersionsFile;
			}
		}

		// Token: 0x1700042D RID: 1069
		// (get) Token: 0x06002BE8 RID: 11240 RVA: 0x000BD87B File Offset: 0x000BBC7B
		// (set) Token: 0x06002BE9 RID: 11241 RVA: 0x000BD882 File Offset: 0x000BBC82
		public static int ServerID
		{
			get
			{
				return Resources.m_serverId;
			}
			set
			{
				Resources.m_serverId = value;
			}
		}

		// Token: 0x1700042E RID: 1070
		// (get) Token: 0x06002BEA RID: 11242 RVA: 0x000BD88A File Offset: 0x000BBC8A
		// (set) Token: 0x06002BEB RID: 11243 RVA: 0x000BD891 File Offset: 0x000BBC91
		public static string ServerName
		{
			get
			{
				return Resources.m_serverName;
			}
			set
			{
				Resources.m_serverName = value;
			}
		}

		// Token: 0x1700042F RID: 1071
		// (get) Token: 0x06002BEC RID: 11244 RVA: 0x000BD899 File Offset: 0x000BBC99
		public static string Hostname
		{
			get
			{
				return Environment.MachineName;
			}
		}

		// Token: 0x17000430 RID: 1072
		// (get) Token: 0x06002BED RID: 11245 RVA: 0x000BD8A0 File Offset: 0x000BBCA0
		// (set) Token: 0x06002BEE RID: 11246 RVA: 0x000BD8A7 File Offset: 0x000BBCA7
		public static Resources.ChannelType Channel
		{
			get
			{
				return Resources.m_channel;
			}
			set
			{
				Resources.m_channel = value;
			}
		}

		// Token: 0x17000431 RID: 1073
		// (get) Token: 0x06002BEF RID: 11247 RVA: 0x000BD8AF File Offset: 0x000BBCAF
		public static string ChannelName
		{
			get
			{
				return Resources.m_channel.ToString().ToLower();
			}
		}

		// Token: 0x17000432 RID: 1074
		// (get) Token: 0x06002BF0 RID: 11248 RVA: 0x000BD8C6 File Offset: 0x000BBCC6
		// (set) Token: 0x06002BF1 RID: 11249 RVA: 0x000BD8E3 File Offset: 0x000BBCE3
		public static Config ModuleSettings
		{
			get
			{
				return Resources.m_configModules ?? Resources.m_configurationService.GetConfig(ConfigInfo.ModuleConfiguration);
			}
			set
			{
				Resources.m_configModules = value;
			}
		}

		// Token: 0x17000433 RID: 1075
		// (get) Token: 0x06002BF2 RID: 11250 RVA: 0x000BD8EB File Offset: 0x000BBCEB
		// (set) Token: 0x06002BF3 RID: 11251 RVA: 0x000BD908 File Offset: 0x000BBD08
		public static Config CommonSettings
		{
			get
			{
				return Resources.m_configCommon ?? Resources.m_configurationService.GetConfig(MsConfigInfo.CommonConfiguration);
			}
			set
			{
				Resources.m_configCommon = value;
			}
		}

		// Token: 0x17000434 RID: 1076
		// (get) Token: 0x06002BF4 RID: 11252 RVA: 0x000BD910 File Offset: 0x000BBD10
		public static Config DBMasterSettings
		{
			get
			{
				return Resources.m_configDBMaster ?? Resources.m_configurationService.GetConfig(MsConfigInfo.DbMasterConfiguration);
			}
		}

		// Token: 0x17000435 RID: 1077
		// (get) Token: 0x06002BF5 RID: 11253 RVA: 0x000BD92D File Offset: 0x000BBD2D
		public static Config DBSlaveSettings
		{
			get
			{
				return Resources.m_configDBSlave ?? Resources.m_configurationService.GetConfig(MsConfigInfo.DbSlaveConfiguration);
			}
		}

		// Token: 0x17000436 RID: 1078
		// (get) Token: 0x06002BF6 RID: 11254 RVA: 0x000BD94A File Offset: 0x000BBD4A
		// (set) Token: 0x06002BF7 RID: 11255 RVA: 0x000BD967 File Offset: 0x000BBD67
		public static Config ECatalogSettings
		{
			get
			{
				return Resources.m_configECatalog ?? Resources.m_configurationService.GetConfig(MsConfigInfo.ECatalogConfiguration);
			}
			set
			{
				Resources.m_configECatalog = value;
			}
		}

		// Token: 0x17000437 RID: 1079
		// (get) Token: 0x06002BF8 RID: 11256 RVA: 0x000BD96F File Offset: 0x000BBD6F
		// (set) Token: 0x06002BF9 RID: 11257 RVA: 0x000BD98C File Offset: 0x000BBD8C
		public static Config XMPPSettings
		{
			get
			{
				return Resources.m_configXMPP ?? Resources.m_configurationService.GetConfig(MsConfigInfo.XmppConfiguration);
			}
			set
			{
				Resources.m_configXMPP = value;
			}
		}

		// Token: 0x17000438 RID: 1080
		// (get) Token: 0x06002BFA RID: 11258 RVA: 0x000BD994 File Offset: 0x000BBD94
		public static Config OnlineVariablesSettings
		{
			get
			{
				return Resources.m_configOV ?? Resources.m_configurationService.GetConfig(MsConfigInfo.OnlineVariablesConfiguration);
			}
		}

		// Token: 0x17000439 RID: 1081
		// (get) Token: 0x06002BFB RID: 11259 RVA: 0x000BD9B1 File Offset: 0x000BBDB1
		// (set) Token: 0x06002BFC RID: 11260 RVA: 0x000BD9CE File Offset: 0x000BBDCE
		public static Config Rewards
		{
			get
			{
				return Resources.m_configRewards ?? Resources.m_configurationService.GetConfig(MsConfigInfo.RewardsConfiguration);
			}
			set
			{
				Resources.m_configRewards = value;
			}
		}

		// Token: 0x1700043A RID: 1082
		// (get) Token: 0x06002BFD RID: 11261 RVA: 0x000BD9D6 File Offset: 0x000BBDD6
		public static Config QoSSettings
		{
			get
			{
				return Resources.m_configQoS ?? Resources.m_configurationService.GetConfig(MsConfigInfo.QosConfiguration);
			}
		}

		// Token: 0x1700043B RID: 1083
		// (get) Token: 0x06002BFE RID: 11262 RVA: 0x000BD9F3 File Offset: 0x000BBDF3
		public static Config LBSettings
		{
			get
			{
				return Resources.m_configLB ?? Resources.m_configurationService.GetConfig(MsConfigInfo.LbConfiguration);
			}
		}

		// Token: 0x1700043C RID: 1084
		// (get) Token: 0x06002BFF RID: 11263 RVA: 0x000BDA10 File Offset: 0x000BBE10
		public static Config AntiCheatConfig
		{
			get
			{
				return Resources.m_configAntiCheat ?? Resources.m_configurationService.GetConfig(MsConfigInfo.AnticheatConfiguration);
			}
		}

		// Token: 0x1700043D RID: 1085
		// (get) Token: 0x06002C00 RID: 11264 RVA: 0x000BDA2D File Offset: 0x000BBE2D
		public static Config AbuseManagerConfig
		{
			get
			{
				return Resources.m_abuseManager ?? Resources.m_configurationService.GetConfig(MsConfigInfo.AbuseConfiguration);
			}
		}

		// Token: 0x1700043E RID: 1086
		// (get) Token: 0x06002C01 RID: 11265 RVA: 0x000BDA4A File Offset: 0x000BBE4A
		// (set) Token: 0x06002C02 RID: 11266 RVA: 0x000BDA67 File Offset: 0x000BBE67
		public static Config ProfileProgressionConfig
		{
			get
			{
				return Resources.m_profileProgression ?? Resources.m_configurationService.GetConfig(MsConfigInfo.ProgressionConfiguration);
			}
			set
			{
				Resources.m_profileProgression = value;
			}
		}

		// Token: 0x1700043F RID: 1087
		// (get) Token: 0x06002C03 RID: 11267 RVA: 0x000BDA6F File Offset: 0x000BBE6F
		// (set) Token: 0x06002C04 RID: 11268 RVA: 0x000BDA8C File Offset: 0x000BBE8C
		public static Config SpecialRewardSettings
		{
			get
			{
				return Resources.m_configSpecialReward ?? Resources.m_configurationService.GetConfig(MsConfigInfo.SpecialRewardConfiguration);
			}
			set
			{
				Resources.m_configSpecialReward = value;
			}
		}

		// Token: 0x17000440 RID: 1088
		// (get) Token: 0x06002C05 RID: 11269 RVA: 0x000BDA94 File Offset: 0x000BBE94
		// (set) Token: 0x06002C06 RID: 11270 RVA: 0x000BDAB1 File Offset: 0x000BBEB1
		public static Config QuickplayConfig
		{
			get
			{
				return Resources.m_configQuickplay ?? Resources.m_configurationService.GetConfig(MsConfigInfo.QuickplayConfiguration);
			}
			set
			{
				Resources.m_configQuickplay = value;
			}
		}

		// Token: 0x17000441 RID: 1089
		// (get) Token: 0x06002C07 RID: 11271 RVA: 0x000BDAB9 File Offset: 0x000BBEB9
		// (set) Token: 0x06002C08 RID: 11272 RVA: 0x000BDAD6 File Offset: 0x000BBED6
		public static Config CustomRulesConfig
		{
			get
			{
				return Resources.m_configCustomRules ?? Resources.m_configurationService.GetConfig(MsConfigInfo.CustomRulesConfiguration);
			}
			set
			{
				Resources.m_configCustomRules = value;
			}
		}

		// Token: 0x17000442 RID: 1090
		// (get) Token: 0x06002C09 RID: 11273 RVA: 0x000BDADE File Offset: 0x000BBEDE
		public static Config GFaceSettings
		{
			get
			{
				return Resources.m_configGFace ?? Resources.m_configurationService.GetConfig(MsConfigInfo.GFaceConfiguration);
			}
		}

		// Token: 0x17000443 RID: 1091
		// (get) Token: 0x06002C0A RID: 11274 RVA: 0x000BDAFB File Offset: 0x000BBEFB
		public static Config AmqpConfig
		{
			get
			{
				return Resources.m_configAmqp ?? Resources.m_configurationService.GetConfig(AmqpConfigInfo.AmqpConfiguration);
			}
		}

		// Token: 0x17000444 RID: 1092
		// (get) Token: 0x06002C0B RID: 11275 RVA: 0x000BDB18 File Offset: 0x000BBF18
		public static Config AmqpQoSConfig
		{
			get
			{
				return Resources.m_configAmqpQoS ?? Resources.m_configurationService.GetConfig(AmqpConfigInfo.AmqpQosConfiguration);
			}
		}

		// Token: 0x17000445 RID: 1093
		// (get) Token: 0x06002C0C RID: 11276 RVA: 0x000BDB35 File Offset: 0x000BBF35
		// (set) Token: 0x06002C0D RID: 11277 RVA: 0x000BDB52 File Offset: 0x000BBF52
		public static Config BanRequestsConfig
		{
			get
			{
				return Resources.m_banRequests ?? Resources.m_configurationService.GetConfig(MsConfigInfo.BanRequestsConfiguration);
			}
			set
			{
				Resources.m_banRequests = value;
			}
		}

		// Token: 0x17000446 RID: 1094
		// (get) Token: 0x06002C0E RID: 11278 RVA: 0x000BDB5A File Offset: 0x000BBF5A
		public static Config RatingCurveConfig
		{
			get
			{
				return Resources.m_ratingCurve ?? Resources.m_configurationService.GetConfig(MsConfigInfo.RatingCurveConfiguration);
			}
		}

		// Token: 0x17000447 RID: 1095
		// (get) Token: 0x06002C0F RID: 11279 RVA: 0x000BDB77 File Offset: 0x000BBF77
		// (set) Token: 0x06002C10 RID: 11280 RVA: 0x000BDB7E File Offset: 0x000BBF7E
		public static List<string> StartupConsoleCmds
		{
			get
			{
				return Resources.m_startupConsoleCmds;
			}
			set
			{
				Resources.m_startupConsoleCmds = value;
			}
		}

		// Token: 0x17000448 RID: 1096
		// (get) Token: 0x06002C11 RID: 11281 RVA: 0x000BDB88 File Offset: 0x000BBF88
		public static int RealmId
		{
			get
			{
				ConfigSection section = Resources.CommonSettings.GetSection("realm");
				if (section != null)
				{
					int result;
					section.Get("id", out result);
					return result;
				}
				return 1;
			}
		}

		// Token: 0x17000449 RID: 1097
		// (get) Token: 0x06002C12 RID: 11282 RVA: 0x000BDBBB File Offset: 0x000BBFBB
		public static int GlobalRealmId
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700044A RID: 1098
		// (get) Token: 0x06002C13 RID: 11283 RVA: 0x000BDBBE File Offset: 0x000BBFBE
		// (set) Token: 0x06002C14 RID: 11284 RVA: 0x000BDBC5 File Offset: 0x000BBFC5
		public static bool DbgCacheServices { get; set; }

		// Token: 0x04001764 RID: 5988
		public const string DEFAULT_TIMESTAMP_FORMAT = "yyyy-MM-ddTHH:mm:ss.fffzz";

		// Token: 0x04001765 RID: 5989
		public const int DEFAULT_REALM_ID = 1;

		// Token: 0x04001766 RID: 5990
		public const int GLOBAL_REALM_ID = 0;

		// Token: 0x04001767 RID: 5991
		public const string DEFAULT_REGION_ID = "global";

		// Token: 0x04001768 RID: 5992
		private static string m_sqlServerAddr;

		// Token: 0x04001769 RID: 5993
		private static string m_sqlLogin;

		// Token: 0x0400176A RID: 5994
		private static string m_sqlPassword;

		// Token: 0x0400176B RID: 5995
		private static string m_sqlDbName;

		// Token: 0x0400176C RID: 5996
		private static DBSchema m_sqlDbSchema = new DBSchema();

		// Token: 0x0400176D RID: 5997
		private static DBSchema m_latestDbUpdateSchema = new DBSchema();

		// Token: 0x0400176E RID: 5998
		private static string m_xmpp_resource;

		// Token: 0x0400176F RID: 5999
		private static bool m_isDevMode;

		// Token: 0x04001770 RID: 6000
		private static bool m_isDaemon;

		// Token: 0x04001771 RID: 6001
		private static bool m_isForceUpdateDatabase;

		// Token: 0x04001772 RID: 6002
		private static bool m_debugUpdateItemsAllow;

		// Token: 0x04001773 RID: 6003
		private static bool m_recordExec;

		// Token: 0x04001774 RID: 6004
		private static bool m_stressTest;

		// Token: 0x04001775 RID: 6005
		private static bool m_dbUpdaterPermissionReset;

		// Token: 0x04001776 RID: 6006
		private static bool m_dbUpdaterEcatPermissionReset;

		// Token: 0x04001777 RID: 6007
		private static bool m_aggEnabled = true;

		// Token: 0x04001778 RID: 6008
		private static bool m_dbgEnabled;

		// Token: 0x04001779 RID: 6009
		private static bool m_dbgGameModeSettingsEnabled;

		// Token: 0x0400177A RID: 6010
		private static bool m_debugContentEnabled;

		// Token: 0x0400177B RID: 6011
		private static bool m_storeStatsDB;

		// Token: 0x0400177C RID: 6012
		private static List<string> m_startupConsoleCmds;

		// Token: 0x0400177D RID: 6013
		private const string UPDATE_DIR = "Update";

		// Token: 0x0400177E RID: 6014
		private const string LOG_FILENAME = "MasterServer.log";

		// Token: 0x0400177F RID: 6015
		private const string DEFAULT_SERVER_ID = "msid";

		// Token: 0x04001780 RID: 6016
		private static string m_resUpdatePath;

		// Token: 0x04001781 RID: 6017
		private static string m_root;

		// Token: 0x04001782 RID: 6018
		private static string m_logDir;

		// Token: 0x04001783 RID: 6019
		private static string m_resPath;

		// Token: 0x04001784 RID: 6020
		private static string m_overrideResPath;

		// Token: 0x04001785 RID: 6021
		private static string m_receivedDir;

		// Token: 0x04001786 RID: 6022
		private static string m_statsDir;

		// Token: 0x04001787 RID: 6023
		private static string m_sessionSummaryDir;

		// Token: 0x04001788 RID: 6024
		private static string m_sessionSummaryHistoryDir;

		// Token: 0x04001789 RID: 6025
		private static string m_telemStreamDir;

		// Token: 0x0400178A RID: 6026
		private static string m_recorderDir;

		// Token: 0x0400178B RID: 6027
		private static string m_telemResources;

		// Token: 0x0400178C RID: 6028
		private static string m_statsFormat;

		// Token: 0x0400178D RID: 6029
		private static string m_statsPreprocDefFile;

		// Token: 0x0400178E RID: 6030
		private static string m_statsDataAccDefFile;

		// Token: 0x0400178F RID: 6031
		private static string m_telemSchemaCfg;

		// Token: 0x04001790 RID: 6032
		private static string m_telemStatsMap;

		// Token: 0x04001791 RID: 6033
		private static string m_supportedClientVersionsFile;

		// Token: 0x04001792 RID: 6034
		private static string m_devClientVersionsFile;

		// Token: 0x04001793 RID: 6035
		private static int m_serverId;

		// Token: 0x04001794 RID: 6036
		private static string m_serverName = string.Empty;

		// Token: 0x04001795 RID: 6037
		private static Resources.ChannelType m_channel = Resources.ChannelType.PVE;

		// Token: 0x04001796 RID: 6038
		private static string m_version = "undefined";

		// Token: 0x04001797 RID: 6039
		private static string m_monoVersion = "undefined";

		// Token: 0x04001798 RID: 6040
		private static int m_metricsEnabled = -1;

		// Token: 0x04001799 RID: 6041
		private static string m_jid;

		// Token: 0x0400179A RID: 6042
		private static Resources.PortData m_rconPort = new Resources.PortData();

		// Token: 0x0400179B RID: 6043
		private static Resources.PortData m_giPort = new Resources.PortData();

		// Token: 0x0400179C RID: 6044
		private static Resources.ResourceElement[] m_resFiles = new Resources.ResourceElement[]
		{
			new Resources.ResourceElement("module_configuration.xml", Resources.ResFiles.MODULES_CONFIG, Resources.ResSeverity.MANDATORY),
			new Resources.ResourceElement("configuration.xml", Resources.ResFiles.COMMON_CONFIG, Resources.ResSeverity.MANDATORY),
			new Resources.ResourceElement("xmpp_configuration.xml", Resources.ResFiles.XMPP_CONFIG, Resources.ResSeverity.MANDATORY),
			new Resources.ResourceElement("qos_configuration.xml", Resources.ResFiles.QOS_CONFIG, Resources.ResSeverity.MANDATORY),
			new Resources.ResourceElement("online_variables.xml", Resources.ResFiles.OV_CONFIG, Resources.ResSeverity.MANDATORY),
			new Resources.ResourceElement("lb_configuration.xml", Resources.ResFiles.LB_CONFIG, Resources.ResSeverity.MANDATORY),
			new Resources.ResourceElement("ecat_configuration.xml", Resources.ResFiles.ECAT_CONFIG, Resources.ResSeverity.MANDATORY),
			new Resources.ResourceElement("db_configuration.xml", Resources.ResFiles.DB_MASTER_CONFIG, Resources.ResSeverity.MANDATORY),
			new Resources.ResourceElement("slave_configuration.xml", Resources.ResFiles.DB_SLAVE_CONFIG, Resources.ResSeverity.OPTIONAL),
			new Resources.ResourceElement("rewards_configuration.xml", Resources.ResFiles.REWARDS_CONFIG, Resources.ResSeverity.MANDATORY),
			new Resources.ResourceElement("Update", "MasterServerUpdate.dll", Resources.ResFiles.MSA_UPDATE, Resources.ResSeverity.OPTIONAL),
			new Resources.ResourceElement("missions", Resources.ResFiles.MISSIONS_DIR, Resources.ResSeverity.OPTIONAL),
			new Resources.ResourceElement("../telemetry_resources", "connection.xml", Resources.ResFiles.TELEM_CONNECTION, Resources.ResSeverity.MANDATORY),
			new Resources.ResourceElement("anticheat_configuration.xml", Resources.ResFiles.ANTICHEAT_CONFIG, Resources.ResSeverity.MANDATORY),
			new Resources.ResourceElement("abuse_manager_config.xml", Resources.ResFiles.ABUSE_CONFIG, Resources.ResSeverity.MANDATORY),
			new Resources.ResourceElement("profile_progression_config.xml", Resources.ResFiles.PROF_PROGRESSION_CONFIG, Resources.ResSeverity.MANDATORY),
			new Resources.ResourceElement("special_reward_configuration.xml", Resources.ResFiles.SPECIAL_REWARD_CONFIG, Resources.ResSeverity.MANDATORY),
			new Resources.ResourceElement("quickplay_maps.xml", Resources.ResFiles.QUICKPLAY_CONFIG, Resources.ResSeverity.OPTIONAL),
			new Resources.ResourceElement("custom_rules.xml", Resources.ResFiles.CUSTOM_RULES, Resources.ResSeverity.OPTIONAL),
			new Resources.ResourceElement("gface_configuration.xml", Resources.ResFiles.GFACE_CONFIG, Resources.ResSeverity.MANDATORY),
			new Resources.ResourceElement("mission_generation_configuration.xml", Resources.ResFiles.MISSION_GENERATION_CONFIG, Resources.ResSeverity.MANDATORY),
			new Resources.ResourceElement("amqp_configuration.xml", Resources.ResFiles.AMQP_CONFIG, Resources.ResSeverity.MANDATORY),
			new Resources.ResourceElement("amqp_qos_configuration.xml", Resources.ResFiles.AMQP_QOS_CONFIG, Resources.ResSeverity.MANDATORY),
			new Resources.ResourceElement("client_versions.txt", Resources.ResFiles.CLIENT_VERSIONS, Resources.ResSeverity.OPTIONAL),
			new Resources.ResourceElement("ban_requests_config.xml", Resources.ResFiles.BAN_REQUESTS_CONFIG, Resources.ResSeverity.OPTIONAL),
			new Resources.ResourceElement("rating_curve.xml", Resources.ResFiles.RATING_CURVE_CONFIG, Resources.ResSeverity.MANDATORY)
		};

		// Token: 0x0400179D RID: 6045
		private static ConnectionPool m_masterConnPool;

		// Token: 0x0400179E RID: 6046
		private static ConnectionPool[] m_slaveConnPools;

		// Token: 0x0400179F RID: 6047
		private static Config m_configModules = new Config();

		// Token: 0x040017A0 RID: 6048
		private static Config m_configCommon = new Config();

		// Token: 0x040017A1 RID: 6049
		private static Config m_configECatalog = new Config();

		// Token: 0x040017A2 RID: 6050
		private static Config m_configXMPP = new Config();

		// Token: 0x040017A3 RID: 6051
		private static Config m_configDBMaster = new Config();

		// Token: 0x040017A4 RID: 6052
		private static Config m_configDBSlave = new Config();

		// Token: 0x040017A5 RID: 6053
		private static Config m_configRewards = new Config();

		// Token: 0x040017A6 RID: 6054
		private static Config m_configQoS = new Config();

		// Token: 0x040017A7 RID: 6055
		private static Config m_configLB = new Config();

		// Token: 0x040017A8 RID: 6056
		private static Config m_configOV = new Config();

		// Token: 0x040017A9 RID: 6057
		private static Config m_configAntiCheat = new Config();

		// Token: 0x040017AA RID: 6058
		private static Config m_abuseManager = new Config();

		// Token: 0x040017AB RID: 6059
		private static Config m_profileProgression = new Config();

		// Token: 0x040017AC RID: 6060
		private static Config m_configSpecialReward = new Config();

		// Token: 0x040017AD RID: 6061
		private static Config m_configQuickplay = new Config();

		// Token: 0x040017AE RID: 6062
		private static Config m_configCustomRules = new Config();

		// Token: 0x040017AF RID: 6063
		private static Config m_configGFace = new Config();

		// Token: 0x040017B0 RID: 6064
		private static Config m_configAmqp = new Config();

		// Token: 0x040017B1 RID: 6065
		private static Config m_configAmqpQoS = new Config();

		// Token: 0x040017B2 RID: 6066
		private static Config m_banRequests = new Config();

		// Token: 0x040017B3 RID: 6067
		private static Config m_ratingCurve = new Config();

		// Token: 0x040017B4 RID: 6068
		private static IConfigurationService m_configurationService;

		// Token: 0x0200083A RID: 2106
		public enum ResFiles
		{
			// Token: 0x040017BD RID: 6077
			MODULES_CONFIG,
			// Token: 0x040017BE RID: 6078
			COMMON_CONFIG,
			// Token: 0x040017BF RID: 6079
			XMPP_CONFIG,
			// Token: 0x040017C0 RID: 6080
			OV_CONFIG,
			// Token: 0x040017C1 RID: 6081
			LB_CONFIG,
			// Token: 0x040017C2 RID: 6082
			QOS_CONFIG,
			// Token: 0x040017C3 RID: 6083
			ECAT_CONFIG,
			// Token: 0x040017C4 RID: 6084
			DB_MASTER_CONFIG,
			// Token: 0x040017C5 RID: 6085
			DB_SLAVE_CONFIG,
			// Token: 0x040017C6 RID: 6086
			REWARDS_CONFIG,
			// Token: 0x040017C7 RID: 6087
			MSA_UPDATE,
			// Token: 0x040017C8 RID: 6088
			MISSIONS_DIR,
			// Token: 0x040017C9 RID: 6089
			TELEM_CONNECTION,
			// Token: 0x040017CA RID: 6090
			ANTICHEAT_CONFIG,
			// Token: 0x040017CB RID: 6091
			ABUSE_CONFIG,
			// Token: 0x040017CC RID: 6092
			PROF_PROGRESSION_CONFIG,
			// Token: 0x040017CD RID: 6093
			SPECIAL_REWARD_CONFIG,
			// Token: 0x040017CE RID: 6094
			QUICKPLAY_CONFIG,
			// Token: 0x040017CF RID: 6095
			CUSTOM_RULES,
			// Token: 0x040017D0 RID: 6096
			GFACE_CONFIG,
			// Token: 0x040017D1 RID: 6097
			MISSION_GENERATION_CONFIG,
			// Token: 0x040017D2 RID: 6098
			AMQP_CONFIG,
			// Token: 0x040017D3 RID: 6099
			AMQP_QOS_CONFIG,
			// Token: 0x040017D4 RID: 6100
			CLIENT_VERSIONS,
			// Token: 0x040017D5 RID: 6101
			BAN_REQUESTS_CONFIG,
			// Token: 0x040017D6 RID: 6102
			RATING_CURVE_CONFIG
		}

		// Token: 0x0200083B RID: 2107
		public enum ResSeverity
		{
			// Token: 0x040017D8 RID: 6104
			MANDATORY,
			// Token: 0x040017D9 RID: 6105
			OPTIONAL
		}

		// Token: 0x0200083C RID: 2108
		public enum ChannelType
		{
			// Token: 0x040017DB RID: 6107
			PVE,
			// Token: 0x040017DC RID: 6108
			PVP_Newbie,
			// Token: 0x040017DD RID: 6109
			PVP_Skilled,
			// Token: 0x040017DE RID: 6110
			PVP_Pro,
			// Token: 0x040017DF RID: 6111
			Service
		}

		// Token: 0x0200083D RID: 2109
		public enum ChannelRankGroup
		{
			// Token: 0x040017E1 RID: 6113
			All,
			// Token: 0x040017E2 RID: 6114
			Newbie,
			// Token: 0x040017E3 RID: 6115
			Skilled
		}

		// Token: 0x0200083E RID: 2110
		public static class ChannelTypes
		{
			// Token: 0x06002C16 RID: 11286 RVA: 0x000BDFAF File Offset: 0x000BC3AF
			public static bool IsPvP(Resources.ChannelType channel)
			{
				return Utils.Contains<Resources.ChannelType>(new Resources.ChannelType[]
				{
					Resources.ChannelType.PVP_Newbie,
					Resources.ChannelType.PVP_Skilled,
					Resources.ChannelType.PVP_Pro
				}, channel);
			}

			// Token: 0x06002C17 RID: 11287 RVA: 0x000BDFC8 File Offset: 0x000BC3C8
			public static bool IsPvE(Resources.ChannelType channel)
			{
				return channel == Resources.ChannelType.PVE;
			}

			// Token: 0x1700044B RID: 1099
			// (get) Token: 0x06002C18 RID: 11288 RVA: 0x000BDFCE File Offset: 0x000BC3CE
			public static IEnumerable<Resources.ChannelType> LobbyChannels
			{
				get
				{
					return new Resources.ChannelType[]
					{
						Resources.ChannelType.PVP_Newbie,
						Resources.ChannelType.PVP_Skilled,
						Resources.ChannelType.PVP_Pro,
						Resources.ChannelType.PVE
					};
				}
			}
		}

		// Token: 0x0200083F RID: 2111
		public static class MMQueuesNames
		{
			// Token: 0x1700044C RID: 1100
			// (get) Token: 0x06002C19 RID: 11289 RVA: 0x000BDFE1 File Offset: 0x000BC3E1
			public static string SendingPublicGamesQueueName
			{
				get
				{
					return string.Format("matchmaking.{0}.public.data", Resources.ChannelName);
				}
			}

			// Token: 0x1700044D RID: 1101
			// (get) Token: 0x06002C1A RID: 11290 RVA: 0x000BDFF2 File Offset: 0x000BC3F2
			public static string SendingRatingGamesQueueName
			{
				get
				{
					return string.Format("matchmaking.{0}.rating.data", Resources.ChannelName);
				}
			}

			// Token: 0x1700044E RID: 1102
			// (get) Token: 0x06002C1B RID: 11291 RVA: 0x000BE003 File Offset: 0x000BC403
			public static string ReceivingRoomUpdatesQueueName
			{
				get
				{
					return string.Format("matchmaking.{0}.updates", Resources.ServerName);
				}
			}
		}

		// Token: 0x02000840 RID: 2112
		private struct ResourceElement
		{
			// Token: 0x06002C1C RID: 11292 RVA: 0x000BE014 File Offset: 0x000BC414
			public ResourceElement(string _res_path, string _filename, Resources.ResFiles _type, Resources.ResSeverity _sev)
			{
				this.res_path = _res_path;
				this.filename = _filename;
				this.type = _type;
				this.severity = _sev;
			}

			// Token: 0x06002C1D RID: 11293 RVA: 0x000BE033 File Offset: 0x000BC433
			public ResourceElement(string _filename, Resources.ResFiles _type, Resources.ResSeverity _sev)
			{
				this = new Resources.ResourceElement(string.Empty, _filename, _type, _sev);
			}

			// Token: 0x040017E4 RID: 6116
			public string res_path;

			// Token: 0x040017E5 RID: 6117
			public string filename;

			// Token: 0x040017E6 RID: 6118
			public Resources.ResFiles type;

			// Token: 0x040017E7 RID: 6119
			public Resources.ResSeverity severity;
		}

		// Token: 0x02000841 RID: 2113
		public class PortData
		{
			// Token: 0x06002C1E RID: 11294 RVA: 0x000BE043 File Offset: 0x000BC443
			public PortData()
			{
				this.BindAddr = IPAddress.Loopback;
				this.Port = -1;
			}

			// Token: 0x06002C1F RID: 11295 RVA: 0x000BE05D File Offset: 0x000BC45D
			public bool IsValid()
			{
				return this.Port != -1;
			}

			// Token: 0x06002C20 RID: 11296 RVA: 0x000BE06C File Offset: 0x000BC46C
			public void Init(string connectionStr)
			{
				Match match = Regex.Match(connectionStr, "(([0-9\\.]+):)?([0-9]+)");
				if (!string.IsNullOrEmpty(match.Groups[2].Value))
				{
					this.BindAddr = IPAddress.Parse(match.Groups[2].Value);
				}
				this.Port = int.Parse(match.Groups[3].Value);
			}

			// Token: 0x06002C21 RID: 11297 RVA: 0x000BE0D8 File Offset: 0x000BC4D8
			public override string ToString()
			{
				return string.Format("{0}:{1}", this.BindAddr, this.Port);
			}

			// Token: 0x040017E8 RID: 6120
			public const int INVALID_PORT = -1;

			// Token: 0x040017E9 RID: 6121
			public IPAddress BindAddr;

			// Token: 0x040017EA RID: 6122
			public int Port;
		}
	}
}
