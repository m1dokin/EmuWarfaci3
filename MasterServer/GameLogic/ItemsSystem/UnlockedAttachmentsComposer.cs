using System;
using System.Collections.Generic;
using MasterServer.DAL;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200037A RID: 890
	[ProfileItemsComposer(Priority = 90)]
	internal class UnlockedAttachmentsComposer : IProfileItemsComposer
	{
		// Token: 0x0600142F RID: 5167 RVA: 0x000520FA File Offset: 0x000504FA
		public UnlockedAttachmentsComposer(IProfileItems profileItems, IItemsValidator itemsValidator)
		{
			this.m_profileItems = profileItems;
			this.m_itemsValidator = itemsValidator;
		}

		// Token: 0x06001430 RID: 5168 RVA: 0x00052110 File Offset: 0x00050510
		public void Compose(ulong profileId, EquipOptions options, List<SEquipItem> composedEquip)
		{
			foreach (SItem sitem in this.m_profileItems.GetUnlockedItems(profileId).Values)
			{
				if (sitem.IsAttachmentItem)
				{
					ulong slotIds = this.m_itemsValidator.GetSlotIds(sitem);
					SEquipItem item = new SEquipItem
					{
						ProfileID = profileId,
						ItemID = sitem.ID,
						SlotIDs = slotIds,
						Status = EProfileItemStatus.REWARD,
						ProfileItemID = ulong.MaxValue - sitem.ID,
						Config = string.Empty
					};
					composedEquip.Add(item);
				}
			}
		}

		// Token: 0x04000956 RID: 2390
		private readonly IProfileItems m_profileItems;

		// Token: 0x04000957 RID: 2391
		private readonly IItemsValidator m_itemsValidator;
	}
}
