using System;
using HK2Net;
using MasterServer.Core.Services.Metrics;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x02000719 RID: 1817
	[Contract]
	internal interface IShopSupplierMetricsTracker : IMetricsProvider, IDisposable
	{
		// Token: 0x060025D8 RID: 9688
		void ReportRequest(ShopSupplierRequest request, int shopSupplierId);

		// Token: 0x060025D9 RID: 9689
		void ReportRequestTime(ShopSupplierRequest request, int shopSupplierId, TimeSpan time);

		// Token: 0x060025DA RID: 9690
		void ReportRequestFailed(ShopSupplierRequest request, int shopSupplierId);
	}
}
