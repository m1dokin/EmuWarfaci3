using System;
using System.Collections.Generic;
using MasterServer.Core;
using MasterServer.GameLogic.GameInterface;
using OLAPHypervisor;

namespace MasterServer.GameLogic.PlayerStats
{
	// Token: 0x0200053B RID: 1339
	[ConsoleCmdAttributes(CmdName = "stats_debug_add", ArgsSize = 2, Help = "Generate debug player stats")]
	internal class DebugAddStatsCmd : IConsoleCmd
	{
		// Token: 0x06001D0C RID: 7436 RVA: 0x0007544B File Offset: 0x0007384B
		public DebugAddStatsCmd(IDebugPlayerStatsService statsService)
		{
			this.m_statsService = statsService;
		}

		// Token: 0x06001D0D RID: 7437 RVA: 0x0007545C File Offset: 0x0007385C
		public void ExecuteCmd(string[] args)
		{
			List<Measure> list = new List<Measure>();
			int num = int.Parse(args[2]);
			for (int i = 0; i < num; i++)
			{
				list.Add(new Measure
				{
					AggregateOp = EAggOperation.Sum,
					RowCount = 1L,
					Value = (long)i,
					Dimensions = new SortedList<string, string>
					{
						{
							"stat",
							string.Format("player_sessions_won{0}", i)
						},
						{
							"mode",
							"PVP"
						}
					}
				});
			}
			foreach (ulong num2 in GameInterfaceCmd.GetProfiles(args[1]))
			{
				MasterServer.Core.Log.Info<ulong>("Adding stats for profile {0}", num2);
				this.m_statsService.FlushPlayerStats(num2, list);
			}
		}

		// Token: 0x04000DD7 RID: 3543
		private readonly IDebugPlayerStatsService m_statsService;
	}
}
