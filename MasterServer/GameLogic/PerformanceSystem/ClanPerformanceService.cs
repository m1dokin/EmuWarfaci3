using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Diagnostics.Profiler;
using MasterServer.Core.Diagnostics.Threading;
using MasterServer.Core.Services;
using MasterServer.Core.Services.Jobs;
using MasterServer.Core.Services.Jobs.Exceptions;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.ClanSystem;
using Util.Common;

namespace MasterServer.GameLogic.PerformanceSystem
{
	// Token: 0x020003EB RID: 1003
	[Service]
	[Singleton]
	internal class ClanPerformanceService : ServiceModule, IClanPerformanceService
	{
		// Token: 0x060015C6 RID: 5574 RVA: 0x0005A848 File Offset: 0x00058C48
		public ClanPerformanceService(IJobSchedulerService jobSchedulerService, IDALService dalService, IClanService clanService, IMemcachedService memcachedService)
		{
			this.m_jobSchedulerService = jobSchedulerService;
			this.m_dalService = dalService;
			this.m_clanService = clanService;
			this.m_memcachedService = memcachedService;
			this.m_cache_lock = ((!Resources.UseMonoRWLock) ? new Util.Common.ReaderWriterLockSlim(Util.Common.LockRecursionPolicy.SupportsRecursion) : new NativeReaderWriterLockSlim(Util.Common.LockRecursionPolicy.SupportsRecursion));
		}

		// Token: 0x14000048 RID: 72
		// (add) Token: 0x060015C7 RID: 5575 RVA: 0x0005A8A4 File Offset: 0x00058CA4
		// (remove) Token: 0x060015C8 RID: 5576 RVA: 0x0005A8DC File Offset: 0x00058CDC
		public event PerformanceServiceStatsDeleg OnServiceStats;

		// Token: 0x060015C9 RID: 5577 RVA: 0x0005A914 File Offset: 0x00058D14
		public override void Init()
		{
			base.Init();
			this.m_domain = cache_domains.clan_performance_master_record;
			this.m_topClans = new List<ClanInfo>();
			this.m_performanceServiceSection = Resources.LBSettings.GetSection("PerformanceService");
			int minutes = int.Parse(this.m_performanceServiceSection.Get("MasterRecordUpdateMin"));
			this.m_updateFrequency = new TimeSpan(0, minutes, 0);
			this.m_performanceServiceSection.OnConfigChanged += this.OnPerformanceConfigChanged;
			this.m_leaderboardSection = Resources.LBSettings.GetSection("Leaderboard");
			this.m_clanLeaderBoardSize = int.Parse(this.m_leaderboardSection.Get("ClanLeaderBoardSize"));
			this.m_leaderboardInitialPrecision = int.Parse(this.m_leaderboardSection.Get("InitialPrecision"));
			this.m_leaderboardSection.OnConfigChanged += this.OnLeaderboardConfigChanged;
			this.m_clanService.ClanRemoved += this.ClanRemoved;
			this.m_useJobs = true;
		}

		// Token: 0x060015CA RID: 5578 RVA: 0x0005AA10 File Offset: 0x00058E10
		public override void Start()
		{
			if (Resources.DBUpdaterPermission)
			{
				this.ForceRefreshMasterRecord();
			}
			try
			{
				this.m_jobSchedulerService.AddJob("clan_leaderboard_refresh");
				this.m_jobSchedulerService.AddJob("clan_leaderboard_reload");
			}
			catch (JobNotFoundException ex)
			{
				this.m_useJobs = false;
			}
			Log.Info<bool>("Clan performance service started with jobs mode: {0}", this.m_useJobs);
		}

		// Token: 0x060015CB RID: 5579 RVA: 0x0005AA80 File Offset: 0x00058E80
		public override void Stop()
		{
			this.m_performanceServiceSection.OnConfigChanged -= this.OnPerformanceConfigChanged;
			this.m_leaderboardSection.OnConfigChanged -= this.OnLeaderboardConfigChanged;
			this.m_clanService.ClanRemoved -= this.ClanRemoved;
			this.OnServiceStats = null;
			base.Stop();
		}

