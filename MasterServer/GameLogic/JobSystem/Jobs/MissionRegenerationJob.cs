using System;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Jobs;
using MasterServer.Core.Services.Jobs.Attributes;
using MasterServer.Core.Services.Jobs.JobSchedulers;
using MasterServer.Core.Services.Jobs.Metrics;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.JobSystem.Jobs
{
	// Token: 0x02000393 RID: 915
	[Job("mission_regeneration")]
	internal class MissionRegenerationJob : Job
	{
		// Token: 0x06001475 RID: 5237 RVA: 0x000530B0 File Offset: 0x000514B0
		public MissionRegenerationJob(ConfigSection jobConfigSection, IJobScheduler jobScheduler, IMissionGenerationService missionGenerationService, IJobMetricsTracker jobMetricsTracker) : base(jobConfigSection, jobScheduler, jobMetricsTracker)
		{
			this.m_missionGenerationService = missionGenerationService;
		}

		// Token: 0x06001476 RID: 5238 RVA: 0x000530C3 File Offset: 0x000514C3
		protected override JobResult ExecuteImpl()
		{
			this.m_missionGenerationService.RegenerateMissionSet();
			return JobResult.Finished;
		}

		// Token: 0x0400098C RID: 2444
		public const string MISSION_REGENERATION_JOB_NAME = "mission_regeneration";

		// Token: 0x0400098D RID: 2445
		private readonly IMissionGenerationService m_missionGenerationService;
	}
}
