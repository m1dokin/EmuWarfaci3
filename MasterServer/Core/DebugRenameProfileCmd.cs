using System;
using MasterServer.Database;

namespace MasterServer.Core
{
	// Token: 0x020007AB RID: 1963
	[ConsoleCmdAttributes(CmdName = "debug_rename_profile", ArgsSize = 2, Help = "profile_id, nickname")]
	internal class DebugRenameProfileCmd : IConsoleCmd
	{
		// Token: 0x06002880 RID: 10368 RVA: 0x000AE3FC File Offset: 0x000AC7FC
		public void ExecuteCmd(string[] args)
		{
			IDALService service = ServicesManager.GetService<IDALService>();
			ulong num = ulong.Parse(args[1]);
			string text = args[2];
			bool flag = service.ProfileSystem.UpdateProfileNickname(num, text);
			service.ClanSystem.FlushClanCacheForMember(num);
			service.ProfileSystem.FlushProfileFriendsCache(num);
			Log.Info<ulong, string, string>("Renaming profile {0} to {1} is {2}", num, text, (!flag) ? "failed" : "succeeded");
		}
	}
}
