using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HK2Net;

namespace MasterServer.GameLogic.VoucherSystem.VoucherProviders
{
	// Token: 0x0200047D RID: 1149
	[Contract]
	internal interface IVoucherProvider
	{
		// Token: 0x0600183C RID: 6204
		Task<IEnumerable<Voucher>> GetNewVouchers(ulong userId);

		// Token: 0x0600183D RID: 6205
		IEnumerable<Voucher> GetAllVouchersForUser(ulong userId);

		// Token: 0x0600183E RID: 6206
		void ReportVoucher(Voucher voucher);
	}
}
