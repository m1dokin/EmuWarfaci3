using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.DAL;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200077F RID: 1919
	[Contract]
	internal interface IItemsValidator
	{
		// Token: 0x060027BC RID: 10172
		bool CheckProfileItems(Dictionary<ulong, SProfileItem> profile, ulong profileID);

		// Token: 0x060027BD RID: 10173
		bool CheckProfileItems(ulong profileID);

		// Token: 0x170003B1 RID: 945
		// (get) Token: 0x060027BE RID: 10174
		// (set) Token: 0x060027BF RID: 10175
		int ValidationVerbose { get; set; }

		// Token: 0x060027C0 RID: 10176
		bool FixSlotIds(ulong profileID, ulong itemId, ref ulong slotIds);

		// Token: 0x060027C1 RID: 10177
		void FixAndUpdateSlotIds(ulong profileID, ulong itemId, ulong attachedTo);

		// Token: 0x060027C2 RID: 10178
		ulong GetSlotIds(SItem item);

		// Token: 0x060027C3 RID: 10179
		IEnumerable<SlotInfo> GetSlotsInfo(ulong slotIds);

		// Token: 0x060027C4 RID: 10180
		SlotInfo GetSlotInfoPerClass(ulong slot_ids, int classIndex);

		// Token: 0x170003B2 RID: 946
		// (get) Token: 0x060027C5 RID: 10181
		IEnumerable<SlotInfo> DefaultInventory { get; }
	}
}
