using System;

namespace MasterServer.GameLogic.VoucherSystem.Exceptions
{
	// Token: 0x0200046E RID: 1134
	public class VoucherAddException : VoucherException
	{
		// Token: 0x060017EE RID: 6126 RVA: 0x000631DA File Offset: 0x000615DA
		public VoucherAddException(Voucher voucher) : base(string.Format("Adding voucher failed, voucher with same id already exist, {0}", voucher))
		{
		}
	}
}
