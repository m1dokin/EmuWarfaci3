using System;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.RewardSystem;

namespace MasterServer.GameLogic.ItemsSystem.ItemsPurchaseHandler
{
	// Token: 0x0200031F RID: 799
	internal abstract class MoneyItemPurchaseHandler : IItemsPurchaseHandler
	{
		// Token: 0x06001227 RID: 4647 RVA: 0x000481B0 File Offset: 0x000465B0
		public MoneyItemPurchaseHandler(ICatalogService catalogService, IRewardService rewardService)
		{
			this.m_catalogService = catalogService;
			this.m_rewardService = rewardService;
		}

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x06001228 RID: 4648
		public abstract string ItemType { get; }

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x06001229 RID: 4649
		protected abstract Currency ItemCurrency { get; }

		// Token: 0x0600122A RID: 4650 RVA: 0x000481C8 File Offset: 0x000465C8
		public void HandleItemPurchase(ItemPurchaseContext ctx)
		{
			this.m_rewardService.RewardMoney(ctx.UserID, ctx.ProfileID, this.ItemCurrency, ctx.PurchasedItem.Quantity, ctx.LogGroup, LogGroup.ProduceType.Buy);
			this.m_catalogService.DeleteCustomerItem(ctx.UserID, ctx.PurchasedItem.InstanceID);
			CustomerAccount customerAccount = this.m_catalogService.GetCustomerAccount(ctx.UserID, this.ItemCurrency);
			ctx.Listener.HandleMoney(ctx.Item, this.ItemCurrency, ctx.PurchasedItem.Quantity, customerAccount.Money, ctx.Offer);
		}

		// Token: 0x04000855 RID: 2133
		private readonly ICatalogService m_catalogService;

		// Token: 0x04000856 RID: 2134
		private readonly IRewardService m_rewardService;
	}
}
