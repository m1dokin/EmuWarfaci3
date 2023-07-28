using System;
using MasterServer.Core;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x0200017F RID: 383
	[ConsoleCmdAttributes(CmdName = "online_verbose", ArgsSize = 1)]
	internal class OnlineVerboseCmd : IConsoleCmd
	{
		// Token: 0x06000709 RID: 1801 RVA: 0x0001B40C File Offset: 0x0001980C
		public void ExecuteCmd(string[] args)
		{
			IOnline online = CryOnline.CryOnlineGetInstance();
			IOnlineConfiguration configuration = online.GetConfiguration(Resources.XmppOnlineDomain);
			if (args.Length == 1)
			{
				Log.Info<bool>("online_verbose = {0}", configuration.IsOnlineVerbose());
				return;
			}
			configuration.SetOnlineVerbose(int.Parse(args[1]));
		}
	}
}
