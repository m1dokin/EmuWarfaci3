using System;
using System.Collections.Generic;
using MasterServer.Core;
using OLAPHypervisor;

namespace MasterServer.GameLogic.PlayerStats
{
	// Token: 0x02000538 RID: 1336
	[ConsoleCmdAttributes(CmdName = "stats_get_agg", ArgsSize = 1, Help = "Show player stats after aggregation")]
	internal class StatsGetAggCmd : IConsoleCmd
	{
		// Token: 0x06001D06 RID: 7430 RVA: 0x000752CD File Offset: 0x000736CD
		public StatsGetAggCmd(IPlayerStatsService statsService)
		{
			this.m_statsService = statsService;
		}

		// Token: 0x06001D07 RID: 7431 RVA: 0x000752DC File Offset: 0x000736DC
		public void ExecuteCmd(string[] args)
		{
			ulong num = ulong.Parse(args[1]);
			List<Measure> playerStatsAggregated = this.m_statsService.GetPlayerStatsAggregated(num);
			MasterServer.Core.Log.Info<ulong, int>("PlayerStats for {0}, count: {1}", num, playerStatsAggregated.Count);
			foreach (Measure measure in playerStatsAggregated)
			{
				MasterServer.Core.Log.Info(measure.ToString());
			}
		}

		// Token: 0x04000DD2 RID: 3538
		private readonly IPlayerStatsService m_statsService;
	}
}
