using System;
using HK2Net;
using MasterServer.Core.Services.Metrics;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x020006F8 RID: 1784
	[Contract]
	internal interface ILogMetricsTracker : IMetricsProvider, IDisposable
	{
		// Token: 0x06002565 RID: 9573
		void ReportLogMessageSent(string logCategory);
	}
}
