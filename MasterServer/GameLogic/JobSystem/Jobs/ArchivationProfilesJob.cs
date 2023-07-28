using System;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Jobs;
using MasterServer.Core.Services.Jobs.Attributes;
using MasterServer.Core.Services.Jobs.JobSchedulers;
using MasterServer.Core.Services.Jobs.Metrics;
using MasterServer.GameLogic.ProfileLogic;

namespace MasterServer.GameLogic.JobSystem.Jobs
{
	// Token: 0x02000386 RID: 902
	[Job("archivation_profiles")]
	internal class ArchivationProfilesJob : BatchJob
	{
		// Token: 0x06001455 RID: 5205 RVA: 0x00052BC4 File Offset: 0x00050FC4
		public ArchivationProfilesJob(ConfigSection jobConfigSection, IJobScheduler jobScheduler, IColdStorageService coldStorageService, IJobMetricsTracker jobMetricsTracker) : base(jobConfigSection, jobScheduler, jobMetricsTracker)
		{
			this.m_coldStorageService = coldStorageService;
			int num;
			jobConfigSection.Get("expiration_threshold_day", out num);
			this.m_expirationThreshold = TimeSpan.FromDays((double)num);
		}

		// Token: 0x06001456 RID: 5206 RVA: 0x00052BFC File Offset: 0x00050FFC
		protected override JobResult ExecuteBatch(int batchSize, TimeSpan dbTimeout)
		{
			this.m_coldStorageService.ArchiveProfiles(this.m_expirationThreshold, batchSize);
			return JobResult.Finished;
		}

		// Token: 0x06001457 RID: 5207 RVA: 0x00052C11 File Offset: 0x00051011
		protected override void OnConfigChaged(ConfigEventArgs args)
		{
			base.OnConfigChaged(args);
			if (string.Equals(args.Name, "expiration_threshold_day", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_expirationThreshold = TimeSpan.FromDays((double)int.Parse(args.sValue));
			}
		}

		// Token: 0x0400096D RID: 2413
		public const string ARCHIVATION_PROFILES_JOB_NAME = "archivation_profiles";

		// Token: 0x0400096E RID: 2414
		private readonly IColdStorageService m_coldStorageService;

		// Token: 0x0400096F RID: 2415
		private TimeSpan m_expirationThreshold;
	}
}
