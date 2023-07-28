using System;
using MasterServer.DAL;
using MasterServer.GameLogic.NotificationSystem;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200034E RID: 846
	internal interface IPurchaseListener
	{
		// Token: 0x060012EE RID: 4846
		void HandleProfileItem(SPurchasedItem item, StoreOffer offer);

		// Token: 0x060012EF RID: 4847
		void HandleExperience(SItem item, ulong added, SRankInfo total, StoreOffer offer);

		// Token: 0x060012F0 RID: 4848
		void HandleMoney(SItem item, Currency currency, ulong added, ulong total, StoreOffer offer);

		// Token: 0x060012F1 RID: 4849
		void HandleMetaGameItem(SPurchasedItem item, StoreOffer offer);

		// Token: 0x060012F2 RID: 4850
		SNotification CreateNotification(OfferItem givenItem, string message = "", bool notify = true);
	}
}
