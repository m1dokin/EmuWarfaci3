using System;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Jobs;
using MasterServer.Core.Services.Jobs.Attributes;
using MasterServer.Core.Services.Jobs.JobSchedulers;
using MasterServer.Core.Services.Jobs.Metrics;
using MasterServer.GameLogic.VoucherSystem.VoucherSynchronization;

namespace MasterServer.GameLogic.JobSystem.Jobs
{
	// Token: 0x0200038B RID: 907
	[Job("voucher_synchronization")]
	internal class VoucherSynchronizationJob : Job
	{
		// Token: 0x06001461 RID: 5217 RVA: 0x00052D4D File Offset: 0x0005114D
		public VoucherSynchronizationJob(ConfigSection jobConfigSection, IJobScheduler jobScheduler, IJobMetricsTracker jobMetricsTracker, IVoucherSynchronizer synchronizer) : base(jobConfigSection, jobScheduler, jobMetricsTracker)
		{
			this.m_synchronizer = synchronizer;
		}

		// Token: 0x06001462 RID: 5218 RVA: 0x00052D60 File Offset: 0x00051160
		protected override JobResult ExecuteImpl()
		{
			this.m_synchronizer.Synchronize();
			return JobResult.Finished;
		}

		// Token: 0x04000979 RID: 2425
		public const string VOUCHER_SYNCHRONIZATION_JOB_NAME = "voucher_synchronization";

		// Token: 0x0400097A RID: 2426
		private readonly IVoucherSynchronizer m_synchronizer;
	}
}
