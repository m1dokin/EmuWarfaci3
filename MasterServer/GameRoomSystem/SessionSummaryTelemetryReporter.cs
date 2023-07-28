using System;
using MasterServer.Core;
using MasterServer.GameLogic.PlayerStats;
using MasterServer.Telemetry;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000630 RID: 1584
	internal class SessionSummaryTelemetryReporter
	{
		// Token: 0x06002208 RID: 8712 RVA: 0x0008DA98 File Offset: 0x0008BE98
		public SessionSummaryTelemetryReporter(ISessionSummaryService summaryService, IPlayerStatsService playerStatsService, ITelemetryService telemetryService)
		{
			this.m_summaryService = summaryService;
			this.m_playerStatsService = playerStatsService;
			this.m_telemetryService = telemetryService;
			this.m_summaryService.SessionSummaryFinalized += this.WriteSummaryToTelemetry;
		}

		// Token: 0x06002209 RID: 8713 RVA: 0x0008DACC File Offset: 0x0008BECC
		private void WriteSummaryToTelemetry(SessionSummary data)
		{
			this.m_telemetryService.AddMeasure(data.MeasuresList);
			this.m_playerStatsService.UpdatePlayerStats(data.MeasuresList);
			Log.Info(string.Format("Telemetry stream for session '{0}' done, {1} DB updates created", data.SessionId, data.MeasuresList.Count));
		}

		// Token: 0x040010C1 RID: 4289
		private readonly ISessionSummaryService m_summaryService;

		// Token: 0x040010C2 RID: 4290
		private readonly IPlayerStatsService m_playerStatsService;

		// Token: 0x040010C3 RID: 4291
		private readonly ITelemetryService m_telemetryService;
	}
}
