using System;
using HK2Net;
using HK2Net.Attributes.Bootstrap;
using MasterServer.Core.Services.Metrics;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x02000700 RID: 1792
	[Contract]
	[BootstrapExplicit]
	internal interface IPaymentMetricsTracker : IMetricsProvider, IDisposable
	{
		// Token: 0x0600257B RID: 9595
		void ReportSpendMoneyRequest();

		// Token: 0x0600257C RID: 9596
		void ReportSpendMoneyRequestTime(TimeSpan time);

		// Token: 0x0600257D RID: 9597
		void ReportSpendMoneyRequestFailed();

		// Token: 0x0600257E RID: 9598
		void ReportGetMoneyRequest();

		// Token: 0x0600257F RID: 9599
		void ReportGetMoneyRequestTime(TimeSpan time);

		// Token: 0x06002580 RID: 9600
		void ReportGetMoneyRequestFailed();
	}
}
