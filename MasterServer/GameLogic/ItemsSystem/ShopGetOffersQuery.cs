using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.ElectronicCatalog;
using MasterServer.ElectronicCatalog.ShopSupplier.Serializers;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000384 RID: 900
	[QueryAttributes(TagName = "shop_get_offers")]
	internal class ShopGetOffersQuery : PagedQueryStatic
	{
		// Token: 0x0600144D RID: 5197 RVA: 0x000529F8 File Offset: 0x00050DF8
		public ShopGetOffersQuery(IItemCache itemCache, ICatalogService catalogService, IShopService shopService, IDALService dalService)
		{
			this.m_itemCache = itemCache;
			this.m_catalogService = catalogService;
			this.m_shopService = shopService;
			this.m_dalService = dalService;
		}

		// Token: 0x0600144E RID: 5198 RVA: 0x00052A1D File Offset: 0x00050E1D
		protected override int GetMaxBatch()
		{
			return 250;
		}

		// Token: 0x0600144F RID: 5199 RVA: 0x00052A24 File Offset: 0x00050E24
		protected override string GetDataHash()
		{
			return this.m_shopService.OffersHash.ToString();
		}

		// Token: 0x06001450 RID: 5200 RVA: 0x00052A4C File Offset: 0x00050E4C
		protected override List<XmlElement> GetData(XmlDocument doc)
		{
			List<XmlElement> list = new List<XmlElement>();
			IOrderedEnumerable<StoreOffer> orderedEnumerable = from x in this.m_catalogService.GetStoreOffers()
			orderby x.StoreID
			select x;
			Dictionary<string, SItem> allItemsByName = this.m_itemCache.GetAllItemsByName();
			StoreOfferXmlSerializer storeOfferXmlSerializer = new StoreOfferXmlSerializer(this.m_dalService, doc);
			foreach (StoreOffer storeOffer in orderedEnumerable)
			{
				SItem sitem;
				if (allItemsByName.TryGetValue(storeOffer.Content.Item.Name, out sitem) && sitem.ShopContent)
				{
					XmlElement item = storeOfferXmlSerializer.Serialize(storeOffer);
					list.Add(item);
				}
			}
			return list;
		}

		// Token: 0x04000964 RID: 2404
		private const int OFFERS_MAX_BATCH = 250;

		// Token: 0x04000965 RID: 2405
		private readonly IItemCache m_itemCache;

		// Token: 0x04000966 RID: 2406
		private readonly ICatalogService m_catalogService;

		// Token: 0x04000967 RID: 2407
		private readonly IShopService m_shopService;

		// Token: 0x04000968 RID: 2408
		private readonly IDALService m_dalService;
	}
}
