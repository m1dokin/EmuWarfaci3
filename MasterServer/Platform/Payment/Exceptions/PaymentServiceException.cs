using System;
using System.Runtime.Serialization;

namespace MasterServer.Platform.Payment.Exceptions
{
	// Token: 0x0200069F RID: 1695
	[Serializable]
	public class PaymentServiceException : Exception
	{
		// Token: 0x0600239B RID: 9115 RVA: 0x00095E2D File Offset: 0x0009422D
		public PaymentServiceException() : this(string.Empty, null)
		{
		}

		// Token: 0x0600239C RID: 9116 RVA: 0x00095E3B File Offset: 0x0009423B
		public PaymentServiceException(string message) : base(message, null)
		{
		}

		// Token: 0x0600239D RID: 9117 RVA: 0x00095E45 File Offset: 0x00094245
		public PaymentServiceException(Exception inner) : base(string.Empty, inner)
		{
		}

		// Token: 0x0600239E RID: 9118 RVA: 0x00095E53 File Offset: 0x00094253
		public PaymentServiceException(string message, Exception inner) : base(message, inner)
		{
		}

		// Token: 0x0600239F RID: 9119 RVA: 0x00095E5D File Offset: 0x0009425D
		public PaymentServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
