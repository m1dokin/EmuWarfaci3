using System;
using MasterServer.Core;
using MasterServer.Core.Services.Jobs;

namespace MasterServer.GameLogic.JobSystem.Commands
{
	// Token: 0x02000060 RID: 96
	[ConsoleCmdAttributes(CmdName = "debug_dump_jobs", ArgsSize = 0)]
	internal class DebugDumpJobsCmd : IConsoleCmd
	{
		// Token: 0x06000171 RID: 369 RVA: 0x0000A0F1 File Offset: 0x000084F1
		public DebugDumpJobsCmd(IDebugJobSchedulerService jobSchedulerService)
		{
			this.m_jobSchedulerService = jobSchedulerService;
		}

		// Token: 0x06000172 RID: 370 RVA: 0x0000A100 File Offset: 0x00008500
		public void ExecuteCmd(string[] args)
		{
			this.m_jobSchedulerService.DumpJobs();
		}

		// Token: 0x040000AD RID: 173
		private readonly IDebugJobSchedulerService m_jobSchedulerService;
	}
}
