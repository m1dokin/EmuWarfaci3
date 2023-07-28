using System;
using System.Linq;
using MasterServer.Core;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x020001B3 RID: 435
	[ConsoleCmdAttributes(CmdName = "qos_set_class_concurrency", ArgsSize = 2)]
	internal class QoSSetClassConcurrencyCmd : IConsoleCmd
	{
		// Token: 0x0600081F RID: 2079 RVA: 0x0001F414 File Offset: 0x0001D814
		public void ExecuteCmd(string[] args)
		{
			IQoSQueue service = ServicesManager.GetService<IQoSQueue>();
			QueryClassShaper queryClassShaper = service.GetShapers().FirstOrDefault((IQoSShaper s) => s is QueryClassShaper) as QueryClassShaper;
			QueryClassShaper.QoSParams @params = queryClassShaper.GetParams(args[1]);
			if (args.Length == 2)
			{
				Log.Info<string, int>("QoS {0} concurrency limit {1}", args[1], @params.ConcurrencyLimit);
				return;
			}
			int num = int.Parse(args[2]);
			if (num < 0)
			{
				Log.Info("Invalid concurrency value");
			}
			else
			{
				@params.ConcurrencyLimit = num;
			}
			queryClassShaper.SetParams(args[1], @params);
			Log.Info<string, int>("QoS {0} concurrency limit {1}", args[1], num);
		}
	}
}