		// Token: 0x060015CC RID: 5580 RVA: 0x0005AAE0 File Offset: 0x00058EE0
		public ClanPerformanceInfo GetClanPerformance(ulong clan_id)
		{
			int position = 0;
			using (new scoped_upgradable_lock(this.m_cache_lock))
			{
				if (!this.m_useJobs)
				{
					this.RefreshMasterRecordInternal();
				}
				if (clan_id > 0UL)
				{
					ClanInfo clanInfo = this.m_dalService.ClanSystem.GetClanInfo(clan_id);
					if (clanInfo != null)
					{
						position = this.PredictClanPosition(clanInfo.ClanPoints);
					}
				}
			}
			return new ClanPerformanceInfo(position, this.m_topClans);
		}

		// Token: 0x060015CD RID: 5581 RVA: 0x0005AB6C File Offset: 0x00058F6C
		public bool RefreshLeaderboard()
		{
			object update_lock = this.m_update_lock;
			lock (update_lock)
			{
				if (this.m_localRefresh)
				{
					return false;
				}
				this.m_localRefresh = true;
			}
			if (!ThreadPoolProxy.QueueUserWorkItem(new WaitCallback(this.RefreshLeaderboardTask)))
			{
				object update_lock2 = this.m_update_lock;
				lock (update_lock2)
				{
					this.m_localRefresh = false;
					return false;
				}
			}
			return true;
		}

		// Token: 0x060015CE RID: 5582 RVA: 0x0005AC14 File Offset: 0x00059014
		public void RefreshMasterRecord()
		{
			MasterRecord masterRecord = this.ReadMasterRecordFromDB();
			if (DateTime.UtcNow - masterRecord.MinLastUpdateUtc >= this.m_updateFrequency)
			{
				Log.Info<DateTime>("DAL master record is outdated, update time: {0}", masterRecord.MinLastUpdateUtc);
				this.RefreshLeaderboard();
			}
			else
			{
				Log.Info("DAL master record is uptodate");
			}
		}

		// Token: 0x060015CF RID: 5583 RVA: 0x0005AC70 File Offset: 0x00059070
		public MasterRecord ReadMasterRecordFromDB()
		{
			MasterRecord performanceMasterRecord = this.GetPerformanceMasterRecord();
			bool flag;
			using (new scoped_upgradable_lock(this.m_cache_lock))
			{
				flag = (performanceMasterRecord.MinLastUpdateUtc > this.m_cachedMasterRecord.MinLastUpdateUtc);
				if (flag)
				{
					using (new scoped_write_lock(this.m_cache_lock))
					{
						Log.Info<DateTime, DateTime>("Local master record is outdated: {0}, DAL version update time: {1}", this.m_cachedMasterRecord.MinLastUpdateUtc, performanceMasterRecord.MinLastUpdateUtc);
						this.m_cachedMasterRecord = performanceMasterRecord;
					}
				}
				else
				{
					Log.Info("Local master record is uptodate");
				}
			}
			if (flag)
			{
				this.UpdateTopClans();
			}
			return performanceMasterRecord;
		}

		// Token: 0x060015D0 RID: 5584 RVA: 0x0005AD3C File Offset: 0x0005913C
		public void ForceRefreshMasterRecord()
		{
			Log.Info<DateTime>("Force refresh DAL master record, update time: {0}", this.ReadMasterRecordFromDB().MinLastUpdateUtc);
			this.RefreshLeaderboard();
		}

		// Token: 0x060015D1 RID: 5585 RVA: 0x0005AD68 File Offset: 0x00059168
		private int PredictClanPosition(ulong clan_points)
		{
			if (this.m_cachedMasterRecord.Records == null)
			{
				return 0;
			}
			foreach (MasterRecord.Record record in this.m_cachedMasterRecord.Records)
			{
				if (!(record.RecordID != "clan.clan"))
				{
					if (record.StatSamples.Count == 0)
					{
						Log.Warning("ClanPerformanceService: Record doesn't contains prediction polynomial coefficients");
						break;
					}
					return PositionApproximation.GetPosition(record.StatSamples[0].Samples, clan_points);
				}
			}
			return 0;
		}

