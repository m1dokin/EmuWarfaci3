using System;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Jobs;
using MasterServer.Core.Services.Jobs.Attributes;
using MasterServer.Core.Services.Jobs.JobSchedulers;
using MasterServer.Core.Services.Jobs.Metrics;
using MasterServer.Users;

namespace MasterServer.GameLogic.JobSystem.Jobs
{
	// Token: 0x0200038C RID: 908
	[Job("corrupted_users_check")]
	internal class CorruptedUsersCheckingJob : Job
	{
		// Token: 0x06001463 RID: 5219 RVA: 0x00052D70 File Offset: 0x00051170
		public CorruptedUsersCheckingJob(ConfigSection jobConfigSection, IJobScheduler jobScheduler, ICorruptedUsersCheckingService corruptedUsersCheckingService, IJobMetricsTracker jobMetricsTracker) : base(jobConfigSection, jobScheduler, jobMetricsTracker)
		{
			this.m_corruptedUsersCheckingService = corruptedUsersCheckingService;
			int num;
			jobConfigSection.Get("untouched_for_check_min", out num);
			this.m_untouchedForCheck = TimeSpan.FromMinutes((double)num);
		}

		// Token: 0x06001464 RID: 5220 RVA: 0x00052DA8 File Offset: 0x000511A8
		protected override JobResult ExecuteImpl()
		{
			this.m_corruptedUsersCheckingService.PerformCheck(this.m_untouchedForCheck);
			return JobResult.Finished;
		}

		// Token: 0x06001465 RID: 5221 RVA: 0x00052DBC File Offset: 0x000511BC
		protected override void OnConfigChaged(ConfigEventArgs args)
		{
			base.OnConfigChaged(args);
			if (string.Equals(args.Name, "untouched_for_check_min"))
			{
				this.m_untouchedForCheck = TimeSpan.FromMinutes((double)args.iValue);
			}
		}

		// Token: 0x0400097B RID: 2427
		public const string CORRUPTED_USERS_CHECK_JOB_NAME = "corrupted_users_check";

		// Token: 0x0400097C RID: 2428
		private readonly ICorruptedUsersCheckingService m_corruptedUsersCheckingService;

		// Token: 0x0400097D RID: 2429
		private TimeSpan m_untouchedForCheck;
	}
}
