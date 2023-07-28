using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core.Services.Metrics;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x0200070C RID: 1804
	[Contract]
	internal interface IQueryMetricsTracker : IMetricsProvider, IDisposable
	{
		// Token: 0x060025A9 RID: 9641
		void AddInterest(string wildcard);

		// Token: 0x060025AA RID: 9642
		void RemoveInterest(int index);

		// Token: 0x060025AB RID: 9643
		List<string> GetInterests();
	}
}
