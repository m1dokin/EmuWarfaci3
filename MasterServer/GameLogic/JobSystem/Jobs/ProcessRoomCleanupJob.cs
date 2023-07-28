using System;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Jobs;
using MasterServer.Core.Services.Jobs.Attributes;
using MasterServer.Core.Services.Jobs.JobSchedulers;
using MasterServer.Core.Services.Jobs.Metrics;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.JobSystem.Jobs
{
	// Token: 0x02000397 RID: 919
	[Job("process_room_cleanup", FailureStrategy = JobFailureStrategy.Finish)]
	internal class ProcessRoomCleanupJob : Job
	{
		// Token: 0x0600147D RID: 5245 RVA: 0x00053136 File Offset: 0x00051536
		public ProcessRoomCleanupJob(ConfigSection jobConfigSection, IJobScheduler jobScheduler, IJobMetricsTracker jobMetricsTracker, IGameRoomManager gameRoomManager) : base(jobConfigSection, jobScheduler, jobMetricsTracker)
		{
			this.m_gameRoomManager = gameRoomManager;
		}

		// Token: 0x0600147E RID: 5246 RVA: 0x00053149 File Offset: 0x00051549
		protected override JobResult ExecuteImpl()
		{
			this.m_gameRoomManager.CleanEmptyRooms();
			return JobResult.Finished;
		}

		// Token: 0x04000994 RID: 2452
		public const string PROCESS_ROOM_CLEANUP_JOB_NAME = "process_room_cleanup";

		// Token: 0x04000995 RID: 2453
		private readonly IGameRoomManager m_gameRoomManager;
	}
}
