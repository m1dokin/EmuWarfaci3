using System;
using HK2Net;

namespace MasterServer.GameLogic.VoucherSystem
{
	// Token: 0x02000472 RID: 1138
	[Contract]
	internal interface IVoucherValidator
	{
		// Token: 0x060017F4 RID: 6132
		bool IsValid(Voucher voucher);
	}
}
