using System;
using System.Collections.Generic;
using HK2Net;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000344 RID: 836
	[Contract]
	public interface IItemBoxService
	{
		// Token: 0x060012B4 RID: 4788
		IEnumerable<RandomBoxDesc.Choice> OpenRandomBox(ulong userId, ulong profileId, RandomBoxDesc desc);

		// Token: 0x060012B5 RID: 4789
		IEnumerable<BundleDesc.BundledItem> OpenBundle(ulong profileId, ulong itemId);
	}
}
