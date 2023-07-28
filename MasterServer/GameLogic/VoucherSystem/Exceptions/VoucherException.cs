using System;

namespace MasterServer.GameLogic.VoucherSystem.Exceptions
{
	// Token: 0x0200046F RID: 1135
	public class VoucherException : Exception
	{
		// Token: 0x060017EF RID: 6127 RVA: 0x00060DE9 File Offset: 0x0005F1E9
		public VoucherException(string message) : base(message)
		{
		}

		// Token: 0x060017F0 RID: 6128 RVA: 0x00060DF2 File Offset: 0x0005F1F2
		public VoucherException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
