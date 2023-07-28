using System;
using MasterServer.Core;

namespace MasterServer.XMPP
{
	// Token: 0x02000813 RID: 2067
	[ConsoleCmdAttributes(CmdName = "presence_enabled", ArgsSize = 1)]
	internal class PresenceEnabledCmd : IConsoleCmd
	{
		// Token: 0x06002A6C RID: 10860 RVA: 0x000B7048 File Offset: 0x000B5448
		public void ExecuteCmd(string[] args)
		{
			IServerPresenceNotifier service = ServicesManager.GetService<IServerPresenceNotifier>();
			if (args.Length == 1)
			{
				Log.Info<int>("presence_enabled = {0}", (!service.PresenceEnabled) ? 0 : 1);
			}
			else
			{
				service.PresenceEnabled = (int.Parse(args[1]) != 0);
			}
		}
	}
}
