using System;
using MasterServer.ServerInfo;

namespace MasterServer.Core
{
	// Token: 0x020007A6 RID: 1958
	[ConsoleCmdAttributes(CmdName = "debug_dump_server", ArgsSize = 1)]
	internal class DebugDumpServerCmd : IConsoleCmd
	{
		// Token: 0x06002877 RID: 10359 RVA: 0x000AE180 File Offset: 0x000AC580
		public void ExecuteCmd(string[] args)
		{
			IServerInfo service = ServicesManager.GetService<IServerInfo>();
			service.DumpServer(args[1]);
		}
	}
}
