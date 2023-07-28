using System;
using System.Threading.Tasks;
using HK2Net;
using HK2Net.Attributes.Bootstrap;
using MasterServer.DAL;

namespace MasterServer.Platform.Payment
{
	// Token: 0x0200069C RID: 1692
	[Contract]
	[BootstrapExplicit]
	public interface IPaymentService
	{
		// Token: 0x06002373 RID: 9075
		Task<ulong> GetMoneyAsync(ulong userId);

		// Token: 0x06002374 RID: 9076
		ulong GetMoney(ulong userId);

		// Token: 0x06002375 RID: 9077
		PaymentResult SpendMoney(ulong userId, ulong amount, SpendMoneyReason spendMoneyReason);

		// Token: 0x06002376 RID: 9078
		PaymentResult SpendMoney(ulong userId, StoreOffer offer);
	}
}
