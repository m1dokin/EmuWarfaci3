using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003B9 RID: 953
	[ConsoleCmdAttributes(CmdName = "mission_set_validate", ArgsSize = 0)]
	internal class MissionSetValidateCmd : IConsoleCmd
	{
		// Token: 0x06001529 RID: 5417 RVA: 0x000578C0 File Offset: 0x00055CC0
		public void ExecuteCmd(string[] args)
		{
			IDebugMissionGenerationService service = ServicesManager.GetService<IDebugMissionGenerationService>();
			service.DebugValidateMissionGraphs();
		}
	}
}
