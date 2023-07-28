using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog.ShopSupplier;
using MasterServer.Users;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x02000248 RID: 584
	[Contract]
	internal interface IShopSupplier : IDisposable
	{
		// Token: 0x1700016B RID: 363
		// (get) Token: 0x06000CDB RID: 3291
		int SupplierId { get; }

		// Token: 0x06000CDC RID: 3292
		IEnumerable<StoreOffer> GetOffers();

		// Token: 0x06000CDD RID: 3293
		long GetOfferHash();

		// Token: 0x06000CDE RID: 3294
		PurchaseResult PurchaseOffers(UserInfo.User user, IEnumerable<StoreOffer> offers, IPaymentCallback paymentCallback);
	}
}
