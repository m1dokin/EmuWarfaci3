using System;
using MasterServer.Core;

namespace MasterServer.Database
{
	// Token: 0x0200003F RID: 63
	[ConsoleCmdAttributes(CmdName = "debug_dal_deadlock_dump", Help = "Dump emulated deadlocks")]
	internal class DebugDALDeadlockDumpCmd : IConsoleCmd
	{
		// Token: 0x06000105 RID: 261 RVA: 0x00008745 File Offset: 0x00006B45
		public DebugDALDeadlockDumpCmd(IDALService dalService)
		{
			this.m_dalService = dalService;
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00008754 File Offset: 0x00006B54
		public void ExecuteCmd(string[] args)
		{
			this.m_dalService.Config.DebugDeadlockDump();
		}

		// Token: 0x04000088 RID: 136
		private readonly IDALService m_dalService;
	}
}
