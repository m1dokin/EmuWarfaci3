using System;
using System.Runtime.Serialization;

namespace MasterServer.ElectronicCatalog.Exceptions
{
	// Token: 0x0200024B RID: 587
	[Serializable]
	public class ShopServiceInternalErrorException : ShopServiceException
	{
		// Token: 0x06000CF2 RID: 3314 RVA: 0x00032275 File Offset: 0x00030675
		public ShopServiceInternalErrorException() : this(string.Empty, null)
		{
		}

		// Token: 0x06000CF3 RID: 3315 RVA: 0x00032283 File Offset: 0x00030683
		public ShopServiceInternalErrorException(string message) : this(message, null)
		{
		}

		// Token: 0x06000CF4 RID: 3316 RVA: 0x0003228D File Offset: 0x0003068D
		public ShopServiceInternalErrorException(Exception inner) : this("shop service internal error.", inner)
		{
		}

		// Token: 0x06000CF5 RID: 3317 RVA: 0x0003229B File Offset: 0x0003069B
		public ShopServiceInternalErrorException(string message, Exception inner) : base(message, inner)
		{
		}

		// Token: 0x06000CF6 RID: 3318 RVA: 0x000322A5 File Offset: 0x000306A5
		protected ShopServiceInternalErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
