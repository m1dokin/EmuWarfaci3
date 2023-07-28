using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x0200054A RID: 1354
	[ConsoleCmdAttributes(CmdName = "cold_profile_status", ArgsSize = 1)]
	internal class ColdProfileStatusCmd : IConsoleCmd
	{
		// Token: 0x06001D42 RID: 7490 RVA: 0x000765E8 File Offset: 0x000749E8
		public void ExecuteCmd(string[] args)
		{
			ulong num = ulong.Parse(args[1]);
			IColdStorageService service = ServicesManager.GetService<IColdStorageService>();
			bool? flag = service.IsProfileCold(num);
			Log.Info<ulong, string>("profile {0} {1}", num, (flag == null) ? "doesn't exist" : ("is: " + ((!flag.Value) ? "hot" : "cold")));
		}
	}
}
