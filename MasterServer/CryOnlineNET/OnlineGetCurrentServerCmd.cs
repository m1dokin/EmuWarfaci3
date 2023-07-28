using System;
using MasterServer.Core;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x0200017E RID: 382
	[ConsoleCmdAttributes(CmdName = "online_get_current_server", ArgsSize = 0)]
	internal class OnlineGetCurrentServerCmd : IConsoleCmd
	{
		// Token: 0x06000707 RID: 1799 RVA: 0x0001B3EE File Offset: 0x000197EE
		public void ExecuteCmd(string[] args)
		{
			Log.Info<string>("Current ejabberd node address = {0}", ServicesManager.GetService<IOnlineClient>().Server);
		}
	}
}
