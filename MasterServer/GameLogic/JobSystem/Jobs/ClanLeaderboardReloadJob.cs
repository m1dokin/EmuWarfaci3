using System;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Jobs;
using MasterServer.Core.Services.Jobs.Attributes;
using MasterServer.Core.Services.Jobs.JobSchedulers;
using MasterServer.Core.Services.Jobs.Metrics;
using MasterServer.GameLogic.PerformanceSystem;

namespace MasterServer.GameLogic.JobSystem.Jobs
{
	// Token: 0x02000388 RID: 904
	[Job("clan_leaderboard_reload")]
	internal class ClanLeaderboardReloadJob : Job
	{
		// Token: 0x0600145A RID: 5210 RVA: 0x00052C68 File Offset: 0x00051068
		public ClanLeaderboardReloadJob(ConfigSection jobConfigSection, IJobScheduler jobScheduler, IClanPerformanceService clanPerformanceService, IJobMetricsTracker jobMetricsTracker) : base(jobConfigSection, jobScheduler, jobMetricsTracker)
		{
			this.m_clanPerformanceService = clanPerformanceService;
		}

		// Token: 0x0600145B RID: 5211 RVA: 0x00052C7B File Offset: 0x0005107B
		protected override JobResult ExecuteImpl()
		{
			this.m_clanPerformanceService.ReadMasterRecordFromDB();
			return JobResult.Finished;
		}

		// Token: 0x04000972 RID: 2418
		public const string CLAN_LEADERBOARD_RELOAD_JOB_NAME = "clan_leaderboard_reload";

		// Token: 0x04000973 RID: 2419
		private readonly IClanPerformanceService m_clanPerformanceService;
	}
}
