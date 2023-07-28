using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Jobs;
using MasterServer.Core.Services.Jobs.Attributes;
using MasterServer.Core.Services.Jobs.JobSchedulers;
using MasterServer.Core.Services.Jobs.Metrics;
using MasterServer.GameLogic.VoucherSystem;
using MasterServer.GameLogic.VoucherSystem.VoucherSynchronization;
using Util.Common;

namespace MasterServer.GameLogic.JobSystem.Jobs
{
	// Token: 0x0200038A RID: 906
	[Job("corrupted_voucher_synchronization")]
	internal class CorruptedVoucherSynchronizationJob : BatchJob
	{
		// Token: 0x0600145E RID: 5214 RVA: 0x00052CC5 File Offset: 0x000510C5
		public CorruptedVoucherSynchronizationJob(ConfigSection jobConfigSection, IJobScheduler jobScheduler, IJobMetricsTracker jobMetricsTracker, IVoucherSynchronizer voucherSynchronizer) : base(jobConfigSection, jobScheduler, jobMetricsTracker)
		{
			this.m_startIndex = 0UL;
			this.m_voucherSynchronizer = voucherSynchronizer;
		}

		// Token: 0x0600145F RID: 5215 RVA: 0x00052CE0 File Offset: 0x000510E0
		protected override JobResult ExecuteBatch(int batchSize, TimeSpan dbTimeout)
		{
			IEnumerable<Voucher> enumerable = this.m_voucherSynchronizer.SynchronizeCorrupted(this.m_startIndex, batchSize);
			enumerable.ForEach(delegate(Voucher v)
			{
				this.m_startIndex = Math.Max(v.Id, this.m_startIndex);
			});
			this.m_startIndex += 1UL;
			return (enumerable.Count<Voucher>() != batchSize) ? JobResult.Finished : JobResult.Continue;
		}

		// Token: 0x04000976 RID: 2422
		public const string VOUCHER_RECOVERY_JOB_NAME = "corrupted_voucher_synchronization";

		// Token: 0x04000977 RID: 2423
		private readonly IVoucherSynchronizer m_voucherSynchronizer;

		// Token: 0x04000978 RID: 2424
		private ulong m_startIndex;
	}
}
