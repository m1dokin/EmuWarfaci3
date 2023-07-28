using System;
using MasterServer.Core;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x0200019D RID: 413
	[ConsoleCmdAttributes(CmdName = "online_query_block", ArgsSize = 100)]
	internal class OnlineQueryBlockCmd : IConsoleCmd
	{
		// Token: 0x060007BA RID: 1978 RVA: 0x0001DA68 File Offset: 0x0001BE68
		public void ExecuteCmd(string[] args)
		{
			IQueryManager service = ServicesManager.GetService<IQueryManager>();
			if (args.Length < 2 || args.Length > 4)
			{
				Log.Info("Usage: online_query_block [tag1[,tag2,...,tagN] block [error]] - blocks or unblocks query(ies)\n\t- tag1,tag2,...,tagN - tags of the queries to block/unblock (optional)\n\t- block - parameter that should be set to 1 to block query or to 0 to unblock (optional)\n\t- error - error code (integer) that will be returned in response to the blocked query (default is 8 - eOnlineError_ParseError)\n");
				service.DumpQueryBlockingFlags();
			}
			else
			{
				string[] tags = args[1].Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries);
				if (args.Length < 3)
				{
					service.DumpQueryBlockingFlags(tags);
				}
				else
				{
					EOnlineError errorCode = EOnlineError.eOnlineError_ParseError;
					if (args.Length > 3 && Enum.IsDefined(typeof(EOnlineError), int.Parse(args[3])))
					{
						errorCode = (EOnlineError)int.Parse(args[3]);
					}
					service.UpdateQueryBlockingFlags(tags, int.Parse(args[2]) == 1, errorCode);
				}
			}
		}
	}
}
