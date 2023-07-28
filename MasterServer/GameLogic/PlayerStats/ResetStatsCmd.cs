using System;
using MasterServer.Core;
using MasterServer.Database;

namespace MasterServer.GameLogic.PlayerStats
{
	// Token: 0x02000539 RID: 1337
	[ConsoleCmdAttributes(CmdName = "stats_reset", ArgsSize = 2, Help = "Reset player stats")]
	internal class ResetStatsCmd : IConsoleCmd
	{
		// Token: 0x06001D08 RID: 7432 RVA: 0x00075368 File Offset: 0x00073768
		public ResetStatsCmd(ITelemetryDALService telemetryDal, IDebugPlayerStatsService statsService)
		{
			this.m_telemetryDal = telemetryDal;
			this.m_statsService = statsService;
		}

		// Token: 0x06001D09 RID: 7433 RVA: 0x00075380 File Offset: 0x00073780
		public void ExecuteCmd(string[] args)
		{
			ulong num = ulong.Parse(args[1]);
			bool flag = args.Length > 2;
			Log.Info<ulong, string>("Reset PlayerStats for {0}, {1}", num, (!flag) ? "not full" : "full");
			this.m_statsService.ResetPlayerStats(num);
			if (flag)
			{
				this.m_telemetryDal.TelemetrySystem.ResetPlayerStats(num);
			}
		}

		// Token: 0x04000DD3 RID: 3539
		private readonly ITelemetryDALService m_telemetryDal;

		// Token: 0x04000DD4 RID: 3540
		private readonly IDebugPlayerStatsService m_statsService;
	}
}
