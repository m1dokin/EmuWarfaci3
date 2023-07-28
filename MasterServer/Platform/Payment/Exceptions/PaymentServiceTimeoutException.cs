using System;
using System.Runtime.Serialization;

namespace MasterServer.Platform.Payment.Exceptions
{
	// Token: 0x020006A1 RID: 1697
	[Serializable]
	public class PaymentServiceTimeoutException : PaymentServiceException
	{
		// Token: 0x060023A5 RID: 9125 RVA: 0x00095EA1 File Offset: 0x000942A1
		public PaymentServiceTimeoutException(TimeSpan timeout) : this(timeout, null)
		{
		}

		// Token: 0x060023A6 RID: 9126 RVA: 0x00095EAB File Offset: 0x000942AB
		public PaymentServiceTimeoutException(TimeSpan timeout, Exception inner) : base(string.Format("Payment service call timeout {0}.", timeout), inner)
		{
		}

		// Token: 0x060023A7 RID: 9127 RVA: 0x00095EC4 File Offset: 0x000942C4
		protected PaymentServiceTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
