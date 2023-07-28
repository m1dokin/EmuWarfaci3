using System;
using MasterServer.Core;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x0200019B RID: 411
	[ConsoleCmdAttributes(CmdName = "online_query_request_delay", Help = "Adds request delay for specified query", ArgsSize = 100)]
	internal class OnlineQueryRequestDelayCmd : IConsoleCmd
	{
		// Token: 0x060007B6 RID: 1974 RVA: 0x0001DA44 File Offset: 0x0001BE44
		public void ExecuteCmd(string[] args)
		{
			QueryDelayCmd.ExecuteCmd(QueryDelayCmd.DelayType.Request, args);
		}
	}
}
