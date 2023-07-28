using System;
using System.Threading;
using MasterServer.Core;
using MasterServer.Core.Services.Jobs;
using MasterServer.Core.Services.Jobs.JobSchedulers;
using MasterServer.Core.Services.Jobs.Metrics;

namespace MasterServer.GameLogic.JobSystem.Jobs.Debug
{
	// Token: 0x0200038E RID: 910
	internal class DebugBatchJob : BatchJob
	{
		// Token: 0x06001468 RID: 5224 RVA: 0x00052E0D File Offset: 0x0005120D
		public DebugBatchJob(string name, byte priority, int steps, string crontab, IJobMetricsTracker jobMetricsTracker) : base(name, priority, steps, 0, TimeSpan.Zero, new CronJobScheduler(crontab, null), jobMetricsTracker)
		{
		}

		// Token: 0x06001469 RID: 5225 RVA: 0x00052E28 File Offset: 0x00051228
		protected override JobResult ExecuteBatch(int batchSize, TimeSpan dbTimeout)
		{
			Log.Info<DebugBatchJob>("{0} is working now!", this);
			Thread.Sleep(TimeSpan.FromSeconds(15.0));
			return JobResult.Continue;
		}
	}
}
