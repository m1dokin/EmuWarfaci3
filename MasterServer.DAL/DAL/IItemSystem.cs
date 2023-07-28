using System;

namespace MasterServer.DAL
{
	// Token: 0x02000071 RID: 113
	public interface IItemSystem
	{
		// Token: 0x06000123 RID: 291
		DALResultMulti<SItem> GetAllItems();

		// Token: 0x06000124 RID: 292
		DALResultVoid AddItem(SItem item);

		// Token: 0x06000125 RID: 293
		DALResultVoid UpdateItem(SItem item);

		// Token: 0x06000126 RID: 294
		DALResultVoid ActivateItem(ulong item_id, bool active);

		// Token: 0x06000127 RID: 295
		DALResultMulti<SEquipItem> GetProfileItems(ulong profile_id);

		// Token: 0x06000128 RID: 296
		DALResultMulti<SEquipItem> GetDefaultProfileItems();

		// Token: 0x06000129 RID: 297
		DALResultMulti<ulong> GetUnlockedItems(ulong profile_id);

		// Token: 0x0600012A RID: 298
		DALResult<ulong> AddPurchasedItem(ulong profile_id, ulong item_id, ulong catalog_id);

		// Token: 0x0600012B RID: 299
		DALResultVoid UpdateProfileItem(ulong profile_id, ulong profile_item_id, ulong slot_ids, ulong attached_to, string config);

		// Token: 0x0600012C RID: 300
		DALResultVoid RepairProfileItem(ulong profile_id, ulong profile_item_id);

		// Token: 0x0600012D RID: 301
		DALResultVoid ExpireProfileItem(ulong profile_id, ulong profile_item_id);

		// Token: 0x0600012E RID: 302
		DALResultVoid ExpireProfileItemConfirmed(ulong profile_id, ulong profile_item_id);

		// Token: 0x0600012F RID: 303
		DALResultVoid UnlockItem(ulong profileId, ulong itemId);

		// Token: 0x06000130 RID: 304
		DALResultVoid LockItem(ulong profileId, ulong itemId);

		// Token: 0x06000131 RID: 305
		DALResult<ulong> GiveItem(ulong profileId, ulong itemId, EProfileItemStatus status);

		// Token: 0x06000132 RID: 306
		DALResultVoid DeleteDefaultItems(ulong profileId);

		// Token: 0x06000133 RID: 307
		DALResultVoid AddDefaultItem(ulong profile_id, ulong item_id, ulong slot_ids, ulong attached_to, string config);

		// Token: 0x06000134 RID: 308
		DALResultVoid ClearDefaultProfileItems();

		// Token: 0x06000135 RID: 309
		DALResultVoid AddDefaultProfileItem(ulong id, ulong itemId, ulong slotIds, string config);

		// Token: 0x06000136 RID: 310
		DALResultVoid DebugUnlockAllItems(ulong profileId);

		// Token: 0x06000137 RID: 311
		DALResultVoid DebugResetProfileItems(ulong profileId);

		// Token: 0x06000138 RID: 312
		DALResultVoid DeleteProfileItem(ulong profile_id, ulong profile_item_id);

		// Token: 0x06000139 RID: 313
		DALResult<ulong> ExtendProfileItem(ulong profileId, ulong profile_item_id);
	}
}
