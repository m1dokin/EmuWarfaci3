using System;
using MasterServer.Core;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x02000198 RID: 408
	[ConsoleCmdAttributes(CmdName = "online_query_global_delay", Help = "Adds global query delay", ArgsSize = 100)]
	internal class OnlineQueryGlobalDelayCmd : IConsoleCmd
	{
		// Token: 0x060007B3 RID: 1971 RVA: 0x0001D934 File Offset: 0x0001BD34
		public void ExecuteCmd(string[] args)
		{
			IQueryManager service = ServicesManager.GetService<IQueryManager>();
			int num = args.Length;
			if (num != 1)
			{
				if (num != 2)
				{
					Log.Error("Invalid number of arguments");
				}
				else
				{
					service.QueryExtraDelay = Math.Max(0, int.Parse(args[1]));
					Log.Info<int>("Query extra delay is {0} msec", service.QueryExtraDelay);
				}
			}
			else
			{
				Log.Info<int>("Query global delay is {0} ms", service.QueryExtraDelay);
			}
		}
	}
}
