using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x02000792 RID: 1938
	[ConsoleCmdAttributes(CmdName = "mission_system_dump", ArgsSize = 1)]
	internal class MissionSystemDumpCmd : IConsoleCmd
	{
		// Token: 0x06002839 RID: 10297 RVA: 0x000ACE66 File Offset: 0x000AB266
		public MissionSystemDumpCmd(IMissionSystem missionSystem)
		{
			this.m_missionSystem = missionSystem;
		}

		// Token: 0x0600283A RID: 10298 RVA: 0x000ACE75 File Offset: 0x000AB275
		public void ExecuteCmd(string[] args)
		{
			if (args.Length == 1)
			{
				this.m_missionSystem.Dump();
			}
			else
			{
				this.m_missionSystem.Dump(args[1]);
			}
		}

		// Token: 0x04001517 RID: 5399
		private readonly IMissionSystem m_missionSystem;
	}
}
