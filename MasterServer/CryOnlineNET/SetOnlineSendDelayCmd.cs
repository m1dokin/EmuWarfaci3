using System;
using MasterServer.Core;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x02000181 RID: 385
	[ConsoleCmdAttributes(CmdName = "online_send_delay", ArgsSize = 1)]
	internal class SetOnlineSendDelayCmd : IConsoleCmd
	{
		// Token: 0x0600070E RID: 1806 RVA: 0x0001B544 File Offset: 0x00019944
		public void ExecuteCmd(string[] args)
		{
			IOnlineClient service = ServicesManager.GetService<IOnlineClient>();
			if (args.Length == 1)
			{
				Log.Info<int>("online_send_delay = {0}", service.GetSendDelay());
				return;
			}
			int num = int.Parse(args[1]);
			if (num < 0)
			{
				Log.Info("Invalid delay value");
			}
			else
			{
				service.SetSendDelay(num);
			}
		}
	}
}
