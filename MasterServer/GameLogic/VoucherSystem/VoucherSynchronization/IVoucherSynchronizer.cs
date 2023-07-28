using System;
using System.Collections.Generic;
using HK2Net;

namespace MasterServer.GameLogic.VoucherSystem.VoucherSynchronization
{
	// Token: 0x02000482 RID: 1154
	[Contract]
	public interface IVoucherSynchronizer
	{
		// Token: 0x0600184D RID: 6221
		void Synchronize();

		// Token: 0x0600184E RID: 6222
		IEnumerable<Voucher> SynchronizeCorrupted(ulong startIndex, int count);
	}
}
