using System;
using System.Text;
using MasterServer.Core;

namespace MasterServer.Matchmaking
{
	// Token: 0x02000501 RID: 1281
	[ConsoleCmdAttributes(CmdName = "mm_get_autostart_missions", ArgsSize = 0, Help = "Returns autostart missions population on current MS.")]
	internal class GetAutostartMissionsCmd : IConsoleCmd
	{
		// Token: 0x06001BB3 RID: 7091 RVA: 0x00070432 File Offset: 0x0006E832
		public GetAutostartMissionsCmd(IMatchmakingMissionsProvider matchmakingMissionsProvider)
		{
			this.m_matchmakingMissionsProvider = matchmakingMissionsProvider;
		}

		// Token: 0x06001BB4 RID: 7092 RVA: 0x00070444 File Offset: 0x0006E844
		public void ExecuteCmd(string[] args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("\nAutostart missions:");
			foreach (string arg in this.m_matchmakingMissionsProvider.AutostartMissions)
			{
				stringBuilder.AppendFormat("\tMission:{0}\r\n", arg);
			}
			Log.Info(stringBuilder.ToString());
		}

		// Token: 0x04000D44 RID: 3396
		private readonly IMatchmakingMissionsProvider m_matchmakingMissionsProvider;
	}
}
