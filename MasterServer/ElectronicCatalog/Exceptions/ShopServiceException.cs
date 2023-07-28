using System;
using System.Runtime.Serialization;

namespace MasterServer.ElectronicCatalog.Exceptions
{
	// Token: 0x0200024A RID: 586
	[Serializable]
	public abstract class ShopServiceException : Exception
	{
		// Token: 0x06000CEF RID: 3311 RVA: 0x00032253 File Offset: 0x00030653
		protected ShopServiceException() : this(string.Empty, null)
		{
		}

		// Token: 0x06000CF0 RID: 3312 RVA: 0x00032261 File Offset: 0x00030661
		protected ShopServiceException(string message, Exception inner) : base(message, inner)
		{
		}

		// Token: 0x06000CF1 RID: 3313 RVA: 0x0003226B File Offset: 0x0003066B
		protected ShopServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
