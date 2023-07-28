using System;
using MasterServer.Core;
using MasterServer.Database;

namespace MasterServer.Telemetry
{
	// Token: 0x020007D5 RID: 2005
	[ConsoleCmdAttributes(CmdName = "debug_telemetry_reset", Help = "Drop generated test stats")]
	internal class DebugTelemetryResetStatsCmd : IConsoleCmd
	{
		// Token: 0x06002909 RID: 10505 RVA: 0x000B1E1F File Offset: 0x000B021F
		public DebugTelemetryResetStatsCmd(ITelemetryDALService telemetryDal)
		{
			this.m_telemetryDal = telemetryDal;
		}

		// Token: 0x0600290A RID: 10506 RVA: 0x000B1E2E File Offset: 0x000B022E
		public void ExecuteCmd(string[] args)
		{
			this.m_telemetryDal.TelemetrySystem.ResetTelemetryTestStats();
			Log.Info("Debug stat reset done");
		}

		// Token: 0x040015E0 RID: 5600
		private readonly ITelemetryDALService m_telemetryDal;
	}
}
