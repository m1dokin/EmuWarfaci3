using System;
using System.Linq;
using HK2Net;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog;
using Util.Common;

namespace MasterServer.GameLogic.ItemsSystem.ItemsPurchaseHandler
{
	// Token: 0x02000323 RID: 803
	[Service]
	[Singleton]
	internal class ProfileItemPurchaseHandler : IItemsPurchaseHandler
	{
		// Token: 0x06001234 RID: 4660 RVA: 0x000482AE File Offset: 0x000466AE
		public ProfileItemPurchaseHandler(ICatalogService catalogService, IItemStats itemStats, IProfileItems profileItems, IItemsValidator itemsValidator)
		{
			this.m_catalogService = catalogService;
			this.m_itemStats = itemStats;
			this.m_profileItems = profileItems;
			this.m_itemsValidator = itemsValidator;
		}

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x06001235 RID: 4661 RVA: 0x000482D3 File Offset: 0x000466D3
		public string ItemType
		{
			get
			{
				return string.Empty;
			}
		}

		// Token: 0x06001236 RID: 4662 RVA: 0x000482DC File Offset: 0x000466DC
		public void HandleItemPurchase(ItemPurchaseContext ctx)
		{
			SProfileItem sprofileItem = this.m_profileItems.GetProfileItems(ctx.ProfileID, EquipOptions.ActiveOnly).Values.FirstOrDefault((SProfileItem i) => i.CatalogID == ctx.PurchasedItem.InstanceID);
			ulong num;
			if (sprofileItem == null || ctx.PurchasedItem.DurabilityPoints > 0)
			{
				num = this.m_profileItems.AddPurchasedItem(ctx.ProfileID, ctx.Item.ID, ctx.PurchasedItem.InstanceID);
				this.m_itemsValidator.FixAndUpdateSlotIds(ctx.ProfileID, num, 0UL);
			}
			else
			{
				StackableItemStats stackableItemStats = this.m_itemStats.GetStackableItemStats(sprofileItem.ItemID);
				if (stackableItemStats != null && stackableItemStats.IsStackable)
				{
					if (sprofileItem.IsExpired && ctx.PurchasedItem.Quantity > 0UL)
					{
						this.m_profileItems.RepairProfileItem(ctx.ProfileID, sprofileItem.ProfileItemID);
					}
				}
				else
				{
					this.m_profileItems.ExtendProfileItem(ctx.ProfileID, sprofileItem.ProfileItemID);
				}
				num = sprofileItem.ProfileItemID;
			}
			if (ctx.ProduceType == LogGroup.ProduceType.RandomBox || ctx.ProduceType == LogGroup.ProduceType.Bundle)
			{
				this.ContainerItemShopOfferBoughtLog(ctx);
			}
			SPurchasedItem item = new SPurchasedItem
			{
				Item = ctx.Item,
				ProfileItemID = num,
				AddedExpiration = TimeUtils.GetExpireTime(TimeUtils.UTCTimestampToTimeSpan(ctx.PurchasedItem.ExpirationTimeUTC)),
				AddedQuantity = ctx.PurchasedItem.AddedQuantity,
				Status = ctx.PurchasedItem.Status
			};
			ctx.Listener.HandleProfileItem(item, ctx.Offer);
		}

		// Token: 0x06001237 RID: 4663 RVA: 0x000484E8 File Offset: 0x000468E8
		private void ContainerItemShopOfferBoughtLog(ItemPurchaseContext ctx)
		{
			bool flag = ctx.Offer != null;
			ctx.LogGroup.ShopOfferBoughtLog(ctx.UserID, ctx.ProfileID, string.Empty, 0, string.Empty, TransactionStatus.OK, 0L, 0L, 0L, string.Empty, 0UL, 0UL, 0UL, (!flag) ? "N/A" : ctx.Offer.Status, 0U, (!flag) ? 0UL : ctx.Offer.StoreID, (!flag) ? OfferType.Regular : ctx.Offer.Type, ctx.PurchasedItem.CatalogItem.ID, ctx.PurchasedItem.CatalogItem.Name, ctx.PurchasedItem.DurabilityPoints, TimeUtils.GetExpireTime(TimeUtils.UTCTimestampToTimeSpan(ctx.PurchasedItem.ExpirationTimeUTC)), ctx.PurchasedItem.CatalogItem.Type, (ctx.PurchasedItem.AddedQuantity <= 0UL) ? 1UL : ctx.PurchasedItem.AddedQuantity, ctx.PurchasedItem.AddedQuantity, ctx.ProduceType, 0UL, "-");
		}

		// Token: 0x04000857 RID: 2135
		private readonly ICatalogService m_catalogService;

		// Token: 0x04000858 RID: 2136
		private readonly IItemStats m_itemStats;

		// Token: 0x04000859 RID: 2137
		private readonly IProfileItems m_profileItems;

		// Token: 0x0400085A RID: 2138
		private readonly IItemsValidator m_itemsValidator;
	}
}
