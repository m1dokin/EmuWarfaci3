using System;
using MasterServer.Core;

namespace MasterServer.Matchmaking
{
	// Token: 0x02000500 RID: 1280
	[ConsoleCmdAttributes(CmdName = "mm_quickplay_reload_maplist", ArgsSize = 0, Help = "Reloads quickplay map set.")]
	internal class QuickplayReloadMaplistCmd : IConsoleCmd
	{
		// Token: 0x06001BB1 RID: 7089 RVA: 0x0007040C File Offset: 0x0006E80C
		public QuickplayReloadMaplistCmd(IMatchmakingMissionsProvider matchmakingMissionsProvider)
		{
			this.m_matchmakingMissionsProvider = matchmakingMissionsProvider;
		}

		// Token: 0x06001BB2 RID: 7090 RVA: 0x0007041B File Offset: 0x0006E81B
		public void ExecuteCmd(string[] args)
		{
			this.m_matchmakingMissionsProvider.ReloadMapLists();
			Log.Info("[Autostart] Quickplay map list reloaded by Console Command");
		}

		// Token: 0x04000D43 RID: 3395
		private readonly IMatchmakingMissionsProvider m_matchmakingMissionsProvider;
	}
}
