using System;
using System.Collections.Generic;
using MasterServer.GameLogic.VoucherSystem;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x020001FB RID: 507
	public interface IVoucherSystemClient
	{
		// Token: 0x06000A28 RID: 2600
		ulong GetCurrentIndex(string globalKey);

		// Token: 0x06000A29 RID: 2601
		void SetCurrentIndex(string globalKey, ulong globalValue);

		// Token: 0x06000A2A RID: 2602
		bool AddVoucher(Voucher voucher);

		// Token: 0x06000A2B RID: 2603
		IEnumerable<Voucher> GetCorruptedVouchers(ulong startIndex, int count);

		// Token: 0x06000A2C RID: 2604
		IEnumerable<Voucher> GetNewVouchers(ulong userId);

		// Token: 0x06000A2D RID: 2605
		IEnumerable<Voucher> GetAllVouchers(ulong userId);

		// Token: 0x06000A2E RID: 2606
		Voucher UpdateVoucher(Voucher voucher);

		// Token: 0x06000A2F RID: 2607
		void CleanUpVouchers(ulong userId);
	}
}
