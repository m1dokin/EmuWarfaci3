using System;
using System.Collections.Generic;
using MasterServer.Core;
using MasterServer.Database;
using OLAPHypervisor;

namespace MasterServer.GameLogic.PlayerStats
{
	// Token: 0x02000537 RID: 1335
	[ConsoleCmdAttributes(CmdName = "stats_restore", ArgsSize = 1, Help = "Restore players stats from telemDB to GameDB")]
	internal class StatsRestoreCmd : IConsoleCmd
	{
		// Token: 0x06001D04 RID: 7428 RVA: 0x0007525B File Offset: 0x0007365B
		public StatsRestoreCmd(ITelemetryDALService telemetryDal, IPlayerStatsService statsService, IDebugPlayerStatsService statsServiceDebug)
		{
			this.m_telemetryDal = telemetryDal;
			this.m_statsService = statsService;
			this.m_statsServiceDebug = statsServiceDebug;
		}

		// Token: 0x06001D05 RID: 7429 RVA: 0x00075278 File Offset: 0x00073678
		public void ExecuteCmd(string[] args)
		{
			ulong num = ulong.Parse(args[1]);
			List<Measure> playerStats = this.m_statsService.GetPlayerStats(num);
			if (playerStats.Count == 0)
			{
				MasterServer.Core.Log.Info<ulong>("Restoring player stats for {0}", num);
				this.m_statsService.InitPlayerStats(num);
			}
			else
			{
				MasterServer.Core.Log.Info<ulong>("Player stats already restored for {0}", num);
			}
		}

		// Token: 0x04000DCF RID: 3535
		private readonly ITelemetryDALService m_telemetryDal;

		// Token: 0x04000DD0 RID: 3536
		private readonly IDebugPlayerStatsService m_statsServiceDebug;

		// Token: 0x04000DD1 RID: 3537
		private readonly IPlayerStatsService m_statsService;
	}
}
