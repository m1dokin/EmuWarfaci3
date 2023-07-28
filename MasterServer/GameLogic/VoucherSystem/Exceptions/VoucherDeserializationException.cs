using System;

namespace MasterServer.GameLogic.VoucherSystem.Exceptions
{
	// Token: 0x02000448 RID: 1096
	public class VoucherDeserializationException : VoucherException
	{
		// Token: 0x06001756 RID: 5974 RVA: 0x00060DFC File Offset: 0x0005F1FC
		public VoucherDeserializationException(string message) : base(message)
		{
		}

		// Token: 0x06001757 RID: 5975 RVA: 0x00060E05 File Offset: 0x0005F205
		public VoucherDeserializationException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
