using System;
using System.Linq;
using MasterServer.Core;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x020001B2 RID: 434
	[ConsoleCmdAttributes(CmdName = "qos_set_class_queue", ArgsSize = 2)]
	internal class QoSSetClassQueueCmd : IConsoleCmd
	{
		// Token: 0x0600081C RID: 2076 RVA: 0x0001F358 File Offset: 0x0001D758
		public void ExecuteCmd(string[] args)
		{
			IQoSQueue service = ServicesManager.GetService<IQoSQueue>();
			QueryClassShaper queryClassShaper = service.GetShapers().FirstOrDefault((IQoSShaper s) => s is QueryClassShaper) as QueryClassShaper;
			QueryClassShaper.QoSParams @params = queryClassShaper.GetParams(args[1]);
			if (args.Length == 2)
			{
				Log.Info<string, int>("QoS {0} queue limit {1}", args[1], @params.QueueLimit);
				return;
			}
			int num = int.Parse(args[2]);
			if (num < 0)
			{
				Log.Info("Invalid queue value");
			}
			else
			{
				@params.QueueLimit = num;
			}
			queryClassShaper.SetParams(args[1], @params);
			Log.Info<string, int>("QoS {0} queue limit {1}", args[1], num);
		}
	}
}
