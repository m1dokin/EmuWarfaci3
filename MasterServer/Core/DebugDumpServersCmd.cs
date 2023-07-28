using System;
using MasterServer.ServerInfo;

namespace MasterServer.Core
{
	// Token: 0x020007A5 RID: 1957
	[ConsoleCmdAttributes(CmdName = "debug_dump_servers", ArgsSize = 0)]
	internal class DebugDumpServersCmd : IConsoleCmd
	{
		// Token: 0x06002875 RID: 10357 RVA: 0x000AE15C File Offset: 0x000AC55C
		public void ExecuteCmd(string[] args)
		{
			IServerInfo service = ServicesManager.GetService<IServerInfo>();
			service.DumpServers();
		}
	}
}
