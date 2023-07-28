using System;
using HK2Net;

namespace MasterServer.GameLogic.ItemsSystem.ItemsPurchaseHandler
{
	// Token: 0x02000319 RID: 793
	[Contract]
	internal interface IItemsPurchaseHandler
	{
		// Token: 0x1700019D RID: 413
		// (get) Token: 0x06001217 RID: 4631
		string ItemType { get; }

		// Token: 0x06001218 RID: 4632
		void HandleItemPurchase(ItemPurchaseContext ctx);
	}
}
