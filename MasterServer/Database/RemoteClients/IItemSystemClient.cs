using System;
using System.Collections.Generic;
using MasterServer.DAL;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x02000201 RID: 513
	internal interface IItemSystemClient
	{
		// Token: 0x06000A9E RID: 2718
		IEnumerable<SItem> GetAllItems();

		// Token: 0x06000A9F RID: 2719
		void AddDefaultProfileItem(ulong id, ulong itemId, ulong slotIds, string config);

		// Token: 0x06000AA0 RID: 2720
		void AddItem(SItem item);

		// Token: 0x06000AA1 RID: 2721
		void UpdateItem(SItem item);

		// Token: 0x06000AA2 RID: 2722
		void ActivateItem(ulong itemId, bool active);

		// Token: 0x06000AA3 RID: 2723
		IEnumerable<SEquipItem> GetProfileItems(ulong profileId);

		// Token: 0x06000AA4 RID: 2724
		IEnumerable<SEquipItem> GetDefaultProfileItems();

		// Token: 0x06000AA5 RID: 2725
		IEnumerable<ulong> GetUnlockedItems(ulong profileId);

		// Token: 0x06000AA6 RID: 2726
		ulong AddPurchasedItem(ulong profileId, ulong itemId, ulong catalogId);

		// Token: 0x06000AA7 RID: 2727
		void UpdateProfileItem(ulong profileId, ulong profileItemId, ulong slotIds, ulong attachedTo, string config);

		// Token: 0x06000AA8 RID: 2728
		void RepairProfileItem(ulong profileId, ulong profileItemId);

		// Token: 0x06000AA9 RID: 2729
		void ExpireProfileItem(ulong profileId, ulong profileItemId);

		// Token: 0x06000AAA RID: 2730
		void ExpireProfileItemConfirmed(ulong profileId, ulong profileItemId);

		// Token: 0x06000AAB RID: 2731
		void UnlockItem(ulong profileId, ulong itemId);

		// Token: 0x06000AAC RID: 2732
		void LockItem(ulong profileId, ulong itemId);

		// Token: 0x06000AAD RID: 2733
		ulong GiveItem(ulong profileId, ulong itemId, EProfileItemStatus status);

		// Token: 0x06000AAE RID: 2734
		void DeleteDefaultItems(ulong profileId);

		// Token: 0x06000AAF RID: 2735
		void AddDefaultItem(ulong profileId, ulong itemId, ulong slotIds, ulong attached_to, string config);

		// Token: 0x06000AB0 RID: 2736
		void ClearDefaultProfileItems();

		// Token: 0x06000AB1 RID: 2737
		void DebugUnlockAllItems(ulong profileId);

		// Token: 0x06000AB2 RID: 2738
		void DebugResetProfileItems(ulong profileId);

		// Token: 0x06000AB3 RID: 2739
		void DeleteProfileItem(ulong profileId, ulong profileItemId);

		// Token: 0x06000AB4 RID: 2740
		ulong ExtendProfileItem(ulong profileId, ulong profileItemId);
	}
}
