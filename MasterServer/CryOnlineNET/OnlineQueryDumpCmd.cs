using System;
using System.Collections.Generic;
using MasterServer.Core;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x0200019E RID: 414
	[ConsoleCmdAttributes(CmdName = "online_query_dump", ArgsSize = 1)]
	internal class OnlineQueryDumpCmd : IConsoleCmd
	{
		// Token: 0x060007BC RID: 1980 RVA: 0x0001DB1C File Offset: 0x0001BF1C
		public void ExecuteCmd(string[] args)
		{
			IQueryManager service = ServicesManager.GetService<IQueryManager>();
			IEnumerable<string> enumerable = (args.Length != 2) ? service.GetQueries() : service.GetQueries(args[1]);
			foreach (string p in enumerable)
			{
				Log.Info<string>("Query: {0}", p);
			}
		}
	}
}
