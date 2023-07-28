using System;
using System.Collections.Generic;
using MasterServer.Core;
using OLAPHypervisor;

namespace MasterServer.GameLogic.PlayerStats
{
	// Token: 0x0200053A RID: 1338
	[ConsoleCmdAttributes(CmdName = "stats_reset_pvp", ArgsSize = 1, Help = "Reset PvP player stats")]
	internal class ResetStatsPvPCmd : IConsoleCmd
	{
		// Token: 0x06001D0A RID: 7434 RVA: 0x000753E0 File Offset: 0x000737E0
		public ResetStatsPvPCmd(IPlayerStatsService statsService, IPlayerStatsFactory statsFactory)
		{
			this.m_statsService = statsService;
			this.m_statsFactory = statsFactory;
		}

		// Token: 0x06001D0B RID: 7435 RVA: 0x000753F8 File Offset: 0x000737F8
		public void ExecuteCmd(string[] args)
		{
			ulong num = ulong.Parse(args[1]);
			List<Measure> data = this.m_statsFactory.PvPKillDeathRatioReset(num);
			List<Measure> data2 = this.m_statsFactory.PvPWinLoseRatioReset(num);
			this.m_statsService.UpdatePlayerStats(data);
			this.m_statsService.UpdatePlayerStats(data2);
			MasterServer.Core.Log.Info<ulong>("PVP stats is reset for profile {0}", num);
		}

		// Token: 0x04000DD5 RID: 3541
		private readonly IPlayerStatsService m_statsService;

		// Token: 0x04000DD6 RID: 3542
		private readonly IPlayerStatsFactory m_statsFactory;
	}
}
