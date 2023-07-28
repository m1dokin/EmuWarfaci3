using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200005C RID: 92
	[OrphanService]
	[Singleton]
	internal class ItemMaxAmountComposer : ServiceModule
	{
		// Token: 0x06000167 RID: 359 RVA: 0x00009F24 File Offset: 0x00008324
		public ItemMaxAmountComposer(IItemCache itemCache, ICatalogService catalogService)
		{
			this.m_itemCache = itemCache;
			this.m_catalogService = catalogService;
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00009F3A File Offset: 0x0000833A
		public override void Init()
		{
			base.Init();
			this.m_itemCache.ItemsCacheUpdated += this.OnItemsCacheUpdated;
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00009F59 File Offset: 0x00008359
		public override void Stop()
		{
			this.m_itemCache.ItemsCacheUpdated -= this.OnItemsCacheUpdated;
			base.Stop();
		}

		// Token: 0x0600016A RID: 362 RVA: 0x00009F78 File Offset: 0x00008378
		private void OnItemsCacheUpdated(IEnumerable<SItem> items)
		{
			Dictionary<string, CatalogItem> catalogItems = this.m_catalogService.GetCatalogItems();
			foreach (SItem sitem in items)
			{
				CatalogItem catalogItem;
				if (catalogItems.TryGetValue(sitem.Name, out catalogItem))
				{
					sitem.MaxAmount = catalogItem.MaxAmount;
				}
			}
		}

		// Token: 0x040000A7 RID: 167
		private readonly IItemCache m_itemCache;

		// Token: 0x040000A8 RID: 168
		private readonly ICatalogService m_catalogService;
	}
}
