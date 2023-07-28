using System;
using System.Collections.Generic;
using HK2Net;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200035A RID: 858
	[Contract]
	public interface IItemsReimbursement
	{
		// Token: 0x1400003F RID: 63
		// (add) Token: 0x06001343 RID: 4931
		// (remove) Token: 0x06001344 RID: 4932
		event ReimbursementItemsUpdatedDelegate ReimbursementItemsUpdated;

		// Token: 0x06001345 RID: 4933
		List<string> ProcessReimbursement(ulong user_id, ulong profile_id);

		// Token: 0x06001346 RID: 4934
		List<ItemToReimburse> GetItemsToReimburse();
	}
}
