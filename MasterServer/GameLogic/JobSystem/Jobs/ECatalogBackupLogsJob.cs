using System;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Jobs;
using MasterServer.Core.Services.Jobs.Attributes;
using MasterServer.Core.Services.Jobs.JobSchedulers;
using MasterServer.Core.Services.Jobs.Metrics;
using MasterServer.ElectronicCatalog;

namespace MasterServer.GameLogic.JobSystem.Jobs
{
	// Token: 0x02000390 RID: 912
	[Job("ecat_backup_logs")]
	internal class ECatalogBackupLogsJob : BatchJob
	{
		// Token: 0x0600146C RID: 5228 RVA: 0x00052E7C File Offset: 0x0005127C
		public ECatalogBackupLogsJob(ConfigSection jobConfigSection, IJobScheduler jobScheduler, ICatalogService catalogService, IJobMetricsTracker jobMetricsTracker) : base(jobConfigSection, jobScheduler, jobMetricsTracker)
		{
			this.m_catalogService = catalogService;
			int num;
			jobConfigSection.Get("archive_lifetime_hours", out num);
			this.m_logRecordLifetime = TimeSpan.FromHours((double)num);
		}

		// Token: 0x0600146D RID: 5229 RVA: 0x00052EB4 File Offset: 0x000512B4
		protected override JobResult ExecuteBatch(int batchSize, TimeSpan dbTimeout)
		{
			if (this.m_catalogService.BackupLogs(this.m_logRecordLifetime, dbTimeout, batchSize))
			{
				return JobResult.Finished;
			}
			return JobResult.Continue;
		}

		// Token: 0x0600146E RID: 5230 RVA: 0x00052ED1 File Offset: 0x000512D1
		protected override void OnConfigChaged(ConfigEventArgs args)
		{
			base.OnConfigChaged(args);
			if (string.Equals(args.Name, "archive_lifetime_hours", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_logRecordLifetime = TimeSpan.FromHours((double)int.Parse(args.sValue));
			}
		}

		// Token: 0x04000980 RID: 2432
		public const string ECATALOG_BACKUP_LOGS_JOB_NAME = "ecat_backup_logs";

		// Token: 0x04000981 RID: 2433
		private readonly ICatalogService m_catalogService;

		// Token: 0x04000982 RID: 2434
		private TimeSpan m_logRecordLifetime;
	}
}
