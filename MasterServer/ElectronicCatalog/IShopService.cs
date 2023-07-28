using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog.ShopSupplier;
using MasterServer.Users;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x02000247 RID: 583
	[Contract]
	internal interface IShopService
	{
		// Token: 0x14000029 RID: 41
		// (add) Token: 0x06000CD4 RID: 3284
		// (remove) Token: 0x06000CD5 RID: 3285
		event Action<IEnumerable<StoreOffer>> OffersUpdated;

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x06000CD6 RID: 3286
		long OffersHash { get; }

		// Token: 0x06000CD7 RID: 3287
		IEnumerable<StoreOffer> GetOffers();

		// Token: 0x06000CD8 RID: 3288
		StoreOffer GetStoreOfferById(int supplierId, ulong offerId);

		// Token: 0x06000CD9 RID: 3289
		PurchaseResult PurchaseOffers(UserInfo.User user, long offerHash, IEnumerable<StoreOffer> offers, IPaymentCallback paymentCallback);

		// Token: 0x06000CDA RID: 3290
		void LoadOffers();
	}
}
