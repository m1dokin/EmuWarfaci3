using System;
using System.Collections.Generic;
using HK2Net;

namespace MasterServer.GameLogic.VoucherSystem
{
	// Token: 0x0200044A RID: 1098
	[Contract]
	internal interface IDebugVoucherService
	{
		// Token: 0x0600175A RID: 5978
		IEnumerable<Voucher> GetAllVouchers(ulong userId);
	}
}
