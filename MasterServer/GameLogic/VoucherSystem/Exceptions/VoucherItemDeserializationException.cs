using System;

namespace MasterServer.GameLogic.VoucherSystem.Exceptions
{
	// Token: 0x02000449 RID: 1097
	public class VoucherItemDeserializationException : VoucherException
	{
		// Token: 0x06001758 RID: 5976 RVA: 0x00060E0F File Offset: 0x0005F20F
		public VoucherItemDeserializationException(string message) : base(message)
		{
		}

		// Token: 0x06001759 RID: 5977 RVA: 0x00060E18 File Offset: 0x0005F218
		public VoucherItemDeserializationException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
