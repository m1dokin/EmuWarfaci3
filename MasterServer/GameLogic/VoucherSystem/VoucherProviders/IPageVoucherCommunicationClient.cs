using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HK2Net;

namespace MasterServer.GameLogic.VoucherSystem.VoucherProviders
{
	// Token: 0x02000474 RID: 1140
	[Contract]
	internal interface IPageVoucherCommunicationClient : IDisposable
	{
		// Token: 0x060017FB RID: 6139
		Task<IEnumerable<Voucher>> GetVouchers(ulong pageIndex, int pageSize);
	}
}
