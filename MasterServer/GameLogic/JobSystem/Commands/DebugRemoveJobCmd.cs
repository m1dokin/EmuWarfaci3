using System;
using MasterServer.Core;
using MasterServer.Core.Services.Jobs;

namespace MasterServer.GameLogic.JobSystem.Commands
{
	// Token: 0x02000061 RID: 97
	[ConsoleCmdAttributes(CmdName = "debug_remove_job", ArgsSize = 1)]
	internal class DebugRemoveJobCmd : IConsoleCmd
	{
		// Token: 0x06000173 RID: 371 RVA: 0x0000A10D File Offset: 0x0000850D
		public DebugRemoveJobCmd(IJobSchedulerService jobSchedulerService)
		{
			this.m_jobSchedulerService = jobSchedulerService;
		}

		// Token: 0x06000174 RID: 372 RVA: 0x0000A11C File Offset: 0x0000851C
		public void ExecuteCmd(string[] args)
		{
			this.m_jobSchedulerService.RemoveJob(ulong.Parse(args[1]));
		}

		// Token: 0x040000AE RID: 174
		private readonly IJobSchedulerService m_jobSchedulerService;
	}
}
