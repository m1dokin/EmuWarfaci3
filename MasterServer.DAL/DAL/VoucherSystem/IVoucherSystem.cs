using System;

namespace MasterServer.DAL.VoucherSystem
{
	// Token: 0x020000A1 RID: 161
	public interface IVoucherSystem
	{
		// Token: 0x06000205 RID: 517
		DALResult<ulong> GetCurrentIndex(string globalKey);

		// Token: 0x06000206 RID: 518
		DALResult<ulong> SetCurrentIndex(string globalKey, ulong globalValue);

		// Token: 0x06000207 RID: 519
		DALResultMulti<DalVoucher> GetNewVouchers(ulong userId);

		// Token: 0x06000208 RID: 520
		DALResultMulti<DalVoucher> GetAllVouchers(ulong userId);

		// Token: 0x06000209 RID: 521
		DALResultMulti<DalVoucher> GetCorruptedVouchers(ulong startIndex, int count);

		// Token: 0x0600020A RID: 522
		DALResult<bool> AddVoucher(DalVoucher voucher);

		// Token: 0x0600020B RID: 523
		DALResult<DalVoucher> UpdateVoucher(DalVoucher voucher);

		// Token: 0x0600020C RID: 524
		DALResultVoid CleanUpVouchers(ulong userId);
	}
}
