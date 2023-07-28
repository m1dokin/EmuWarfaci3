using System;

namespace MasterServer.DAL
{
	// Token: 0x0200002F RID: 47
	public interface IPaymentCallback
	{
		// Token: 0x06000078 RID: 120
		PaymentCallbackResult SpendMoneyByOfferId(ulong userId, int supplierId, ulong offerId);

		// Token: 0x06000079 RID: 121
		PaymentCallbackResult SpendMoney(ulong userId, ulong amount, SpendMoneyReason spendMoneyReason);
	}
}
