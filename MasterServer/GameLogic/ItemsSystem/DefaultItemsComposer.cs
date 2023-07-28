using System;
using System.Collections.Generic;
using MasterServer.DAL;
using MasterServer.Users;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000375 RID: 885
	[ProfileItemsComposer(Priority = 80)]
	internal class DefaultItemsComposer : IProfileItemsComposer
	{
		// Token: 0x06001425 RID: 5157 RVA: 0x00051C60 File Offset: 0x00050060
		public DefaultItemsComposer(IItemsValidator itemsValidator, IItemCache itemCacheService, IUserRepository userRepository)
		{
			this.m_itemsValidator = itemsValidator;
			this.m_itemCacheService = itemCacheService;
			this.m_userRepository = userRepository;
		}

		// Token: 0x06001426 RID: 5158 RVA: 0x00051C80 File Offset: 0x00050080
		public void Compose(ulong profileId, EquipOptions options, List<SEquipItem> composedEquip)
		{
			Dictionary<ulong, bool> dictionary = new Dictionary<ulong, bool>();
			Dictionary<ulong, SItem> allItems = this.m_itemCacheService.GetAllItems(false);
			foreach (SlotInfo slotInfo in this.m_itemsValidator.DefaultInventory)
			{
				dictionary[slotInfo.id] = false;
			}
			foreach (SEquipItem sequipItem in composedEquip)
			{
				foreach (SlotInfo slotInfo2 in this.m_itemsValidator.DefaultInventory)
				{
					SlotInfo slotInfoPerClass = this.m_itemsValidator.GetSlotInfoPerClass(sequipItem.SlotIDs, slotInfo2.classIndex);
					if (slotInfoPerClass.id == slotInfo2.id && !sequipItem.IsExpired && (!options.HasFlag(EquipOptions.ActiveOnly) || allItems[sequipItem.ItemID].Active))
					{
						dictionary[slotInfoPerClass.id] = true;
					}
				}
			}
			using (Dictionary<ulong, SEquipItem>.ValueCollection.Enumerator enumerator4 = this.m_itemCacheService.GetDefaultProfileItems().Values.GetEnumerator())
			{
				while (enumerator4.MoveNext())
				{
					SEquipItem defItem = enumerator4.Current;
					if (composedEquip.Find((SEquipItem A) => A.ItemID == defItem.ItemID) == null)
					{
						ulong num = 0UL;
						bool flag = false;
						foreach (SlotInfo slotInfo3 in this.m_itemsValidator.DefaultInventory)
						{
							if (slotInfo3.minCount <= 1 && slotInfo3.maxCount == 1)
							{
								SlotInfo slotInfoPerClass2 = this.m_itemsValidator.GetSlotInfoPerClass(defItem.SlotIDs, slotInfo3.classIndex);
								if (slotInfoPerClass2.id == slotInfo3.id)
								{
									flag |= (slotInfoPerClass2.minCount <= 1 && slotInfoPerClass2.maxCount == 1);
									if (!dictionary[slotInfoPerClass2.id])
									{
										dictionary[slotInfoPerClass2.id] = true;
										num |= slotInfoPerClass2.id;
									}
								}
							}
						}
						if (flag)
						{
							SEquipItem sequipItem2 = (SEquipItem)defItem.Clone();
							sequipItem2.SlotIDs = num;
							sequipItem2.ProfileItemID = ulong.MaxValue - defItem.ItemID;
							composedEquip.Add(sequipItem2);
						}
					}
				}
			}
		}

		// Token: 0x0400094F RID: 2383
		private readonly IItemsValidator m_itemsValidator;

		// Token: 0x04000950 RID: 2384
		private readonly IItemCache m_itemCacheService;

		// Token: 0x04000951 RID: 2385
		private readonly IUserRepository m_userRepository;
	}
}
