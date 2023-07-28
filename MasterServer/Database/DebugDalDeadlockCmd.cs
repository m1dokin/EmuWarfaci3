using System;
using MasterServer.Core;

namespace MasterServer.Database
{
	// Token: 0x020001D7 RID: 471
	[ConsoleCmdAttributes(CmdName = "debug_dal_deadlock", Help = "Emulate deadlock per DAL query")]
	internal class DebugDalDeadlockCmd : ConsoleCommand<DebugDalDeadlockParams>
	{
		// Token: 0x060008FD RID: 2301 RVA: 0x00022387 File Offset: 0x00020787
		public DebugDalDeadlockCmd(IDALService dalService)
		{
			this.m_dalService = dalService;
		}

		// Token: 0x060008FE RID: 2302 RVA: 0x00022396 File Offset: 0x00020796
		protected override void Execute(DebugDalDeadlockParams param)
		{
			this.m_dalService.Config.DebugDeadlockSet(param.QueryName, param.Count, param.RetriesCount);
			this.m_dalService.Config.DebugDeadlockDump();
		}

		// Token: 0x04000539 RID: 1337
		private readonly IDALService m_dalService;
	}
}
