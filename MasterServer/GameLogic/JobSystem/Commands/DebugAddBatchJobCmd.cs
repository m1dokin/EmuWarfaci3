using System;
using MasterServer.Core;
using MasterServer.Core.Services.Jobs;
using MasterServer.Core.Services.Jobs.Metrics;
using MasterServer.GameLogic.JobSystem.Jobs.Debug;

namespace MasterServer.GameLogic.JobSystem.Commands
{
	// Token: 0x0200005E RID: 94
	[ConsoleCmdAttributes(CmdName = "debug_add_batch_job", ArgsSize = 4, Help = "params: name, priority, steps, crontab")]
	internal class DebugAddBatchJobCmd : IConsoleCmd
	{
		// Token: 0x0600016D RID: 365 RVA: 0x0000A074 File Offset: 0x00008474
		public DebugAddBatchJobCmd(IJobSchedulerService jobSchedulerService, IJobMetricsTracker metricsTracker)
		{
			this.m_jobSchedulerService = jobSchedulerService;
			this.m_metricsTracker = metricsTracker;
		}

		// Token: 0x0600016E RID: 366 RVA: 0x0000A08A File Offset: 0x0000848A
		public void ExecuteCmd(string[] args)
		{
			this.m_jobSchedulerService.AddJob(new DebugBatchJob(args[1], byte.Parse(args[2]), int.Parse(args[3]), args[4], this.m_metricsTracker));
		}

		// Token: 0x040000A9 RID: 169
		private readonly IJobSchedulerService m_jobSchedulerService;

		// Token: 0x040000AA RID: 170
		private readonly IJobMetricsTracker m_metricsTracker;
	}
}
