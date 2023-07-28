using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.DAL;
using MasterServer.GameLogic.ItemsSystem;

namespace MasterServer.Core.ServerDataLog
{
	// Token: 0x0200012C RID: 300
	[Service]
	[Singleton]
	internal class DefaultProfileLogExtension : AbstractServerDataLogExtension
	{
		// Token: 0x060004ED RID: 1261 RVA: 0x00015284 File Offset: 0x00013684
		public DefaultProfileLogExtension(IItemCache itemCache, ILogService logService, bool isEnabled) : base(logService, isEnabled)
		{
			this.m_itemCache = itemCache;
		}

		// Token: 0x060004EE RID: 1262 RVA: 0x00015295 File Offset: 0x00013695
		public override void Start()
		{
			base.Start();
			this.m_itemCache.ItemsCacheUpdated += this.OnItemsCacheUpdated;
		}

		// Token: 0x060004EF RID: 1263 RVA: 0x000152B4 File Offset: 0x000136B4
		public override void Dispose()
		{
			this.m_itemCache.ItemsCacheUpdated -= this.OnItemsCacheUpdated;
		}

		// Token: 0x060004F0 RID: 1264 RVA: 0x000152D0 File Offset: 0x000136D0
		protected override void LogData()
		{
			using (ILogGroup logGroup = this.LogService.CreateGroup())
			{
				foreach (KeyValuePair<ulong, SEquipItem> keyValuePair in this.m_itemCache.GetDefaultProfileItems())
				{
					logGroup.DefaultProfileLog(keyValuePair.Value.ItemID, keyValuePair.Value.CatalogID, keyValuePair.Value.SlotIDs);
				}
			}
		}

		// Token: 0x060004F1 RID: 1265 RVA: 0x00015380 File Offset: 0x00013780
		private void OnItemsCacheUpdated(IEnumerable<SItem> sItems)
		{
			base.OnDataUpdated();
		}

		// Token: 0x0400020D RID: 525
		private readonly IItemCache m_itemCache;
	}
}
