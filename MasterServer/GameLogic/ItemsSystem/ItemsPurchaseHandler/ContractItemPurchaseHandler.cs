using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.ContractSystem;
using Util.Common;

namespace MasterServer.GameLogic.ItemsSystem.ItemsPurchaseHandler
{
	// Token: 0x02000316 RID: 790
	[Service]
	[Singleton]
	internal class ContractItemPurchaseHandler : IItemsPurchaseHandler
	{
		// Token: 0x06001210 RID: 4624 RVA: 0x00047B50 File Offset: 0x00045F50
		public ContractItemPurchaseHandler(ICatalogService catalogService, IProfileItems profileSystem, IDALService dalService, IContractService contractService)
		{
			this.m_catalogService = catalogService;
			this.m_dalService = dalService;
			this.m_profileItems = profileSystem;
			this.m_contractService = contractService;
		}

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x06001211 RID: 4625 RVA: 0x00047B75 File Offset: 0x00045F75
		public string ItemType
		{
			get
			{
				return "contract";
			}
		}

		// Token: 0x06001212 RID: 4626 RVA: 0x00047B7C File Offset: 0x00045F7C
		public void HandleItemPurchase(ItemPurchaseContext ctx)
		{
			foreach (KeyValuePair<ulong, SProfileItem> keyValuePair in this.m_profileItems.GetProfileItems(ctx.ProfileID, EquipOptions.All, (SProfileItem i) => i.GameItem.Type == "contract"))
			{
				Log.Verbose("Contract item purchase deletes old contracts {0} for profile {1}", new object[]
				{
					keyValuePair.Value.GameItem.Name,
					ctx.ProfileID
				});
				this.m_catalogService.DeleteCustomerItem(ctx.UserID, keyValuePair.Value.CatalogID);
				this.m_profileItems.DeleteProfileItem(ctx.ProfileID, keyValuePair.Value.ProfileItemID);
				SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(ctx.ProfileID);
				ctx.LogGroup.ItemDestroyLog(ctx.UserID, ctx.ProfileID, profileInfo.Nickname, profileInfo.RankInfo.RankId, ctx.Item.ID, ctx.PurchasedItem.InstanceID, ctx.Item.Type, ctx.Item.Name, 0, string.Empty);
			}
			ulong num = this.m_profileItems.AddPurchasedItem(ctx.ProfileID, ctx.Item.ID, ctx.PurchasedItem.InstanceID);
			this.m_contractService.ActivateContract(ctx.ProfileID, num, ctx.Item.Name);
			ctx.LogGroup.ContractActivateLog(ctx.UserID, ctx.ProfileID, TimeUtils.GetExpireTime(TimeUtils.UTCTimestampToTimeSpan(ctx.PurchasedItem.ExpirationTimeUTC)), ctx.Item.Name);
			SPurchasedItem item = new SPurchasedItem
			{
				Item = ctx.Item,
				ProfileItemID = num,
				AddedExpiration = TimeUtils.GetExpireTime(TimeUtils.UTCTimestampToTimeSpan(ctx.PurchasedItem.ExpirationTimeUTC)),
				Status = ctx.PurchasedItem.Status
			};
			ctx.Listener.HandleProfileItem(item, ctx.Offer);
		}

		// Token: 0x0400083D RID: 2109
		private readonly IProfileItems m_profileItems;

		// Token: 0x0400083E RID: 2110
		private readonly IDALService m_dalService;

		// Token: 0x0400083F RID: 2111
		private readonly ICatalogService m_catalogService;

		// Token: 0x04000840 RID: 2112
		private readonly IContractService m_contractService;
	}
}
