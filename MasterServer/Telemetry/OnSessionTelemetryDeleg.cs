using System;
using System.Collections.Generic;
using StatsDataSource.Storage;

namespace MasterServer.Telemetry
{
	// Token: 0x0200072D RID: 1837
	// (Invoke) Token: 0x0600260E RID: 9742
	internal delegate void OnSessionTelemetryDeleg(TelemetryStreamService.SessionData sessionData, List<DataUpdate> telemetry);
}
