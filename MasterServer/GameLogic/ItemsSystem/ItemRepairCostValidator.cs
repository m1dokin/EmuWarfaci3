using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.DAL;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200006C RID: 108
	[Service]
	[Singleton]
	internal class ItemRepairCostValidator : IItemStatsValidator
	{
		// Token: 0x060001A4 RID: 420 RVA: 0x0000AD3F File Offset: 0x0000913F
		public ItemRepairCostValidator(IItemCache itemCache)
		{
			this.m_itemCache = itemCache;
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x0000AD50 File Offset: 0x00009150
		public void Validate(IEnumerable<StoreOffer> offers)
		{
			Dictionary<ulong, SItem> allItems = this.m_itemCache.GetAllItems();
			using (Dictionary<ulong, SItem>.Enumerator enumerator = allItems.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<ulong, SItem> item = enumerator.Current;
					IEnumerable<StoreOffer> enumerable = from offer in offers
					where offer.Content.Item.Name == item.Value.Name && offer.Type == OfferType.Permanent
					select offer;
					StoreOffer storeOffer = enumerable.FirstOrDefault<StoreOffer>();
					foreach (StoreOffer storeOffer2 in enumerable)
					{
						if (storeOffer2.Content.RepairCost != storeOffer.Content.RepairCost || storeOffer2.Content.DurabilityPoints != storeOffer.Content.DurabilityPoints)
						{
							string format = "Item {0} has offers with different repair information, {1} : {2} - {3} and {4} : {5} - {6} (offerId : repait_cost - durability)";
							throw new ItemStatsValidationException(string.Format(format, new object[]
							{
								item.Value.Name,
								storeOffer.StoreID,
								storeOffer.Content.RepairCost,
								storeOffer.Content.DurabilityPoints,
								storeOffer2.StoreID,
								storeOffer2.Content.RepairCost,
								storeOffer2.Content.DurabilityPoints
							}));
						}
					}
				}
			}
		}

		// Token: 0x040000C4 RID: 196
		private readonly IItemCache m_itemCache;
	}
}
