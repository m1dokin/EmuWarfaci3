using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003B8 RID: 952
	[ConsoleCmdAttributes(CmdName = "mission_set_sync", ArgsSize = 1)]
	internal class MissionSetSyncCmd : IConsoleCmd
	{
		// Token: 0x06001527 RID: 5415 RVA: 0x00057884 File Offset: 0x00055C84
		public void ExecuteCmd(string[] args)
		{
			bool force = false;
			if (args.Length == 2)
			{
				force = (args[1] == "force");
			}
			IMissionGenerationService service = ServicesManager.GetService<IMissionGenerationService>();
			service.ReloadMissionSetFromRealm(force);
		}
	}
}
