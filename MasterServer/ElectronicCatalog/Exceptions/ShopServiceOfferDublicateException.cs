using System;
using System.Runtime.Serialization;

namespace MasterServer.ElectronicCatalog.Exceptions
{
	// Token: 0x0200024C RID: 588
	[Serializable]
	internal class ShopServiceOfferDublicateException : ShopServiceException
	{
		// Token: 0x06000CF7 RID: 3319 RVA: 0x000322AF File Offset: 0x000306AF
		public ShopServiceOfferDublicateException(string message) : base(message, null)
		{
		}

		// Token: 0x06000CF8 RID: 3320 RVA: 0x000322B9 File Offset: 0x000306B9
		protected ShopServiceOfferDublicateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
