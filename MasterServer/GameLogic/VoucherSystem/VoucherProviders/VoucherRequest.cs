using System;

namespace MasterServer.GameLogic.VoucherSystem.VoucherProviders
{
	// Token: 0x02000479 RID: 1145
	internal class VoucherRequest
	{
		// Token: 0x06001817 RID: 6167 RVA: 0x000636DA File Offset: 0x00061ADA
		private VoucherRequest(string name)
		{
			this.Name = name;
		}

		// Token: 0x17000254 RID: 596
		// (get) Token: 0x06001818 RID: 6168 RVA: 0x000636E9 File Offset: 0x00061AE9
		// (set) Token: 0x06001819 RID: 6169 RVA: 0x000636F1 File Offset: 0x00061AF1
		public string Name { get; private set; }

		// Token: 0x04000B91 RID: 2961
		public static readonly VoucherRequest GetVouchersPage = new VoucherRequest("get_vouchers_page");

		// Token: 0x04000B92 RID: 2962
		public static readonly VoucherRequest GetVouchersPerUser = new VoucherRequest("get_vouchers_per_user");

		// Token: 0x04000B93 RID: 2963
		public static readonly VoucherRequest GetVouchersPerUserAll = new VoucherRequest("get_vouchers_per_user_all");

		// Token: 0x04000B94 RID: 2964
		public static readonly VoucherRequest ConfirmVoucher = new VoucherRequest("confirm_voucher");
	}
}
