using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Core.Services.Jobs;
using MasterServer.Core.Services.Jobs.JobSchedulers;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.PlayerStats;
using MasterServer.Users;
using OLAPHypervisor;
using Util.Common;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000540 RID: 1344
	[Service]
	[Singleton]
	internal class AbuseReportService : ServiceModule, IAbuseReportService
	{
		// Token: 0x06001D16 RID: 7446 RVA: 0x000756F3 File Offset: 0x00073AF3
		public AbuseReportService(IDALService dalService, IJobSchedulerService jobSchedulerService, ISessionInfoService sessionInfoService, ILogService logService, IPlayerStatsService playerStatsService)
		{
			this.m_dalService = dalService;
			this.m_jobSchedulerService = jobSchedulerService;
			this.m_sessionInfoService = sessionInfoService;
			this.m_logService = logService;
			this.m_playerStatsService = playerStatsService;
		}

		// Token: 0x06001D17 RID: 7447 RVA: 0x00075720 File Offset: 0x00073B20
		public override void Init()
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("AbuseReports");
			section.OnConfigChanged += this.OnConfigChanged;
			this.m_cacheTimeout = int.Parse(section.Get("CacheExpirationTime"));
			this.m_onlinePlayersTimeDictionary = new CacheDictionary<ulong, TimeSpan>(this.m_cacheTimeout);
			foreach (List<ConfigSection> list in Resources.AbuseManagerConfig.GetAllSections().Values)
			{
				foreach (ConfigSection configSection in list)
				{
					configSection.OnConfigChanged += this.OnConfigChanged;
				}
			}
			this.BuildConfigXml();
		}

		// Token: 0x06001D18 RID: 7448 RVA: 0x00075820 File Offset: 0x00073C20
		public override void Start()
		{
			base.Start();
			if (Resources.DBUpdaterPermission)
			{
				this.m_jobSchedulerService.AddJob("abuse_cleanup");
			}
		}

		// Token: 0x06001D19 RID: 7449 RVA: 0x00075842 File Offset: 0x00073C42
		public override void Stop()
		{
			base.Stop();
			this.m_onlinePlayersTimeDictionary.Dispose();
		}

		// Token: 0x06001D1A RID: 7450 RVA: 0x00075858 File Offset: 0x00073C58
		public Task<EAbuseReportingResult> ProcessReport(UserInfo.User from, ulong to_uid, ulong to_pid, string to_nickname, string type, string comment)
		{
			ushort num = 0;
			ushort num2 = 0;
			foreach (SAbuseReport sabuseReport in this.m_dalService.AbuseSystem.GetAbuseReports(from.ProfileID))
			{
				num += 1;
				if (sabuseReport.To == to_pid)
				{
					num2 += 1;
				}
			}
			if (num >= this.m_reportsPerDay)
			{
				return Task.FromResult<EAbuseReportingResult>(EAbuseReportingResult.eARR_PerDayLimitReached);
			}
			if (num2 >= this.m_reportsPerPlayer)
			{
				return Task.FromResult<EAbuseReportingResult>(EAbuseReportingResult.eARR_PerPlayerLimitReached);
			}
			return this.m_sessionInfoService.GetProfileInfoAsync(to_nickname).ContinueWith<EAbuseReportingResult>(delegate(Task<ProfileInfo> t)
			{
				ProfileInfo result = t.Result;
				string[] param = new string[]
				{
					from.Nickname,
					from.Rank.ToString(),
					from.IP,
					result.Nickname,
					result.RankId.ToString(),
					result.IPAddress,
					this.GetTotalOnlineTime(from.ProfileID).ToString(),
					this.GetTotalOnlineTime(result.ProfileID).ToString(),
					comment
				};
				this.StoreReportToDB(from.ProfileID, to_pid, type, EAbuseReportSource.eARS_User, param);
				return EAbuseReportingResult.eARR_Success;
			}).ContinueWith<EAbuseReportingResult>(delegate(Task<EAbuseReportingResult> t)
			{
				this.m_dalService.AbuseSystem.AddAbuseReport(from.ProfileID, to_pid, type);
				this.m_logService.Event.AbuseReportLog(from.UserID, from.ProfileID, from.IP, to_uid, to_pid, type, comment);
				return t.Result;
			});
		}

		// Token: 0x06001D1B RID: 7451 RVA: 0x0007596C File Offset: 0x00073D6C
		public void StoreReportToDB(ulong from_pid, ulong to_pid, string type, EAbuseReportSource source, params string[] param)
		{
			this.m_dalService.AbuseSystem.AddReportToHistory(from_pid, to_pid, type, (uint)source, string.Join(";", param));
		}

		// Token: 0x06001D1C RID: 7452 RVA: 0x0007598F File Offset: 0x00073D8F
		public TimeSpan GetTotalOnlineTime(ulong profileId)
		{
			return this.GetTotalOnlineTime(profileId, false);
		}

		// Token: 0x06001D1D RID: 7453 RVA: 0x0007599C File Offset: 0x00073D9C
		public TimeSpan GetTotalOnlineTime(ulong profileId, bool forceCacheReset)
		{
			TimeSpan timeSpan;
			if (forceCacheReset || !this.m_onlinePlayersTimeDictionary.TryGetValue(profileId, out timeSpan))
			{
				ulong totalOnlineTimeFromTelem = this.GetTotalOnlineTimeFromTelem(profileId);
				try
				{
					timeSpan = TimeUtils.UTCTimestampToTimeSpan(totalOnlineTimeFromTelem);
				}
				catch (OverflowException innerException)
				{
					MasterServer.Core.Log.Warning(new ApplicationException(string.Format("Can't parse {0}'s total online time from {1} value.", profileId, totalOnlineTimeFromTelem), innerException));
					timeSpan = TimeSpan.Zero;
				}
				this.m_onlinePlayersTimeDictionary.Add(profileId, timeSpan);
			}
			return timeSpan;
		}

		// Token: 0x06001D1E RID: 7454 RVA: 0x00075A24 File Offset: 0x00073E24
		public void DebugCleanupHistory()
		{
			if (!Resources.DBUpdaterPermission)
			{
				MasterServer.Core.Log.Warning("Abuse history cleanup supported on DB updater only.");
				return;
			}
			this.m_jobSchedulerService.AddJob("abuse_cleanup", new SimpleJobScheduler());
		}

		// Token: 0x06001D1F RID: 7455 RVA: 0x00075A50 File Offset: 0x00073E50
		public bool CleanupHistory(TimeSpan abuseReportLifetime, TimeSpan dbTimeout, int batchSize)
		{
			if (!Resources.DBUpdaterPermission)
			{
				MasterServer.Core.Log.Warning("Abuse history cleanup supported on DB updater only.");
				return true;
			}
			bool result;
			lock (this)
			{
				MasterServer.Core.Log.Info("Starting abuse history cleanup ...");
				bool flag2 = this.m_dalService.AbuseSystem.ClearAbuseReportHistory(abuseReportLifetime, dbTimeout, batchSize);
				MasterServer.Core.Log.Info("Abuse history cleanup finished.");
				result = flag2;
			}
			return result;
		}

		// Token: 0x06001D20 RID: 7456 RVA: 0x00075ACC File Offset: 0x00073ECC
		private ulong GetTotalOnlineTimeFromTelem(ulong profile_id)
		{
			List<Measure> playerStats = this.m_playerStatsService.GetPlayerStats(profile_id);
			foreach (Measure measure in playerStats)
			{
				if (measure.Dimensions["stat"] == "player_online_time")
				{
					return (ulong)(measure.Value / 10L);
				}
			}
			return 0UL;
		}

		// Token: 0x06001D21 RID: 7457 RVA: 0x00075B60 File Offset: 0x00073F60
		private void BuildConfigXml()
		{
			this.m_reportsPerDay = ushort.Parse(Resources.AbuseManagerConfig.GetSection("limits").Get("reports_per_day"));
			this.m_reportsPerPlayer = ushort.Parse(Resources.AbuseManagerConfig.GetSection("limits").Get("reports_per_player"));
		}

		// Token: 0x06001D22 RID: 7458 RVA: 0x00075BB5 File Offset: 0x00073FB5
		private void OnConfigChanged(ConfigEventArgs e)
		{
			this.BuildConfigXml();
		}

		// Token: 0x04000DE2 RID: 3554
		private readonly IDALService m_dalService;

		// Token: 0x04000DE3 RID: 3555
		private readonly IJobSchedulerService m_jobSchedulerService;

		// Token: 0x04000DE4 RID: 3556
		private readonly ISessionInfoService m_sessionInfoService;

		// Token: 0x04000DE5 RID: 3557
		private readonly ILogService m_logService;

		// Token: 0x04000DE6 RID: 3558
		private readonly IPlayerStatsService m_playerStatsService;

		// Token: 0x04000DE7 RID: 3559
		private ushort m_reportsPerDay;

		// Token: 0x04000DE8 RID: 3560
		private ushort m_reportsPerPlayer;

		// Token: 0x04000DE9 RID: 3561
		private int m_cacheTimeout;

		// Token: 0x04000DEA RID: 3562
		private CacheDictionary<ulong, TimeSpan> m_onlinePlayersTimeDictionary;
	}
}
