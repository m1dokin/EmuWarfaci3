using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.DAL;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200032E RID: 814
	[Service]
	[Singleton]
	internal class KeyDataStatsValidator : IItemStatsValidator
	{
		// Token: 0x06001256 RID: 4694 RVA: 0x00049514 File Offset: 0x00047914
		public KeyDataStatsValidator(IItemCache itemCache)
		{
			this.m_itemCache = itemCache;
		}

		// Token: 0x06001257 RID: 4695 RVA: 0x00049524 File Offset: 0x00047924
		public void Validate(IEnumerable<StoreOffer> offers)
		{
			Dictionary<string, StoreOffer> dictionary = new Dictionary<string, StoreOffer>();
			foreach (StoreOffer storeOffer in offers)
			{
				if (storeOffer.IsKeyPriceOffer())
				{
					string keyCatalogName = storeOffer.GetPriceTagByCurrency(Currency.KeyMoney).KeyCatalogName;
					SItem sitem;
					if (!this.m_itemCache.TryGetItem(keyCatalogName, out sitem))
					{
						throw new ItemStatsValidationException(string.Format("Offer {0} has non-active key {1}", storeOffer.StoreID, keyCatalogName));
					}
					StoreOffer storeOffer2;
					if (dictionary.TryGetValue(keyCatalogName, out storeOffer2))
					{
						throw new ItemStatsValidationException(string.Format("Offers {0} and {1} has same key item {2}", storeOffer2.StoreID, storeOffer.StoreID, keyCatalogName));
					}
					dictionary.Add(keyCatalogName, storeOffer);
				}
			}
		}

		// Token: 0x04000872 RID: 2162
		private readonly IItemCache m_itemCache;
	}
}
