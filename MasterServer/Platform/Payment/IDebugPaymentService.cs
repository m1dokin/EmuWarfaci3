using System;
using HK2Net;
using HK2Net.Attributes.Bootstrap;

namespace MasterServer.Platform.Payment
{
	// Token: 0x02000692 RID: 1682
	[Contract]
	[BootstrapExplicit]
	public interface IDebugPaymentService
	{
		// Token: 0x0600235B RID: 9051
		void SetMoney(ulong userId, ulong amount);

		// Token: 0x0600235C RID: 9052
		ulong AddMoney(ulong userId, ulong amount);

		// Token: 0x0600235D RID: 9053
		PaymentResult SpendMoney(ulong userId, ulong amount);

		// Token: 0x0600235E RID: 9054
		void SetRequestFailedState(bool state);

		// Token: 0x0600235F RID: 9055
		bool GetRequestFailedState();

		// Token: 0x06002360 RID: 9056
		void SetRequestTimeout(TimeSpan timeout);

		// Token: 0x06002361 RID: 9057
		TimeSpan GetRequestTimeout();
	}
}
