using System;
using MasterServer.Core;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x0200019C RID: 412
	[ConsoleCmdAttributes(CmdName = "online_query_response_delay", Help = "Adds response delay for specified query", ArgsSize = 100)]
	internal class OnlineQueryResponseDelayCmd : IConsoleCmd
	{
		// Token: 0x060007B8 RID: 1976 RVA: 0x0001DA55 File Offset: 0x0001BE55
		public void ExecuteCmd(string[] args)
		{
			QueryDelayCmd.ExecuteCmd(QueryDelayCmd.DelayType.Response, args);
		}
	}
}
