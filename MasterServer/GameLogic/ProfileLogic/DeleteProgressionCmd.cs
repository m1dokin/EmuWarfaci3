using System;
using MasterServer.Core;
using MasterServer.Database;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x0200056F RID: 1391
	[ConsoleCmdAttributes(ArgsSize = 1, CmdName = "delete_profile_progression", Help = "profile_id")]
	internal class DeleteProgressionCmd : IConsoleCmd
	{
		// Token: 0x06001E05 RID: 7685 RVA: 0x00079BB3 File Offset: 0x00077FB3
		public DeleteProgressionCmd(IDALService dalService)
		{
			this.m_dalService = dalService;
		}

		// Token: 0x06001E06 RID: 7686 RVA: 0x00079BC2 File Offset: 0x00077FC2
		public void ExecuteCmd(string[] args)
		{
			this.m_dalService.ProfileProgressionSystem.DeleteProgression(ulong.Parse(args[1]));
		}

		// Token: 0x04000E83 RID: 3715
		private readonly IDALService m_dalService;
	}
}
