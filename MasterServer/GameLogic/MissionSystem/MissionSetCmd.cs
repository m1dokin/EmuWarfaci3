using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003B5 RID: 949
	[ConsoleCmdAttributes(CmdName = "mission_set", ArgsSize = 0)]
	internal class MissionSetCmd : IConsoleCmd
	{
		// Token: 0x06001521 RID: 5409 RVA: 0x000577D0 File Offset: 0x00055BD0
		public void ExecuteCmd(string[] args)
		{
			IDebugMissionGenerationService service = ServicesManager.GetService<IDebugMissionGenerationService>();
			service.DebugDumpMissionSet();
		}
	}
}
