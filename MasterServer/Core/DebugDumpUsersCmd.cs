using System;
using MasterServer.Users;

namespace MasterServer.Core
{
	// Token: 0x020007A7 RID: 1959
	[ConsoleCmdAttributes(CmdName = "debug_dump_users", ArgsSize = 0)]
	internal class DebugDumpUsersCmd : IConsoleCmd
	{
		// Token: 0x06002878 RID: 10360 RVA: 0x000AE19C File Offset: 0x000AC59C
		public DebugDumpUsersCmd(IUserRepository userRepository)
		{
			this.m_userRepository = userRepository;
		}

		// Token: 0x06002879 RID: 10361 RVA: 0x000AE1AB File Offset: 0x000AC5AB
		public void ExecuteCmd(string[] args)
		{
			this.m_userRepository.DumpUsers();
		}

		// Token: 0x04001531 RID: 5425
		private readonly IUserRepository m_userRepository;
	}
}
