using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.DAL;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000371 RID: 881
	[Contract]
	public interface IProfileItems
	{
		// Token: 0x060013DB RID: 5083
		Dictionary<ulong, SProfileItem> GetProfileItems(ulong profileID);

		// Token: 0x060013DC RID: 5084
		Dictionary<ulong, SProfileItem> GetProfileItems(ulong profileID, EquipOptions options);

		// Token: 0x060013DD RID: 5085
		Dictionary<ulong, SProfileItem> GetProfileItems(ulong profileID, EquipOptions options, Predicate<SProfileItem> pred);

		// Token: 0x060013DE RID: 5086
		SProfileItem GetProfileItem(ulong profileID, ulong inventoryID);

		// Token: 0x060013DF RID: 5087
		SProfileItem GetProfileItem(ulong profileID, ulong inventoryID, EquipOptions options);

		// Token: 0x060013E0 RID: 5088
		Dictionary<ulong, SProfileItem> GetProfileDefaultItems(ulong profileID);

		// Token: 0x060013E1 RID: 5089
		Dictionary<ulong, SItem> GetUnlockedItems(ulong profileID);

		// Token: 0x060013E2 RID: 5090
		Dictionary<ulong, SProfileItem> GetExpiredProfileItems(ulong profileID);

		// Token: 0x060013E3 RID: 5091
		Dictionary<ulong, SProfileItem> GetExpiredByDateProfileItems(ulong profileID);

		// Token: 0x060013E4 RID: 5092
		Dictionary<ulong, SProfileItem> GetExpiredByDateProfileItems(ulong profileId, EquipOptions options);

		// Token: 0x060013E5 RID: 5093
		Dictionary<ulong, SProfileItem> GetExpiredByDurabilityProfileItems(ulong profileID);

		// Token: 0x060013E6 RID: 5094
		Dictionary<ulong, SProfileItem> GetExpiredByDurabilityProfileItems(ulong profileId, EquipOptions options);

		// Token: 0x060013E7 RID: 5095
		void AddDefaultItems(ulong profileID);

		// Token: 0x060013E8 RID: 5096
		void DeleteDefaultItems(ulong profileID);

		// Token: 0x060013E9 RID: 5097
		ulong AddPurchasedItem(ulong profileID, ulong item_id, ulong catalog_id);

		// Token: 0x060013EA RID: 5098
		void UpdateProfileItem(ulong profileID, ulong profileItemID, ulong slotIds, ulong attachedTo, string config);

		// Token: 0x060013EB RID: 5099
		void RepairProfileItem(ulong profileID, ulong profile_item_id);

		// Token: 0x060013EC RID: 5100
		void ExpireProfileItem(ulong profileID, ulong profile_item_id);

		// Token: 0x060013ED RID: 5101
		void ExpireProfileItemConfirmed(ulong profileID, ulong profile_item_id);

		// Token: 0x060013EE RID: 5102
		ulong GiveItem(ulong profileID, ulong itemId, EProfileItemStatus status);

		// Token: 0x060013EF RID: 5103
		void DeleteProfileItem(ulong profileID, ulong profile_item_id);

		// Token: 0x060013F0 RID: 5104
		ulong ExtendProfileItem(ulong profileID, ulong profile_item_id);

		// Token: 0x060013F1 RID: 5105
		void UnlockItem(ulong profileID, ulong itemId);

		// Token: 0x060013F2 RID: 5106
		void LockItem(ulong profileID, ulong itemId);

		// Token: 0x060013F3 RID: 5107
		SProfileItem BuildProfileItem(SEquipItem equipItem);

		// Token: 0x060013F4 RID: 5108
		SProfileItem BuildProfileItem(SEquipItem equipItem, CustomerItem customerItem);

		// Token: 0x060013F5 RID: 5109
		void RegisterProfileItemsComposer(IProfileItemsComposer composer);

		// Token: 0x060013F6 RID: 5110
		void RegisterProfileItemsComposer(IProfileItemsComposer composer, int priority);

		// Token: 0x060013F7 RID: 5111
		void UnregisterProfileItemsComposer(IProfileItemsComposer composer);
	}
}
