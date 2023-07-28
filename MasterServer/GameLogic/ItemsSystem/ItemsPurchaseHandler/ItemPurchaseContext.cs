using System;
using MasterServer.Core;
using MasterServer.DAL;

namespace MasterServer.GameLogic.ItemsSystem.ItemsPurchaseHandler
{
	// Token: 0x02000318 RID: 792
	internal struct ItemPurchaseContext
	{
		// Token: 0x04000845 RID: 2117
		public ulong UserID;

		// Token: 0x04000846 RID: 2118
		public ulong ProfileID;

		// Token: 0x04000847 RID: 2119
		public CustomerItem PurchasedItem;

		// Token: 0x04000848 RID: 2120
		public SItem Item;

		// Token: 0x04000849 RID: 2121
		public StoreOffer Offer;

		// Token: 0x0400084A RID: 2122
		public IPurchaseListener Listener;

		// Token: 0x0400084B RID: 2123
		public ILogGroup LogGroup;

		// Token: 0x0400084C RID: 2124
		public LogGroup.ProduceType ProduceType;
	}
}
