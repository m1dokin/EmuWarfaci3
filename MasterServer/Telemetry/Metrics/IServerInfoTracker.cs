using System;
using HK2Net;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x020006D4 RID: 1748
	[Contract]
	internal interface IServerInfoTracker
	{
		// Token: 0x060024C5 RID: 9413
		void ReportQueueSize(int size);

		// Token: 0x060024C6 RID: 9414
		void ReportAskServerFailed();
	}
}
