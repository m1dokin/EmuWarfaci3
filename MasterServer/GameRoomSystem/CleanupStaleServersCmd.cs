using System;
using MasterServer.Core;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000453 RID: 1107
	[ConsoleCmdAttributes(CmdName = "cleanup_stale_servers", ArgsSize = 1)]
	internal class CleanupStaleServersCmd : IConsoleCmd
	{
		// Token: 0x06001778 RID: 6008 RVA: 0x00061A6E File Offset: 0x0005FE6E
		public CleanupStaleServersCmd(IGameRoomServer gameRoomServer)
		{
			this.m_gameRoomServer = gameRoomServer;
		}

		// Token: 0x06001779 RID: 6009 RVA: 0x00061A80 File Offset: 0x0005FE80
		public void ExecuteCmd(string[] args)
		{
			if (args.Length == 2)
			{
				this.m_gameRoomServer.CleanupStaleServers = (args[1] == "1");
			}
			Log.Info<string>("cleanup_stale_servers = {0}", (!this.m_gameRoomServer.CleanupStaleServers) ? "0" : "1");
		}

		// Token: 0x04000B49 RID: 2889
		private readonly IGameRoomServer m_gameRoomServer;
	}
}
