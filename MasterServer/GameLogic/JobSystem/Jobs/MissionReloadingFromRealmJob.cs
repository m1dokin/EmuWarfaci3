using System;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Jobs;
using MasterServer.Core.Services.Jobs.Attributes;
using MasterServer.Core.Services.Jobs.JobSchedulers;
using MasterServer.Core.Services.Jobs.Metrics;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.JobSystem.Jobs
{
	// Token: 0x02000395 RID: 917
	[Job("mission_reloading_from_realm", FailureStrategy = JobFailureStrategy.Finish)]
	internal class MissionReloadingFromRealmJob : Job
	{
		// Token: 0x06001479 RID: 5241 RVA: 0x000530F3 File Offset: 0x000514F3
		public MissionReloadingFromRealmJob(ConfigSection jobConfigSection, IJobScheduler jobScheduler, IMissionGenerationService missionGenerationService, IJobMetricsTracker jobMetricsTracker) : base(jobConfigSection, jobScheduler, jobMetricsTracker)
		{
			this.m_missionGenerationService = missionGenerationService;
		}

		// Token: 0x0600147A RID: 5242 RVA: 0x00053106 File Offset: 0x00051506
		protected override JobResult ExecuteImpl()
		{
			this.m_missionGenerationService.ReloadMissionSetFromRealm(false);
			return JobResult.Finished;
		}

		// Token: 0x04000990 RID: 2448
		public const string MISSION_RELOADING_FROM_REALM_JOB_NAME = "mission_reloading_from_realm";

		// Token: 0x04000991 RID: 2449
		private readonly IMissionGenerationService m_missionGenerationService;
	}
}
