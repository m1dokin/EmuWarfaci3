using System;
using System.Threading.Tasks;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.Platform.Payment.Exceptions;
using MasterServer.Telemetry.Metrics;

namespace MasterServer.Platform.Payment
{
	// Token: 0x02000693 RID: 1683
	internal abstract class ReadOnlyPaymentService : PaymentService
	{
		// Token: 0x06002362 RID: 9058 RVA: 0x000958F6 File Offset: 0x00093CF6
		protected ReadOnlyPaymentService(IPaymentMetricsTracker paymentMetricsTracker, ILogService logService) : base(paymentMetricsTracker, logService)
		{
		}

		// Token: 0x06002363 RID: 9059 RVA: 0x00095900 File Offset: 0x00093D00
		protected override Task<PaymentResult> SpendMoneyImpl(ulong userId, ulong amount, SpendMoneyReason spendMoneyReason)
		{
			throw new PaymentServiceException("Spend money doesn't supported be read-only payment service");
		}

		// Token: 0x06002364 RID: 9060 RVA: 0x0009590C File Offset: 0x00093D0C
		protected override Task<PaymentResult> SpendMoneyImpl(ulong userId, StoreOffer offer)
		{
			throw new PaymentServiceException("Spend money doesn't supported be read-only payment service");
		}
	}
}
