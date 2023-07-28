using System;
using HK2Net;

namespace MasterServer.GameLogic.VoucherSystem.VoucherSynchronization
{
	// Token: 0x02000481 RID: 1153
	[Contract]
	internal interface IDebugVoucherSynchronizer
	{
		// Token: 0x0600184A RID: 6218
		ulong GetCurrentIndex();

		// Token: 0x0600184B RID: 6219
		void SetCurrentIndex(ulong index);

		// Token: 0x0600184C RID: 6220
		void CleanUpVouchers(ulong userId);
	}
}
