using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003BA RID: 954
	[ConsoleCmdAttributes(CmdName = "mission_rotation_emulate", ArgsSize = 2)]
	internal class MissionRotationEmulateCmd : IConsoleCmd
	{
		// Token: 0x0600152B RID: 5419 RVA: 0x000578E4 File Offset: 0x00055CE4
		public void ExecuteCmd(string[] args)
		{
			IDebugMissionGenerationService service = ServicesManager.GetService<IDebugMissionGenerationService>();
			if (args.Length <= 2)
			{
				Log.Info("Set elements number as first argument, and shuffles number as second");
				return;
			}
			int elementsNum = int.Parse(args[1]);
			int shufflesNum = int.Parse(args[2]);
			service.DebugEmulateRotation(elementsNum, shufflesNum);
		}
	}
}
