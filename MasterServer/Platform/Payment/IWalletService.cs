using System;
using HK2Net;
using HK2Net.Attributes.Bootstrap;

namespace MasterServer.Platform.Payment
{
	// Token: 0x0200068E RID: 1678
	[Contract]
	[BootstrapExplicit]
	public interface IWalletService
	{
		// Token: 0x06002331 RID: 9009
		ulong GetMoney(ulong userId);

		// Token: 0x06002332 RID: 9010
		void SetMoney(ulong userId, ulong amount);

		// Token: 0x06002333 RID: 9011
		ulong AddMoney(ulong userId, ulong amount);

		// Token: 0x06002334 RID: 9012
		PaymentResult SpendMoney(ulong userId, ulong amount);

		// Token: 0x06002335 RID: 9013
		void SetRequestFailedState(bool state);

		// Token: 0x06002336 RID: 9014
		bool GetRequestFailedState();

		// Token: 0x06002337 RID: 9015
		void SetRequestTimeout(TimeSpan timeout);

		// Token: 0x06002338 RID: 9016
		TimeSpan GetRequestTimeout();
	}
}
