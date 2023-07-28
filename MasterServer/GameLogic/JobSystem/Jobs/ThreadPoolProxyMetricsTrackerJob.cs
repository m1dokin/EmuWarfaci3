using System;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Jobs;
using MasterServer.Core.Services.Jobs.Attributes;
using MasterServer.Core.Services.Jobs.JobSchedulers;
using MasterServer.Core.Services.Jobs.Metrics;
using MasterServer.Telemetry.Metrics;

namespace MasterServer.GameLogic.JobSystem.Jobs
{
	// Token: 0x02000396 RID: 918
	[Job("thread_pool_proxy_metrics_tracker", FailureStrategy = JobFailureStrategy.Finish)]
	internal class ThreadPoolProxyMetricsTrackerJob : Job
	{
		// Token: 0x0600147B RID: 5243 RVA: 0x00053115 File Offset: 0x00051515
		public ThreadPoolProxyMetricsTrackerJob(ConfigSection jobConfigSection, IJobScheduler jobScheduler, IThreadPoolProxyMetricsTracker processMetricsTracker, IJobMetricsTracker jobMetricsTracker) : base(jobConfigSection, jobScheduler, jobMetricsTracker)
		{
			this.m_metricsTracker = processMetricsTracker;
		}

		// Token: 0x0600147C RID: 5244 RVA: 0x00053128 File Offset: 0x00051528
		protected override JobResult ExecuteImpl()
		{
			this.m_metricsTracker.Report();
			return JobResult.Finished;
		}

		// Token: 0x04000992 RID: 2450
		public const string THREAD_POOL_PROXY_METRICS_TRACKER_JOB_NAME = "thread_pool_proxy_metrics_tracker";

		// Token: 0x04000993 RID: 2451
		private readonly IThreadPoolProxyMetricsTracker m_metricsTracker;
	}
}
