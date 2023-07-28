using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000343 RID: 835
	[QueryAttributes(TagName = "items")]
	internal class GetServerItemsQuery : PagedQueryStatic
	{
		// Token: 0x060012AF RID: 4783 RVA: 0x0004B430 File Offset: 0x00049830
		public GetServerItemsQuery(IItemCache itemCache)
		{
			this.m_itemCache = itemCache;
		}

		// Token: 0x060012B0 RID: 4784 RVA: 0x0004B43F File Offset: 0x0004983F
		protected override int GetMaxBatch()
		{
			return 250;
		}

		// Token: 0x060012B1 RID: 4785 RVA: 0x0004B448 File Offset: 0x00049848
		protected override string GetDataHash()
		{
			return this.m_itemCache.GetItemsHash().ToString();
		}

		// Token: 0x060012B2 RID: 4786 RVA: 0x0004B470 File Offset: 0x00049870
		protected override List<XmlElement> GetData(XmlDocument doc)
		{
			IOrderedEnumerable<SItem> source = from i in this.m_itemCache.GetAllItems().Values
			orderby i.ID
			select i;
			return (from cacheItem in source
			select ServerItem.GetXml(cacheItem, doc, "item")).ToList<XmlElement>();
		}

		// Token: 0x040008A2 RID: 2210
		private const int ITEMS_MAX_BATCH = 250;

		// Token: 0x040008A3 RID: 2211
		private readonly IItemCache m_itemCache;
	}
}
