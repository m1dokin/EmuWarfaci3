using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HK2Net;

namespace MasterServer.GameLogic.VoucherSystem.VoucherProviders
{
	// Token: 0x0200044B RID: 1099
	[Contract]
	internal interface IVoucherCommunicationClient : IDisposable
	{
		// Token: 0x0600175B RID: 5979
		Task<IEnumerable<Voucher>> GetNewVouchers(ulong userId);

		// Token: 0x0600175C RID: 5980
		void UpdateVoucher(Voucher voucher);

		// Token: 0x0600175D RID: 5981
		IEnumerable<Voucher> GetAllVouchers(ulong userId);
	}
}
