using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.DAL;
using MasterServer.GameLogic.ItemsSystem;

namespace MasterServer.Core.ServerDataLog
{
	// Token: 0x0200012D RID: 301
	[Service]
	[Singleton]
	internal class ItemsDataLogExtension : AbstractServerDataLogExtension
	{
		// Token: 0x060004F2 RID: 1266 RVA: 0x00015388 File Offset: 0x00013788
		public ItemsDataLogExtension(IItemCache itemCache, IItemStats itemStats, ILogService logService, bool isEnabled) : base(logService, isEnabled)
		{
			this.m_itemCache = itemCache;
			this.m_itemStats = itemStats;
		}

		// Token: 0x060004F3 RID: 1267 RVA: 0x000153A1 File Offset: 0x000137A1
		public override void Start()
		{
			base.Start();
			this.m_itemCache.ItemsCacheUpdated += this.OnItemsCacheUpdated;
		}

		// Token: 0x060004F4 RID: 1268 RVA: 0x000153C0 File Offset: 0x000137C0
		public override void Dispose()
		{
			this.m_itemCache.ItemsCacheUpdated -= this.OnItemsCacheUpdated;
		}

		// Token: 0x060004F5 RID: 1269 RVA: 0x000153DC File Offset: 0x000137DC
		protected override void LogData()
		{
			using (ILogGroup logGroup = this.LogService.CreateGroup())
			{
				foreach (KeyValuePair<ulong, SItem> keyValuePair in this.m_itemCache.GetAllItems())
				{
					SItem value = keyValuePair.Value;
					TaggedItemDesc taggedItemDesc = this.m_itemStats.GetTaggedItemDesc(value.ID);
					string tagsFilter = (taggedItemDesc == null) ? string.Empty : taggedItemDesc.Filter.ToString();
					logGroup.ItemCacheLog(value.ID, value.Name, value.Type, tagsFilter);
				}
			}
		}

		// Token: 0x060004F6 RID: 1270 RVA: 0x000154B4 File Offset: 0x000138B4
		private void OnItemsCacheUpdated(IEnumerable<SItem> sItems)
		{
			base.OnDataUpdated();
		}

		// Token: 0x0400020E RID: 526
		private readonly IItemCache m_itemCache;

		// Token: 0x0400020F RID: 527
		private readonly IItemStats m_itemStats;
	}
}
