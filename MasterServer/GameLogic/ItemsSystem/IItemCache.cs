using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.DAL;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000348 RID: 840
	[Contract]
	public interface IItemCache
	{
		// Token: 0x14000037 RID: 55
		// (add) Token: 0x060012C5 RID: 4805
		// (remove) Token: 0x060012C6 RID: 4806
		event Action<IEnumerable<SItem>> ItemsCacheUpdated;

		// Token: 0x060012C7 RID: 4807
		Dictionary<ulong, SItem> GetAllItems();

		// Token: 0x060012C8 RID: 4808
		Dictionary<string, SItem> GetAllItemsByName();

		// Token: 0x060012C9 RID: 4809
		Dictionary<ulong, SItem> GetAllItems(bool activeOnly);

		// Token: 0x060012CA RID: 4810
		Dictionary<string, SItem> GetAllItemsByName(bool activeOnly);

		// Token: 0x060012CB RID: 4811
		bool TryGetItem(string name, out SItem item);

		// Token: 0x060012CC RID: 4812
		bool TryGetItem(string name, bool activeOnly, out SItem item);

		// Token: 0x060012CD RID: 4813
		Dictionary<ulong, SEquipItem> GetDefaultProfileItems();

		// Token: 0x060012CE RID: 4814
		int GetItemsHash();
	}
}
