using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog;
using Util.Common;

namespace MasterServer.Core.ServerDataLog
{
	// Token: 0x0200012E RID: 302
	[Service]
	[Singleton]
	internal class OffersDataLogExtension : AbstractServerDataLogExtension
	{
		// Token: 0x060004F7 RID: 1271 RVA: 0x000154BC File Offset: 0x000138BC
		public OffersDataLogExtension(ICatalogService catalogService, IShopService shopService, ILogService logService, bool isEnabled) : base(logService, isEnabled)
		{
			this.m_catalogService = catalogService;
			this.m_shopService = shopService;
		}

		// Token: 0x060004F8 RID: 1272 RVA: 0x000154D5 File Offset: 0x000138D5
		public override void Start()
		{
			base.Start();
			this.m_shopService.OffersUpdated += this.OnOffersUpdated;
		}

		// Token: 0x060004F9 RID: 1273 RVA: 0x000154F4 File Offset: 0x000138F4
		public override void Dispose()
		{
			this.m_shopService.OffersUpdated -= this.OnOffersUpdated;
		}

		// Token: 0x060004FA RID: 1274 RVA: 0x00015510 File Offset: 0x00013910
		protected override void LogData()
		{
			using (ILogGroup logGroup = this.LogService.CreateGroup())
			{
				foreach (StoreOffer storeOffer in this.m_catalogService.GetStoreOffers())
				{
					logGroup.CatalogOfferLog(storeOffer.SupplierID, storeOffer.StoreID, storeOffer.Content.Item.Name, storeOffer.Type, storeOffer.GetPriceByCurrency(Currency.GameMoney), storeOffer.GetPriceByCurrency(Currency.CryMoney), storeOffer.GetPriceByCurrency(Currency.CrownMoney), storeOffer.GetPriceTagByCurrency(Currency.KeyMoney).KeyCatalogName, storeOffer.Status, storeOffer.Discount, storeOffer.GetOriginalPriceTag(Currency.GameMoney).Price, storeOffer.GetOriginalPriceTag(Currency.CryMoney).Price, storeOffer.GetOriginalPriceTag(Currency.CrownMoney).Price, storeOffer.Content.DurabilityPoints, TimeUtils.GetExpireTime(storeOffer.Content.ExpirationTime), storeOffer.Content.Quantity, storeOffer.Content.RepairCost, storeOffer.Rank);
				}
			}
		}

		// Token: 0x060004FB RID: 1275 RVA: 0x00015650 File Offset: 0x00013A50
		private void OnOffersUpdated(IEnumerable<StoreOffer> offers)
		{
			base.OnDataUpdated();
		}

		// Token: 0x04000210 RID: 528
		private readonly ICatalogService m_catalogService;

		// Token: 0x04000211 RID: 529
		private IShopService m_shopService;
	}
}
