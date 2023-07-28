using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.Users;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000351 RID: 849
	[Contract]
	internal interface IItemsPurchase
	{
		// Token: 0x14000039 RID: 57
		// (add) Token: 0x060012FD RID: 4861
		// (remove) Token: 0x060012FE RID: 4862
		event ItemPurchasedDeleg OnItemPurchased;

		// Token: 0x060012FF RID: 4863
		void SyncProfileItemsWithCatalog(ProfileProxy profile);

		// Token: 0x06001300 RID: 4864
		PurchasedResult PurchaseOffer(UserInfo.User user, int supplierId, long offerHash, ulong offerId, IPurchaseListener listener);

		// Token: 0x06001301 RID: 4865
		PurchasedResult PurchaseOffers(UserInfo.User user, int supplierId, long offerHash, IEnumerable<ulong> offerIds, IPurchaseListener listener);

		// Token: 0x06001302 RID: 4866
		void AddMissingOffer(ulong userId, ulong profileId, StoreOffer offer, CustomerItem purchasedItem, IPurchaseListener listener, ILogGroup logGroup);
	}
}
