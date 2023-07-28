using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.ItemsSystem.ItemsPurchaseHandler.MetaGameActions;

namespace MasterServer.GameLogic.ItemsSystem.ItemsPurchaseHandler
{
	// Token: 0x0200031A RID: 794
	[Service]
	[Singleton]
	internal class MetaGameItemPurchaseHandler : IItemsPurchaseHandler
	{
		// Token: 0x06001219 RID: 4633 RVA: 0x00047EC4 File Offset: 0x000462C4
		public MetaGameItemPurchaseHandler(ICatalogService catalogService, IItemStats itemStats, IEnumerable<IMetaGameAction> actions)
		{
			this.m_catalogService = catalogService;
			this.m_itemStats = itemStats;
			this.m_actions = actions;
		}

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x0600121A RID: 4634 RVA: 0x00047EE1 File Offset: 0x000462E1
		public string ItemType
		{
			get
			{
				return "meta_game";
			}
		}

		// Token: 0x0600121B RID: 4635 RVA: 0x00047EE8 File Offset: 0x000462E8
		public void HandleItemPurchase(ItemPurchaseContext ctx)
		{
			MetaGameDesc metaGameDesc = this.m_itemStats.GetMetaGameDesc(ctx.Item.ID);
			if (metaGameDesc == null)
			{
				Log.Warning<string>("Found meta game item {0} without description", ctx.Item.Name);
				return;
			}
			foreach (IMetaGameAction metaGameAction in this.m_actions)
			{
				foreach (string action in metaGameDesc.Get(metaGameAction.Name))
				{
					metaGameAction.Execute(ctx.ProfileID, action);
				}
			}
			this.m_catalogService.DeleteCustomerItem(ctx.UserID, ctx.PurchasedItem.InstanceID);
			SPurchasedItem item = new SPurchasedItem
			{
				Item = ctx.Item
			};
			ctx.Listener.HandleMetaGameItem(item, ctx.Offer);
		}

		// Token: 0x0400084D RID: 2125
		private readonly ICatalogService m_catalogService;

		// Token: 0x0400084E RID: 2126
		private readonly IItemStats m_itemStats;

		// Token: 0x0400084F RID: 2127
		private readonly IEnumerable<IMetaGameAction> m_actions;
	}
}
