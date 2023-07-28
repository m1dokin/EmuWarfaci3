using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x02000793 RID: 1939
	[ConsoleCmdAttributes(CmdName = "mission_system_reset", ArgsSize = 1)]
	internal class MissionSystemResetCmd : IConsoleCmd
	{
		// Token: 0x0600283B RID: 10299 RVA: 0x000ACE9E File Offset: 0x000AB29E
		public MissionSystemResetCmd(IMissionSystem missionSystem)
		{
			this.m_missionSystem = missionSystem;
		}

		// Token: 0x0600283C RID: 10300 RVA: 0x000ACEAD File Offset: 0x000AB2AD
		public void ExecuteCmd(string[] args)
		{
			this.m_missionSystem.ResetUserMissions();
		}

		// Token: 0x04001518 RID: 5400
		private readonly IMissionSystem m_missionSystem;
	}
}