		// Token: 0x060015D2 RID: 5586 RVA: 0x0005AE34 File Offset: 0x00059234
		private List<KeyValuePair<float, float>> ProcessClanStats(List<ulong> entries)
		{
			List<KeyValuePair<float, float>> list = new List<KeyValuePair<float, float>>(this.m_leaderboardInitialPrecision);
			if (entries.Count < this.m_leaderboardInitialPrecision)
			{
				list.AddRange(entries.Select((ulong t, int i) => new KeyValuePair<float, float>((float)(i + 1), t)));
				return list;
			}
			IEnumerable<KeyValuePair<float, float>> collection = PositionApproximation.CalculateSlice(this.m_leaderboardInitialPrecision, 0, entries.Count / this.m_leaderboardInitialPrecision, entries);
			list.AddRange(collection);
			list.Add(new KeyValuePair<float, float>((float)entries.Count, entries[entries.Count - 1]));
			return list;
		}

		// Token: 0x060015D3 RID: 5587 RVA: 0x0005AECE File Offset: 0x000592CE
		private void RefreshMasterRecordInternal()
		{
			if (DateTime.UtcNow - this.m_cachedMasterRecord.MinLastUpdateUtc < this.m_updateFrequency)
			{
				return;
			}
			this.RefreshMasterRecord();
		}

		// Token: 0x060015D4 RID: 5588 RVA: 0x0005AEFC File Offset: 0x000592FC
		private MasterRecord GetPerformanceMasterRecord()
		{
			return this.m_dalService.PerformanceSystem.GetPerformanceMasterRecord(string.Format("{0}.%", "clan"), this.m_domain);
		}

		// Token: 0x060015D5 RID: 5589 RVA: 0x0005AF23 File Offset: 0x00059323
		private string GetLockString()
		{
			return string.Format("{0}.lock", "clan");
		}

		// Token: 0x060015D6 RID: 5590 RVA: 0x0005AF34 File Offset: 0x00059334
		private bool DoRefreshLeaderboard()
		{
			List<MasterRecord.Record> list = new List<MasterRecord.Record>(1);
			Log.Info("Performance clan leaderboard updating ...");
			if (!this.m_dalService.PerformanceSystem.TryBeginUpdate(Resources.Jid, this.GetLockString(), this.m_updateFrequency))
			{
				Log.Info("Performance clan leaderboard another update is already running. Skipping task.");
				return false;
			}
			try
			{
				MasterRecord.Record item = new MasterRecord.Record
				{
					RecordID = "clan.clan",
					StatSamples = new List<MasterRecord.StatSamples>()
				};
				list.Add(item);
				List<ulong> entries = new List<ulong>(this.m_dalService.ClanSystem.GetClansForLeaderboardPrediction());
				MasterRecord.StatSamples item2 = default(MasterRecord.StatSamples);
				item2.Samples = this.ProcessClanStats(entries);
				item.StatSamples.Add(item2);
			}
			finally
			{
				this.m_dalService.PerformanceSystem.EndUpdate(list, this.GetLockString(), new List<cache_domain>
				{
					this.m_domain,
					cache_domains.clan_top
				});
				this.UpdateTopClans();
			}
			MasterRecord performanceMasterRecord = this.GetPerformanceMasterRecord();
			using (new scoped_write_lock(this.m_cache_lock))
			{
				this.m_cachedMasterRecord = performanceMasterRecord;
			}
			Log.Info("Performance clan leaderboard update finished.");
			return true;
		}

