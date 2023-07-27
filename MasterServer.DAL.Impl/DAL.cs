using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using HK2Net;
using MasterServer.Core;
using MasterServer.DAL.CustomRules;
using MasterServer.DAL.Exceptions;
using MasterServer.DAL.FirstWinOfDayByMode;
using MasterServer.DAL.Impl.CustomRules;
using MasterServer.DAL.Impl.FirstWinOfDayByMode;
using MasterServer.DAL.RatingSystem;
using MasterServer.DAL.VoucherSystem;
using MasterServer.Database;
using MasterServer.ElectronicCatalog;

namespace MasterServer.DAL.Impl
{
	// Token: 0x0200000F RID: 15
	[Service]
	public class DAL : IDAL, IDisposable
	{
		// Token: 0x06000063 RID: 99 RVA: 0x00004950 File Offset: 0x00002B50
		public DAL()
		{
			this.m_cacheProxy = new CacheProxy();
			this.m_itemSystem = new CItemSystem(this);
			this.m_achivementSystem = new AchievementsSystem(this);
			this.m_rewardsSystem = new RewardsSystem(this);
			this.m_profileSystem = new ProfileSystem(this);
			this.m_coldStorageSystem = new ColdStorageSystem(this);
			this.m_clanSystem = new ClanSystem(this);
			this.m_contractSystem = new ContractSystem(this);
			this.m_notificationSystem = new NotificationSystem(this);
			this.m_announcementSystem = new AnnouncementSystem(this);
			this.m_abuseSystem = new AbuseReportSystem(this);
			this.m_commonSystem = new CommonSystem(this);
			this.m_performanceSystem = new PerformanceSystem(this);
			this.m_eCatalog = new ECatalog(this);
			this.m_missionSystem = new MissionSystem(this);
			this.m_telemetrySystem = new TelemetrySystem(this);
			this.m_profileProgressionSystem = new ProfileProgressionSystem(this);
			this.m_playerStatsSystem = new PlayerStatsSystem(this);
			this.m_customRulesSystem = new CustomRulesSystem(this);
			this.m_skillSystem = new SkillSystem(this);
			this.m_ratingSystem = new RatingSystem(this);
			this.m_voucherSystem = new VoucherSystem(this);
			this.m_ratingGameBanSystem = new RatingGameBanSystem(this);
			this.m_firstWinOfDayByModeSystem = new FirstWinOfDayByModeSystem(this);
			ServicesManager.OnExecutionPhaseChanged += this.OnExecutionPhaseChanged;
			this.OnExecutionPhaseChanged(ServicesManager.ExecutionPhase);
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000064 RID: 100 RVA: 0x00004AB4 File Offset: 0x00002CB4
		public ECatConnectionPool ConnectionPool
		{
			get
			{
				if (this.m_pool == null)
				{
					object @lock = this.m_lock;
					lock (@lock)
					{
						if (this.m_pool == null)
						{
							this.m_pool = new ECatConnectionPool();
							this.m_pool.Init();
						}
					}
				}
				return this.m_pool;
			}
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00004B24 File Offset: 0x00002D24
		public void Start()
		{
			this.m_telemetrySystem.Start();
			this.m_playerStatsSystem.Start();
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00004B3C File Offset: 0x00002D3C
		public void Dispose()
		{
			Log.Info("Disposing DAL");
			if (this.m_telemetrySystem != null)
			{
				this.m_telemetrySystem.Dispose();
				this.m_telemetrySystem = null;
			}
			ServicesManager.OnExecutionPhaseChanged -= this.OnExecutionPhaseChanged;
			this.SetPooling(false);
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00004B88 File Offset: 0x00002D88
		private void OnExecutionPhaseChanged(ExecutionPhase execution_phase)
		{
			if (execution_phase >= ExecutionPhase.PostUpdate && execution_phase < ExecutionPhase.Stopping && !Resources.MasterConnectionPool.ConnectionConfig.Pooling)
			{
				Log.Info("Enabling connection pooling");
				this.SetPooling(true);
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00004BCC File Offset: 0x00002DCC
		private void SetPooling(bool pool)
		{
			ConnectionPool.Config connectionConfig = Resources.MasterConnectionPool.ConnectionConfig;
			connectionConfig.Pooling = pool;
			Resources.MasterConnectionPool.SetConfig(connectionConfig);
			foreach (ConnectionPool connectionPool in Resources.SlaveConnectionPools)
			{
				connectionConfig = connectionPool.ConnectionConfig;
				connectionConfig.Pooling = pool;
				connectionPool.SetConfig(connectionConfig);
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000069 RID: 105 RVA: 0x00004C2C File Offset: 0x00002E2C
		private Dictionary<string, List<SFixedSizeColumnInfo>> FixedLengthColumns
		{
			get
			{
				object syncObject = this.m_syncObject;
				lock (syncObject)
				{
					if (this.m_varbinaryColumns == null)
					{
						CacheProxy.Options<SFixedSizeColumnInfo> options = new CacheProxy.Options<SFixedSizeColumnInfo>
						{
							db_serializer = new VarbinaryColumnInfoSerializer()
						};
						options.query("SELECT table_name, column_name, character_maximum_length FROM information_schema.COLUMNS WHERE table_schema = database() AND (data_type LIKE 'varbinary' OR data_type LIKE 'varchar')", new object[0]);
						DALResultMulti<SFixedSizeColumnInfo> stream = this.CacheProxy.GetStream<SFixedSizeColumnInfo>(options);
						IEnumerable<IGrouping<string, SFixedSizeColumnInfo>> source = from i in stream.Values
						group i by i.TableName;
						Func<IGrouping<string, SFixedSizeColumnInfo>, string> keySelector = (IGrouping<string, SFixedSizeColumnInfo> g) => g.Key;
						if (DAL.<>f__mg$cache0 == null)
						{
							DAL.<>f__mg$cache0 = new Func<IGrouping<string, SFixedSizeColumnInfo>, List<SFixedSizeColumnInfo>>(Enumerable.ToList<SFixedSizeColumnInfo>);
						}
						this.m_varbinaryColumns = source.ToDictionary(keySelector, DAL.<>f__mg$cache0);
					}
					if (this.m_varbinaryColumns.Count == 0)
					{
						throw new InvalidOperationException("There were no VARBINARY columns found in the database.");
					}
				}
				return this.m_varbinaryColumns;
			}
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00004D38 File Offset: 0x00002F38
		public void ValidateFixedSizeColumnData(string tableName, string columnName, int dataSize)
		{
			List<SFixedSizeColumnInfo> source;
			if (!this.FixedLengthColumns.TryGetValue(tableName, out source) || !source.Any((SFixedSizeColumnInfo ci) => ci.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase) && ci.MaxLength >= dataSize))
			{
				throw new DALBinaryDataLengthViolationException(tableName, columnName, dataSize);
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600006B RID: 107 RVA: 0x00004D9B File Offset: 0x00002F9B
		internal CacheProxy CacheProxy
		{
			get
			{
				return this.m_cacheProxy;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600006C RID: 108 RVA: 0x00004DA3 File Offset: 0x00002FA3
		public IItemSystem ItemSystem
		{
			get
			{
				return this.m_itemSystem;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600006D RID: 109 RVA: 0x00004DAB File Offset: 0x00002FAB
		public IAchievementsSystem AchievementSystem
		{
			get
			{
				return this.m_achivementSystem;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600006E RID: 110 RVA: 0x00004DB3 File Offset: 0x00002FB3
		public IRewardsSystem RewardsSystem
		{
			get
			{
				return this.m_rewardsSystem;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600006F RID: 111 RVA: 0x00004DBB File Offset: 0x00002FBB
		public IProfileSystem ProfileSystem
		{
			get
			{
				return this.m_profileSystem;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000070 RID: 112 RVA: 0x00004DC3 File Offset: 0x00002FC3
		public IColdStorageSystem ColdStorageSystem
		{
			get
			{
				return this.m_coldStorageSystem;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000071 RID: 113 RVA: 0x00004DCB File Offset: 0x00002FCB
		public IClanSystem ClanSystem
		{
			get
			{
				return this.m_clanSystem;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000072 RID: 114 RVA: 0x00004DD3 File Offset: 0x00002FD3
		public IContractSystem ContractSystem
		{
			get
			{
				return this.m_contractSystem;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000073 RID: 115 RVA: 0x00004DDB File Offset: 0x00002FDB
		public INotificationSystem NotificationSystem
		{
			get
			{
				return this.m_notificationSystem;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000074 RID: 116 RVA: 0x00004DE3 File Offset: 0x00002FE3
		public IAnnouncmentSystem AnnouncementSystem
		{
			get
			{
				return this.m_announcementSystem;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000075 RID: 117 RVA: 0x00004DEB File Offset: 0x00002FEB
		public ICommonSystem CommonSystem
		{
			get
			{
				return this.m_commonSystem;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000076 RID: 118 RVA: 0x00004DF3 File Offset: 0x00002FF3
		public IAbuseReportSystem AbuseSystem
		{
			get
			{
				return this.m_abuseSystem;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000077 RID: 119 RVA: 0x00004DFB File Offset: 0x00002FFB
		public IPerformanceSystem PerformanceSystem
		{
			get
			{
				return this.m_performanceSystem;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000078 RID: 120 RVA: 0x00004E03 File Offset: 0x00003003
		public IECatalog ECatalog
		{
			get
			{
				return this.m_eCatalog;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000079 RID: 121 RVA: 0x00004E0B File Offset: 0x0000300B
		public IMissionSystem MissionSystem
		{
			get
			{
				return this.m_missionSystem;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600007A RID: 122 RVA: 0x00004E13 File Offset: 0x00003013
		public ITelemetrySystem TelemetrySystem
		{
			get
			{
				return this.m_telemetrySystem;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600007B RID: 123 RVA: 0x00004E1B File Offset: 0x0000301B
		public IProfileProgressionSystem ProfileProgressionSystem
		{
			get
			{
				return this.m_profileProgressionSystem;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600007C RID: 124 RVA: 0x00004E23 File Offset: 0x00003023
		public IPlayerStatsSystem PlayerStatsSystem
		{
			get
			{
				return this.m_playerStatsSystem;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600007D RID: 125 RVA: 0x00004E2B File Offset: 0x0000302B
		public ICustomRulesSystem CustomRulesSystem
		{
			get
			{
				return this.m_customRulesSystem;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600007E RID: 126 RVA: 0x00004E33 File Offset: 0x00003033
		public ISkillSystem SkillSystem
		{
			get
			{
				return this.m_skillSystem;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600007F RID: 127 RVA: 0x00004E3B File Offset: 0x0000303B
		public IRatingSystem RatingSystem
		{
			get
			{
				return this.m_ratingSystem;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000080 RID: 128 RVA: 0x00004E43 File Offset: 0x00003043
		public IVoucherSystem VoucherSystem
		{
			get
			{
				return this.m_voucherSystem;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000081 RID: 129 RVA: 0x00004E4B File Offset: 0x0000304B
		public IRatingGameBanSystem RatingGameBanSystem
		{
			get
			{
				return this.m_ratingGameBanSystem;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000082 RID: 130 RVA: 0x00004E53 File Offset: 0x00003053
		public IFirstWinOfDayByModeSystem FirstWinOfDayByModeSystem
		{
			get
			{
				return this.m_firstWinOfDayByModeSystem;
			}
		}

		// Token: 0x04000021 RID: 33
		private CacheProxy m_cacheProxy;

		// Token: 0x04000022 RID: 34
		private CItemSystem m_itemSystem;

		// Token: 0x04000023 RID: 35
		private AchievementsSystem m_achivementSystem;

		// Token: 0x04000024 RID: 36
		private RewardsSystem m_rewardsSystem;

		// Token: 0x04000025 RID: 37
		private ProfileSystem m_profileSystem;

		// Token: 0x04000026 RID: 38
		private ColdStorageSystem m_coldStorageSystem;

		// Token: 0x04000027 RID: 39
		private ClanSystem m_clanSystem;

		// Token: 0x04000028 RID: 40
		private ContractSystem m_contractSystem;

		// Token: 0x04000029 RID: 41
		private NotificationSystem m_notificationSystem;

		// Token: 0x0400002A RID: 42
		private AnnouncementSystem m_announcementSystem;

		// Token: 0x0400002B RID: 43
		private AbuseReportSystem m_abuseSystem;

		// Token: 0x0400002C RID: 44
		private CommonSystem m_commonSystem;

		// Token: 0x0400002D RID: 45
		private PerformanceSystem m_performanceSystem;

		// Token: 0x0400002E RID: 46
		private ECatalog m_eCatalog;

		// Token: 0x0400002F RID: 47
		private MissionSystem m_missionSystem;

		// Token: 0x04000030 RID: 48
		private TelemetrySystem m_telemetrySystem;

		// Token: 0x04000031 RID: 49
		private ProfileProgressionSystem m_profileProgressionSystem;

		// Token: 0x04000032 RID: 50
		private PlayerStatsSystem m_playerStatsSystem;

		// Token: 0x04000033 RID: 51
		private CustomRulesSystem m_customRulesSystem;

		// Token: 0x04000034 RID: 52
		private SkillSystem m_skillSystem;

		// Token: 0x04000035 RID: 53
		private RatingSystem m_ratingSystem;

		// Token: 0x04000036 RID: 54
		private VoucherSystem m_voucherSystem;

		// Token: 0x04000037 RID: 55
		private RatingGameBanSystem m_ratingGameBanSystem;

		// Token: 0x04000038 RID: 56
		private FirstWinOfDayByModeSystem m_firstWinOfDayByModeSystem;

		// Token: 0x04000039 RID: 57
		private readonly object m_syncObject = new object();

		// Token: 0x0400003A RID: 58
		private Dictionary<string, List<SFixedSizeColumnInfo>> m_varbinaryColumns;

		// Token: 0x0400003B RID: 59
		private object m_lock = new object();

		// Token: 0x0400003C RID: 60
		private ECatConnectionPool m_pool;

		// Token: 0x0400003D RID: 61
		[CompilerGenerated]
		private static Func<IGrouping<string, SFixedSizeColumnInfo>, List<SFixedSizeColumnInfo>> <>f__mg$cache0;
	}
}
