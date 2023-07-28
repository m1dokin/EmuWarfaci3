using System;
using MasterServer.Core;

namespace MasterServer.Users
{
	// Token: 0x020007F8 RID: 2040
	[ConsoleCmdAttributes(CmdName = "debug_get_profile_info", ArgsSize = 1)]
	internal class DebugGetProfileInfoCmd : IConsoleCmd
	{
		// Token: 0x060029E7 RID: 10727 RVA: 0x000B4AB7 File Offset: 0x000B2EB7
		public DebugGetProfileInfoCmd(ISessionInfoService sessionInfoService)
		{
			this.m_sessionInfoService = sessionInfoService;
		}

		// Token: 0x060029E8 RID: 10728 RVA: 0x000B4AC8 File Offset: 0x000B2EC8
		public void ExecuteCmd(string[] args)
		{
			this.m_sessionInfoService.GetProfileInfo(args[1]).Dump();
		}

		// Token: 0x04001629 RID: 5673
		private readonly ISessionInfoService m_sessionInfoService;
	}
}
