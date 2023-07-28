using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003B6 RID: 950
	[ConsoleCmdAttributes(CmdName = "mission_set_reload", ArgsSize = 1)]
	internal class MissionSetReloadCmd : IConsoleCmd
	{
		// Token: 0x06001523 RID: 5411 RVA: 0x000577F4 File Offset: 0x00055BF4
		public void ExecuteCmd(string[] args)
		{
			bool force = false;
			if (args.Length == 2)
			{
				force = (args[1] == "force");
			}
			IMissionGenerationService service = ServicesManager.GetService<IMissionGenerationService>();
			service.ReloadMissionSetFromDB(force);
		}
	}
}
