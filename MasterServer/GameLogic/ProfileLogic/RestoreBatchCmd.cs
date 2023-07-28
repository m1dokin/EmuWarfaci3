using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x0200054E RID: 1358
	[ConsoleCmdAttributes(CmdName = "move_profile_batch_to_hot", ArgsSize = 1, Help = "Args: BatchSize")]
	internal class RestoreBatchCmd : IConsoleCmd
	{
		// Token: 0x06001D4A RID: 7498 RVA: 0x00076744 File Offset: 0x00074B44
		public void ExecuteCmd(string[] args)
		{
			int batch_size = int.Parse(args[1]);
			IColdStorageService service = ServicesManager.GetService<IColdStorageService>();
			service.DebugRestoreAllProfiles(batch_size);
		}
	}
}
