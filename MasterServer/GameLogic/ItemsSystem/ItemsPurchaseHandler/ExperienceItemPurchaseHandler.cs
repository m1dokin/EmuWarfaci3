using System;
using HK2Net;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.RewardSystem;

namespace MasterServer.GameLogic.ItemsSystem.ItemsPurchaseHandler
{
	// Token: 0x02000317 RID: 791
	[Service]
	[Singleton]
	internal class ExperienceItemPurchaseHandler : IItemsPurchaseHandler
	{
		// Token: 0x06001214 RID: 4628 RVA: 0x00047DFB File Offset: 0x000461FB
		public ExperienceItemPurchaseHandler(ICatalogService catalogService, IRankSystem rankSystem, IDALService dalService)
		{
			this.m_catalogService = catalogService;
			this.m_rankSystem = rankSystem;
			this.m_dalService = dalService;
		}

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x06001215 RID: 4629 RVA: 0x00047E18 File Offset: 0x00046218
		public string ItemType
		{
			get
			{
				return "exp";
			}
		}

		// Token: 0x06001216 RID: 4630 RVA: 0x00047E20 File Offset: 0x00046220
		public void HandleItemPurchase(ItemPurchaseContext ctx)
		{
			ctx.PurchasedItem.Quantity = (ulong)((ushort)this.m_rankSystem.AddExperience(ctx.ProfileID, ctx.PurchasedItem.Quantity, LevelChangeReason.ExperienceItem, ctx.LogGroup));
			this.m_catalogService.DeleteCustomerItem(ctx.UserID, ctx.PurchasedItem.InstanceID);
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(ctx.ProfileID);
			ctx.Listener.HandleExperience(ctx.Item, ctx.PurchasedItem.Quantity, profileInfo.RankInfo, ctx.Offer);
		}

		// Token: 0x04000842 RID: 2114
		private readonly IDALService m_dalService;

		// Token: 0x04000843 RID: 2115
		private readonly ICatalogService m_catalogService;

		// Token: 0x04000844 RID: 2116
		private readonly IRankSystem m_rankSystem;
	}
}
