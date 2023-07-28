using System;
using MasterServer.Core;

namespace MasterServer.ServerInfo
{
	// Token: 0x020006BC RID: 1724
	[ConsoleCmdAttributes(CmdName = "debug_release_server")]
	internal class DebugReleaseServerCmd : ConsoleCommand<DebugReleaseServerCmdParams>
	{
		// Token: 0x06002424 RID: 9252 RVA: 0x000971DF File Offset: 0x000955DF
		public DebugReleaseServerCmd(IServerInfo serverInfo)
		{
			this.m_serverInfo = serverInfo;
		}

		// Token: 0x06002425 RID: 9253 RVA: 0x000971F0 File Offset: 0x000955F0
		protected override void Execute(DebugReleaseServerCmdParams param)
		{
			if (!this.m_serverInfo.IsGlobalLbsEnabled)
			{
				Log.Error("To execute this command you need to enable global lbs flow first in module_configuration.xml");
				return;
			}
			if (Resources.DebugQueriesEnabled)
			{
				this.m_serverInfo.ReleaseServer(param.ServerId, param.ForceRelease);
			}
			else
			{
				this.m_serverInfo.ReleaseServer(param.ServerId, false);
			}
		}

		// Token: 0x0400121F RID: 4639
		private readonly IServerInfo m_serverInfo;
	}
}
