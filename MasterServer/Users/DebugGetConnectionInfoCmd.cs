using System;
using MasterServer.Core;

namespace MasterServer.Users
{
	// Token: 0x020007F7 RID: 2039
	[ConsoleCmdAttributes(CmdName = "debug_get_conn_info", ArgsSize = 1)]
	internal class DebugGetConnectionInfoCmd : IConsoleCmd
	{
		// Token: 0x060029E6 RID: 10726 RVA: 0x000B4A84 File Offset: 0x000B2E84
		public void ExecuteCmd(string[] args)
		{
			ISessionInfoService service = ServicesManager.GetService<ISessionInfoService>();
			SessionInfo sessionInfo = service.GetSessionInfo(args[1]);
			Log.Info<string, UserTags>("IP: {0}, Tags: {1}", sessionInfo.IPAddress, sessionInfo.Tags);
		}
	}
}
