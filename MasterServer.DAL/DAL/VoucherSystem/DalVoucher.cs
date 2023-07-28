using System;

namespace MasterServer.DAL.VoucherSystem
{
	// Token: 0x020000A0 RID: 160
	[Serializable]
	public struct DalVoucher
	{
		// Token: 0x04000194 RID: 404
		public ulong Id;

		// Token: 0x04000195 RID: 405
		public ulong UserId;

		// Token: 0x04000196 RID: 406
		public string Data;

		// Token: 0x04000197 RID: 407
		public VoucherStatus Status;
	}
}
