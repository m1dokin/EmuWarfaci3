using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003B7 RID: 951
	[ConsoleCmdAttributes(CmdName = "mission_set_regenerate", ArgsSize = 1)]
	internal class MissionSetRegenerateCmd : IConsoleCmd
	{
		// Token: 0x06001525 RID: 5413 RVA: 0x00057830 File Offset: 0x00055C30
		public void ExecuteCmd(string[] args)
		{
			int num = 1;
			if (args.Length == 2)
			{
				num = Math.Min(100, Math.Max(1, int.Parse(args[1])));
			}
			IMissionGenerationService service = ServicesManager.GetService<IMissionGenerationService>();
			for (int num2 = 0; num2 != num; num2++)
			{
				service.RegenerateMissionSet();
			}
		}
	}
}
