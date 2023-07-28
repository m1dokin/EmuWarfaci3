using System;
using MasterServer.DAL;
using MasterServer.GameLogic.NotificationSystem;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200034F RID: 847
	internal class NullPurchaseListener : IPurchaseListener
	{
		// Token: 0x060012F4 RID: 4852 RVA: 0x0004C4E8 File Offset: 0x0004A8E8
		public void HandleProfileItem(SPurchasedItem item, StoreOffer offer)
		{
		}

		// Token: 0x060012F5 RID: 4853 RVA: 0x0004C4EA File Offset: 0x0004A8EA
		public void HandleExperience(SItem item, ulong added, SRankInfo total, StoreOffer offer)
		{
		}

		// Token: 0x060012F6 RID: 4854 RVA: 0x0004C4EC File Offset: 0x0004A8EC
		public void HandleMoney(SItem item, Currency currency, ulong added, ulong total, StoreOffer offer)
		{
		}

		// Token: 0x060012F7 RID: 4855 RVA: 0x0004C4EE File Offset: 0x0004A8EE
		public void HandleMetaGameItem(SPurchasedItem item, StoreOffer offer)
		{
		}

		// Token: 0x060012F8 RID: 4856 RVA: 0x0004C4F0 File Offset: 0x0004A8F0
		public SNotification CreateNotification(OfferItem givenItem, string message = "", bool notify = true)
		{
			return new SNotification();
		}
	}
}
