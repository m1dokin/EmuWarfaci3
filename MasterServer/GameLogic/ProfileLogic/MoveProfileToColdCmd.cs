using System;
using MasterServer.Core;
using MasterServer.GameLogic.PunishmentSystem;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x0200054B RID: 1355
	[ConsoleCmdAttributes(CmdName = "move_profile_to_cold", ArgsSize = 1)]
	internal class MoveProfileToColdCmd : IConsoleCmd
	{
		// Token: 0x06001D44 RID: 7492 RVA: 0x0007665C File Offset: 0x00074A5C
		public void ExecuteCmd(string[] args)
		{
			ulong num = ulong.Parse(args[1]);
			IColdStorageService service = ServicesManager.GetService<IColdStorageService>();
			IPunishmentService service2 = ServicesManager.GetService<IPunishmentService>();
			service2.ForceLogout(num);
			bool flag = service.ArchiveProfile(num, TimeSpan.Zero);
			Log.Info<string>("move_profile_to_cold: {0}", (!flag) ? "failed" : "ok");
		}
	}
}
