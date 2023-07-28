using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.DAL;
using MasterServer.GameLogic.ItemsSystem;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000007 RID: 7
	[Service]
	[Singleton]
	internal class ProfileHeadProvider : IProfileHeadProvider
	{
		// Token: 0x06000012 RID: 18 RVA: 0x000045FA File Offset: 0x000029FA
		public ProfileHeadProvider(IItemCache itemCache)
		{
			this.m_itemCache = itemCache;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x0000460C File Offset: 0x00002A0C
		public string GetHead()
		{
			Dictionary<string, SItem> allItemsByName = this.m_itemCache.GetAllItemsByName();
			KeyValuePair<string, SItem> keyValuePair = allItemsByName.FirstOrDefault((KeyValuePair<string, SItem> item) => item.Value.Slots.Contains("defaulthead"));
			return (keyValuePair.Value == null) ? string.Empty : keyValuePair.Value.Name;
		}

		// Token: 0x04000006 RID: 6
		private readonly IItemCache m_itemCache;
	}
}
