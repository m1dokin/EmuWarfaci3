using System;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Jobs;
using MasterServer.Core.Services.Jobs.Attributes;
using MasterServer.Core.Services.Jobs.JobSchedulers;
using MasterServer.Core.Services.Jobs.Metrics;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.JobSystem.Jobs
{
	// Token: 0x02000394 RID: 916
	[Job("mission_reloading_from_db", FailureStrategy = JobFailureStrategy.Finish)]
	internal class MissionReloadingFromDBJob : Job
	{
		// Token: 0x06001477 RID: 5239 RVA: 0x000530D1 File Offset: 0x000514D1
		public MissionReloadingFromDBJob(ConfigSection jobConfigSection, IJobScheduler jobScheduler, IMissionGenerationService missionGenerationService, IJobMetricsTracker jobMetricsTracker) : base(jobConfigSection, jobScheduler, jobMetricsTracker)
		{
			this.m_missionGenerationService = missionGenerationService;
		}

		// Token: 0x06001478 RID: 5240 RVA: 0x000530E4 File Offset: 0x000514E4
		protected override JobResult ExecuteImpl()
		{
			this.m_missionGenerationService.ReloadMissionSetFromDB(false);
			return JobResult.Finished;
		}

		// Token: 0x0400098E RID: 2446
		public const string MISSION_RELOADING_FROM_DB_JOB_NAME = "mission_reloading_from_db";

		// Token: 0x0400098F RID: 2447
		private readonly IMissionGenerationService m_missionGenerationService;
	}
}
