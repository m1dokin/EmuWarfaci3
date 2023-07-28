using System;
using MasterServer.Core;

namespace MasterServer.ServerInfo
{
	// Token: 0x020006BD RID: 1725
	[ConsoleCmdAttributes(CmdName = "debug_steal_server", ArgsSize = 1, Help = "Try to steal server by its server ID")]
	internal class DebugStealServerCmd : IConsoleCmd
	{
		// Token: 0x06002427 RID: 9255 RVA: 0x0009725C File Offset: 0x0009565C
		public void ExecuteCmd(string[] args)
		{
			IServerInfo service = ServicesManager.GetService<IServerInfo>();
			if (service.IsGlobalLbsEnabled)
			{
				Log.Error("To execute this command you need to enable local lbs flow first in module_configuration.xml");
				return;
			}
			service.DebugStealServer(args[1]);
		}
	}
}
