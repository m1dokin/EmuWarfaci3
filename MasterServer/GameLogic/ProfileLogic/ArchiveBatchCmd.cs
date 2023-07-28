using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x0200054D RID: 1357
	[ConsoleCmdAttributes(CmdName = "move_profile_batch_to_cold", ArgsSize = 2, Help = "Args: TimeSpan BatchSize")]
	internal class ArchiveBatchCmd : IConsoleCmd
	{
		// Token: 0x06001D48 RID: 7496 RVA: 0x0007670C File Offset: 0x00074B0C
		public void ExecuteCmd(string[] args)
		{
			TimeSpan threshold = TimeSpan.Parse(args[1]);
			int batch_size = int.Parse(args[2]);
			IColdStorageService service = ServicesManager.GetService<IColdStorageService>();
			service.ArchiveProfiles(threshold, batch_size);
		}
	}
}
