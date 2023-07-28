using System;
using HK2Net;
using MasterServer.Core.Services;
using MasterServer.Core.Services.Jobs;

namespace MasterServer.GameLogic.JobSystem.Jobs
{
	// Token: 0x02000063 RID: 99
	[OrphanService]
	[Singleton]
	internal class ThreadPoolProxyMetricsTrackerJobProvider : ServiceModule
	{
		// Token: 0x0600017A RID: 378 RVA: 0x0000A278 File Offset: 0x00008678
		public ThreadPoolProxyMetricsTrackerJobProvider(IJobSchedulerService jobSchedulerService)
		{
			this.m_jobSchedulerService = jobSchedulerService;
		}

		// Token: 0x0600017B RID: 379 RVA: 0x0000A287 File Offset: 0x00008687
		public override void Init()
		{
			base.Init();
			this.m_jobSchedulerService.AddJob("thread_pool_proxy_metrics_tracker");
		}

		// Token: 0x040000B2 RID: 178
		private readonly IJobSchedulerService m_jobSchedulerService;
	}
}