		// Token: 0x060015D7 RID: 5591 RVA: 0x0005B084 File Offset: 0x00059484
		private void RefreshLeaderboardTask(object dummy)
		{
			using (new ThreadTracker.Tracker("Leaderboard"))
			{
				try
				{
					TimeExecution timeExecution = new TimeExecution();
					if (this.DoRefreshLeaderboard() && this.OnServiceStats != null)
					{
						this.OnServiceStats(timeExecution.Stop());
					}
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
				finally
				{
					object update_lock = this.m_update_lock;
					lock (update_lock)
					{
						this.m_localRefresh = false;
					}
				}
			}
		}

		// Token: 0x060015D8 RID: 5592 RVA: 0x0005B14C File Offset: 0x0005954C
		private void UpdateTopClans()
		{
			this.m_topClans = this.m_dalService.ClanSystem.GetClanTop(this.m_clanLeaderBoardSize);
		}

		// Token: 0x060015D9 RID: 5593 RVA: 0x0005B16A File Offset: 0x0005956A
		private void OnPerformanceConfigChanged(ConfigEventArgs e)
		{
			if (string.Equals(e.Name, "ClanLeaderBoardSize", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_clanLeaderBoardSize = e.iValue;
			}
		}

		// Token: 0x060015DA RID: 5594 RVA: 0x0005B190 File Offset: 0x00059590
		private void OnLeaderboardConfigChanged(ConfigEventArgs e)
		{
			if (string.Equals(e.Name, "MasterRecordUpdateMin", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_updateFrequency = new TimeSpan(0, e.iValue, 0);
			}
			if (string.Equals(e.Name, "InitialPrecision", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_leaderboardInitialPrecision = e.iValue;
			}
		}

		// Token: 0x060015DB RID: 5595 RVA: 0x0005B1E8 File Offset: 0x000595E8
		private void ClanRemoved(ulong clan_id)
		{
			bool flag = false;
			foreach (ClanInfo clanInfo in this.m_topClans)
			{
				if (clanInfo.ClanID == clan_id)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				if (this.m_memcachedService != null && this.m_memcachedService.Connected)
				{
					this.m_memcachedService.Remove(cache_domains.clan_top);
				}
				this.m_topClans = this.m_dalService.ClanSystem.GetClanTop(this.m_clanLeaderBoardSize);
			}
		}

		// Token: 0x04000A5C RID: 2652
		private const string m_clanIdentifier = "clan.clan";

		// Token: 0x04000A5D RID: 2653
		private const string m_typeString = "clan";

		// Token: 0x04000A5E RID: 2654
		private TimeSpan m_updateFrequency;

		// Token: 0x04000A5F RID: 2655
		private bool m_localRefresh;

		// Token: 0x04000A60 RID: 2656
		private bool m_useJobs;

		// Token: 0x04000A61 RID: 2657
		private int m_leaderboardInitialPrecision;

		// Token: 0x04000A62 RID: 2658
		private int m_clanLeaderBoardSize;

		// Token: 0x04000A63 RID: 2659
		private MasterRecord m_cachedMasterRecord;

		// Token: 0x04000A64 RID: 2660
		private IEnumerable<ClanInfo> m_topClans;

		// Token: 0x04000A65 RID: 2661
		private cache_domain m_domain;

		// Token: 0x04000A66 RID: 2662
		private ConfigSection m_performanceServiceSection;

		// Token: 0x04000A67 RID: 2663
		private ConfigSection m_leaderboardSection;

		// Token: 0x04000A68 RID: 2664
		private readonly object m_update_lock = new object();

		// Token: 0x04000A69 RID: 2665
		private readonly IReaderWriterLockSlim m_cache_lock;

		// Token: 0x04000A6A RID: 2666
		private readonly IJobSchedulerService m_jobSchedulerService;

		// Token: 0x04000A6B RID: 2667
		private readonly IDALService m_dalService;

		// Token: 0x04000A6C RID: 2668
		private readonly IClanService m_clanService;

		// Token: 0x04000A6D RID: 2669
		private readonly IMemcachedService m_memcachedService;
	}
}
