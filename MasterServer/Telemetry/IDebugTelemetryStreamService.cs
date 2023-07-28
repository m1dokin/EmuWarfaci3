using System;
using System.Collections.Generic;
using HK2Net;
using StatsDataSource.Storage;

namespace MasterServer.Telemetry
{
	// Token: 0x0200072F RID: 1839
	[Contract]
	internal interface IDebugTelemetryStreamService
	{
		// Token: 0x06002614 RID: 9748
		void SimulateTelemetryEvent(TelemetryStreamService.SessionData sessionData, List<DataUpdate> telemetry);
	}
}
