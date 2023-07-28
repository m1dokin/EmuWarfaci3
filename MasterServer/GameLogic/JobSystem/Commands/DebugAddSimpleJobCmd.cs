using System;
using MasterServer.Core;
using MasterServer.Core.Services.Jobs;
using MasterServer.Core.Services.Jobs.Metrics;
using MasterServer.GameLogic.JobSystem.Jobs.Debug;

namespace MasterServer.GameLogic.JobSystem.Commands
{
	// Token: 0x0200005F RID: 95
	[ConsoleCmdAttributes(CmdName = "debug_add_simple_job", ArgsSize = 2)]
	internal class DebugAddSimpleJobCmd : IConsoleCmd
	{
		// Token: 0x0600016F RID: 367 RVA: 0x0000A0B8 File Offset: 0x000084B8
		public DebugAddSimpleJobCmd(IJobSchedulerService jobSchedulerService, IJobMetricsTracker metricsTracker)
		{
			this.m_jobSchedulerService = jobSchedulerService;
			this.m_metricsTracker = metricsTracker;
		}

		// Token: 0x06000170 RID: 368 RVA: 0x0000A0CE File Offset: 0x000084CE
		public void ExecuteCmd(string[] args)
		{
			this.m_jobSchedulerService.AddJob(new DebugSimpleJob(args[1], byte.Parse(args[2]), this.m_metricsTracker));
		}

		// Token: 0x040000AB RID: 171
		private readonly IJobSchedulerService m_jobSchedulerService;

		// Token: 0x040000AC RID: 172
		private readonly IJobMetricsTracker m_metricsTracker;
	}
}
