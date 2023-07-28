using System;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Jobs;
using MasterServer.Core.Services.Jobs.Attributes;
using MasterServer.Core.Services.Jobs.JobSchedulers;
using MasterServer.Core.Services.Jobs.Metrics;
using MasterServer.GameLogic.PerformanceSystem;

namespace MasterServer.GameLogic.JobSystem.Jobs
{
	// Token: 0x02000387 RID: 903
	[Job("clan_leaderboard_refresh")]
	internal class ClanLeaderboardRefreshJob : Job
	{
		// Token: 0x06001458 RID: 5208 RVA: 0x00052C47 File Offset: 0x00051047
		public ClanLeaderboardRefreshJob(ConfigSection jobConfigSection, IJobScheduler jobScheduler, IClanPerformanceService clanPerformanceService, IJobMetricsTracker jobMetricsTracker) : base(jobConfigSection, jobScheduler, jobMetricsTracker)
		{
			this.m_clanPerformanceService = clanPerformanceService;
		}

		// Token: 0x06001459 RID: 5209 RVA: 0x00052C5A File Offset: 0x0005105A
		protected override JobResult ExecuteImpl()
		{
			this.m_clanPerformanceService.RefreshMasterRecord();
			return JobResult.Finished;
		}

		// Token: 0x04000970 RID: 2416
		public const string CLAN_LEADERBOARD_REFRESH_JOB_NAME = "clan_leaderboard_refresh";

		// Token: 0x04000971 RID: 2417
		private readonly IClanPerformanceService m_clanPerformanceService;
	}
}
