using System;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.GameLogic.PunishmentSystem;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x0200054C RID: 1356
	[ConsoleCmdAttributes(CmdName = "move_profile_to_hot", ArgsSize = 1)]
	internal class MoveProfileToHotCmd : IConsoleCmd
	{
		// Token: 0x06001D46 RID: 7494 RVA: 0x000766BC File Offset: 0x00074ABC
		public void ExecuteCmd(string[] args)
		{
			ulong num = ulong.Parse(args[1]);
			IColdStorageService service = ServicesManager.GetService<IColdStorageService>();
			IPunishmentService service2 = ServicesManager.GetService<IPunishmentService>();
			service2.ForceLogout(num);
			TouchProfileResult touchProfileResult = service.TouchProfile(num, Resources.LatestDbUpdateVersion);
			Log.Info<ETouchProfileResult>("move_profile_to_hot: {0}", touchProfileResult.Status);
		}
	}
}
