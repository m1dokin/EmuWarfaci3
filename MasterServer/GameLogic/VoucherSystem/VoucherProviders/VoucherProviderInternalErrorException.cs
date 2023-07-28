using System;

namespace MasterServer.GameLogic.VoucherSystem.VoucherProviders
{
	// Token: 0x02000478 RID: 1144
	internal class VoucherProviderInternalErrorException : Exception
	{
		// Token: 0x06001815 RID: 6165 RVA: 0x000636C3 File Offset: 0x00061AC3
		public VoucherProviderInternalErrorException(Exception exc) : base("voucher provider internal error", exc)
		{
		}

		// Token: 0x06001816 RID: 6166 RVA: 0x000636D1 File Offset: 0x00061AD1
		public VoucherProviderInternalErrorException(string message) : base(message)
		{
		}
	}
}
