using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Common;
using MasterServer.DAL;

namespace MasterServer.GameLogic.ItemsSystem.ItemStatsValidator
{
	// Token: 0x02000324 RID: 804
	[Service]
	[Singleton]
	internal class BundleStatsValidator : IItemStatsValidator
	{
		// Token: 0x06001238 RID: 4664 RVA: 0x0004863F File Offset: 0x00046A3F
		public BundleStatsValidator(IItemCache itemCache, IItemStats itemStats, IItemRepairDescriptionRepository repairRepository)
		{
			this.m_itemCache = itemCache;
			this.m_itemStats = itemStats;
			this.m_repairRepository = repairRepository;
		}

		// Token: 0x06001239 RID: 4665 RVA: 0x0004865C File Offset: 0x00046A5C
		public void Validate(IEnumerable<StoreOffer> offers)
		{
			foreach (BundleDesc bundle in this.m_itemStats.GetBundlesDesc())
			{
				this.CheckBundleData(bundle, offers);
			}
		}

		// Token: 0x0600123A RID: 4666 RVA: 0x000486BC File Offset: 0x00046ABC
		public void CheckBundleData(BundleDesc bundle, IEnumerable<StoreOffer> storeOffers)
		{
			StoreOffer storeOffer = storeOffers.FirstOrDefault((StoreOffer o) => o.Content.Item.Name == bundle.Name);
			if (storeOffer == null)
			{
				return;
			}
			var source = (from bundledItem in bundle.Items
			join item in this.m_itemCache.GetAllItemsByName(false) on bundledItem.Name equals item.Key into items
			select new
			{
				ItemName = bundledItem.Name,
				Item = ((!items.Any<KeyValuePair<string, SItem>>()) ? null : items.First<KeyValuePair<string, SItem>>().Value)
			}).Distinct().ToList();
			if ((from i in source
			where i.Item != null
			select i).Any(i => i.Item.Type.IsOneOf(new string[]
			{
				"bundle",
				"random_box"
			})))
			{
				throw new BundleValidationException(string.Format("Bundle {0} contains random box/bundle item(s) which is unacceptable", bundle.Name));
			}
			var source2 = (from bundledItem in source
			where !this.m_itemCache.GetAllItemsByName(true).ContainsKey(bundledItem.ItemName)
			select bundledItem).ToList();
			string[] array = (from item in source2
			where item.Item == null
			select item.ItemName).ToArray<string>();
			if (array.Any<string>())
			{
				throw new BundleValidationException(string.Format("Bundle {0} contains non-existent item(s) {1}", bundle.Name, string.Join(",", array)));
			}
			SItem sitem;
			if (!this.m_itemCache.TryGetItem(bundle.Name, out sitem))
			{
				throw new BundleValidationException(string.Format("Bundle {0} wasn't found in the item cache", bundle.Name));
			}
			if (source2.Any())
			{
				throw new BundleValidationException(string.Format("Bundle {0} contains non-active item(s) {1}", bundle.Name, string.Join(",", (from item in source2
				select item.ItemName).ToArray<string>())));
			}
			StackableItemStats stackableItemStats = this.m_itemStats.GetStackableItemStats(sitem.ID);
			if (stackableItemStats.MaxBuyAmount > 1)
			{
				throw new BundleValidationException(string.Format("Bundle {0} has max buy amount ({1}) > 1", bundle.Name, stackableItemStats.MaxBuyAmount));
			}
			string repairCost = storeOffer.Content.RepairCost;
			foreach (BundleDesc.BundledItem bundledItem2 in bundle.Items)
			{
				string name = bundledItem2.Name;
				SItem sitem2;
				this.m_itemCache.TryGetItem(name, out sitem2);
				if (CommonValidationMethods.ValidateContainedItemAmount(bundle.Name, bundledItem2) != ValidationFlowAdvice.Finish)
				{
					if (CommonValidationMethods.ValidateNonPermanentContainedItem(bundle.Name, bundledItem2) != ValidationFlowAdvice.Finish)
					{
						if (CommonValidationMethods.ValidateRegularItem(bundle.Name, bundledItem2, this.m_itemCache, this.m_repairRepository, storeOffers) != ValidationFlowAdvice.Finish)
						{
							CommonValidationMethods.ValidateContainedItemRepairDescription(bundle, name, repairCost, this.m_itemCache, this.m_repairRepository, storeOffers);
						}
					}
				}
			}
		}

		// Token: 0x0400085B RID: 2139
		private readonly IItemCache m_itemCache;

		// Token: 0x0400085C RID: 2140
		private readonly IItemStats m_itemStats;

		// Token: 0x0400085D RID: 2141
		private readonly IItemRepairDescriptionRepository m_repairRepository;
	}
}
