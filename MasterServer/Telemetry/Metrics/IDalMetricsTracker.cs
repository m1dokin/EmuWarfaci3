using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core.Services.Metrics;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x020006ED RID: 1773
	[Contract]
	internal interface IDalMetricsTracker : IMetricsProvider, IDisposable
	{
		// Token: 0x0600253E RID: 9534
		void AddInterest(string wildcard);

		// Token: 0x0600253F RID: 9535
		void RemoveInterest(int index);

		// Token: 0x06002540 RID: 9536
		List<string> GetInterests();
	}
}
