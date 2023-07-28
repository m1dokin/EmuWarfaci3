using System;
using HK2Net;
using MasterServer.Core.Services.Metrics;
using MasterServer.GameLogic.VoucherSystem.VoucherProviders;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x020006DA RID: 1754
	[Contract]
	internal interface IVoucherMetricsTracker : IMetricsProvider, IDisposable
	{
		// Token: 0x060024DD RID: 9437
		void ReportRequest(VoucherRequest request);

		// Token: 0x060024DE RID: 9438
		void ReportRequestTime(VoucherRequest request, TimeSpan time);

		// Token: 0x060024DF RID: 9439
		void ReportRequestFailed(VoucherRequest request);
	}
}
