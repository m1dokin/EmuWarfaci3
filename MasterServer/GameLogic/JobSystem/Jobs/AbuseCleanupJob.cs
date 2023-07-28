using System;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Jobs;
using MasterServer.Core.Services.Jobs.Attributes;
using MasterServer.Core.Services.Jobs.JobSchedulers;
using MasterServer.Core.Services.Jobs.Metrics;
using MasterServer.GameLogic.ProfileLogic;

namespace MasterServer.GameLogic.JobSystem.Jobs
{
	// Token: 0x02000385 RID: 901
	[Job("abuse_cleanup")]
	internal class AbuseCleanupJob : BatchJob
	{
		// Token: 0x06001452 RID: 5202 RVA: 0x00052B38 File Offset: 0x00050F38
		public AbuseCleanupJob(ConfigSection jobConfigSection, IJobScheduler jobScheduler, IAbuseReportService abuseReportService, IJobMetricsTracker jobMetricsTracker) : base(jobConfigSection, jobScheduler, jobMetricsTracker)
		{
			this.m_abuseReportService = abuseReportService;
			int num;
			jobConfigSection.Get("report_lifetime_min", out num);
			this.m_abuseReportLifetime = TimeSpan.FromMinutes((double)num);
		}

		// Token: 0x06001453 RID: 5203 RVA: 0x00052B70 File Offset: 0x00050F70
		protected override JobResult ExecuteBatch(int batchSize, TimeSpan dbTimeout)
		{
			if (this.m_abuseReportService.CleanupHistory(this.m_abuseReportLifetime, dbTimeout, batchSize))
			{
				return JobResult.Finished;
			}
			return JobResult.Continue;
		}

		// Token: 0x06001454 RID: 5204 RVA: 0x00052B8D File Offset: 0x00050F8D
		protected override void OnConfigChaged(ConfigEventArgs args)
		{
			base.OnConfigChaged(args);
			if (string.Equals(args.Name, "report_lifetime_min", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_abuseReportLifetime = TimeSpan.FromMinutes((double)int.Parse(args.sValue));
			}
		}

		// Token: 0x0400096A RID: 2410
		public const string ABUSE_CLEANUP_JOB_NAME = "abuse_cleanup";

		// Token: 0x0400096B RID: 2411
		private readonly IAbuseReportService m_abuseReportService;

		// Token: 0x0400096C RID: 2412
		private TimeSpan m_abuseReportLifetime;
	}
}
