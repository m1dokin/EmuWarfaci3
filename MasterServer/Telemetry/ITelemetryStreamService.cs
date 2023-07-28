using System;
using HK2Net;

namespace MasterServer.Telemetry
{
	// Token: 0x0200072E RID: 1838
	[Contract]
	internal interface ITelemetryStreamService
	{
		// Token: 0x140000A1 RID: 161
		// (add) Token: 0x06002611 RID: 9745
		// (remove) Token: 0x06002612 RID: 9746
		event OnSessionTelemetryDeleg OnSessionTelemetry;

		// Token: 0x06002613 RID: 9747
		bool Process(TelemetryStreamService.StreamPacket packet);
	}
}
