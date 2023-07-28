using System;
using System.Runtime.Serialization;

namespace MasterServer.Platform.Payment.Exceptions
{
	// Token: 0x020006A0 RID: 1696
	[Serializable]
	public class PaymentServiceInternalErrorException : PaymentServiceException
	{
		// Token: 0x060023A0 RID: 9120 RVA: 0x00095E67 File Offset: 0x00094267
		public PaymentServiceInternalErrorException() : this(string.Empty, null)
		{
		}

		// Token: 0x060023A1 RID: 9121 RVA: 0x00095E75 File Offset: 0x00094275
		public PaymentServiceInternalErrorException(string message) : this(message, null)
		{
		}

		// Token: 0x060023A2 RID: 9122 RVA: 0x00095E7F File Offset: 0x0009427F
		public PaymentServiceInternalErrorException(Exception inner) : this("Payment service internal error.", inner)
		{
		}

		// Token: 0x060023A3 RID: 9123 RVA: 0x00095E8D File Offset: 0x0009428D
		public PaymentServiceInternalErrorException(string message, Exception inner) : base(message, inner)
		{
		}

		// Token: 0x060023A4 RID: 9124 RVA: 0x00095E97 File Offset: 0x00094297
		protected PaymentServiceInternalErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
