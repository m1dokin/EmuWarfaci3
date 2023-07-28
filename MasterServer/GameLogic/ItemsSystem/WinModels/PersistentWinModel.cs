using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog;
using MasterServer.Users;

namespace MasterServer.GameLogic.ItemsSystem.WinModels
{
	// Token: 0x02000332 RID: 818
	[Service]
	[Singleton]
	internal class PersistentWinModel : WinModelBase
	{
		// Token: 0x0600126B RID: 4715 RVA: 0x00049F91 File Offset: 0x00048391
		public PersistentWinModel(ICatalogService catalogService, IUserRepository userRepository, ILogService logService, IProfileItems profileItems) : base(userRepository, logService)
		{
			this.m_catalogService = catalogService;
			this.m_profileItems = profileItems;
		}

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x0600126C RID: 4716 RVA: 0x00049FAA File Offset: 0x000483AA
		public override TopPrizeWinModel WinModel
		{
			get
			{
				return TopPrizeWinModel.Persistent;
			}
		}

		// Token: 0x0600126D RID: 4717 RVA: 0x00049FB0 File Offset: 0x000483B0
		public override int AddPrizeToken(ulong userId, string tokenName)
		{
			CatalogItem item;
			if (!this.m_catalogService.TryGetCatalogItem(tokenName, out item))
			{
				Log.Warning<string>("Catalog item with name '{0}' wasn't found", tokenName);
				return 0;
			}
			OfferItem item2 = new OfferItem
			{
				Item = item,
				ExpirationTime = TimeSpan.Zero,
				Quantity = 1UL
			};
			AddCustomerItemResponse addCustomerItemResponse = this.m_catalogService.AddCustomerItem(userId, item2, false);
			if (addCustomerItemResponse.Status != TransactionStatus.OK)
			{
				Log.Warning<string, TransactionStatus>("Failed to add item '{0}' as consumable. Transaction status: {1}", tokenName, addCustomerItemResponse.Status);
				return 0;
			}
			CustomerItem customerItem = this.m_catalogService.GetCustomerItems(userId).Values.FirstOrDefault((CustomerItem x) => x.CatalogItem.Name == tokenName);
			return (customerItem == null) ? 0 : ((int)customerItem.Quantity);
		}

		// Token: 0x0600126E RID: 4718 RVA: 0x0004A08C File Offset: 0x0004848C
		public override void ResetPrizeTokensCount(ulong userId, ulong profileId, string tokenName)
		{
			CustomerItem customerItem = this.m_catalogService.GetCustomerItems(userId).Values.FirstOrDefault((CustomerItem x) => x.CatalogItem.Name == tokenName);
			if (customerItem == null)
			{
				Log.Warning<string>("Customer item named '{0}' intended to be reset wasn't found", tokenName);
				return;
			}
			base.LogTopPrizeTokensResetForUser(userId, tokenName, customerItem.Quantity);
			this.m_catalogService.DeleteCustomerItem(userId, customerItem.InstanceID);
			Dictionary<ulong, SProfileItem> profileItems = this.m_profileItems.GetProfileItems(profileId, EquipOptions.ActiveOnly, (SProfileItem item) => item.CatalogID == customerItem.InstanceID);
			if (profileItems.Count == 0)
			{
				Log.Warning<string, ulong>("Profile item for customer '{0}' in profile '{1}' wasn't found", tokenName, profileId);
				return;
			}
			this.m_profileItems.DeleteProfileItem(profileId, profileItems.First<KeyValuePair<ulong, SProfileItem>>().Value.ProfileItemID);
		}

		// Token: 0x0600126F RID: 4719 RVA: 0x0004A170 File Offset: 0x00048570
		public override Dictionary<string, ulong> GetCollectedPrizeTokensCount(ulong userId)
		{
			return (from x in this.m_catalogService.GetCustomerItems(userId).Values
			where x.CatalogItem.Type == "top_prize_token"
			select x).ToDictionary((CustomerItem el) => el.CatalogItem.Name, (CustomerItem el) => el.Quantity);
		}

		// Token: 0x04000881 RID: 2177
		private const string TopPrizeTokenItemType = "top_prize_token";

		// Token: 0x04000882 RID: 2178
		private readonly ICatalogService m_catalogService;

		// Token: 0x04000883 RID: 2179
		private readonly IProfileItems m_profileItems;
	}
}
