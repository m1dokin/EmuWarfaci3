using System;
using System.Collections.Generic;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.Users;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000374 RID: 884
	[ProfileItemsComposer(Priority = 10)]
	internal class BoughtItemsComposer : IProfileItemsComposer
	{
		// Token: 0x06001423 RID: 5155 RVA: 0x00051AEB File Offset: 0x0004FEEB
		public BoughtItemsComposer(IDALService dalService, IItemStats itemStats, IItemCache itemCache, IUserRepository userRepository, INotificationService notificationService)
		{
			this.m_dalService = dalService;
			this.m_itemStats = itemStats;
			this.m_itemCache = itemCache;
			this.m_userRepository = userRepository;
			this.m_notificationService = notificationService;
		}

		// Token: 0x06001424 RID: 5156 RVA: 0x00051B18 File Offset: 0x0004FF18
		public void Compose(ulong profileId, EquipOptions options, List<SEquipItem> composedEquip)
		{
			UserInfo.User user = this.m_userRepository.GetUser(profileId);
			foreach (SEquipItem sequipItem in this.m_dalService.ItemSystem.GetProfileItems(profileId))
			{
				bool flag = true;
				if (!options.HasFlag(EquipOptions.All))
				{
					flag = this.m_itemStats.IsItemAvailableForUser(sequipItem.ItemID, user);
				}
				if (!flag && sequipItem.SlotIDs != 0UL)
				{
					this.m_dalService.ItemSystem.UpdateProfileItem(profileId, sequipItem.ProfileItemID, 0UL, 0UL, sequipItem.Config);
					SItemUnequippedNotification data = new SItemUnequippedNotification
					{
						ItemName = this.m_itemCache.GetAllItems()[sequipItem.ItemID].Name
					};
					this.m_notificationService.AddNotification<SItemUnequippedNotification>(profileId, ENotificationType.ItemUnequipped, data, TimeSpan.FromMinutes(30.0), EDeliveryType.SendOnCheckPoint, EConfirmationType.None);
				}
				if (!options.HasFlag(EquipOptions.FilterByTags) || flag)
				{
					composedEquip.Add(sequipItem);
				}
			}
		}

		// Token: 0x0400094A RID: 2378
		private readonly IDALService m_dalService;

		// Token: 0x0400094B RID: 2379
		private readonly IItemStats m_itemStats;

		// Token: 0x0400094C RID: 2380
		private readonly IItemCache m_itemCache;

		// Token: 0x0400094D RID: 2381
		private readonly IUserRepository m_userRepository;

		// Token: 0x0400094E RID: 2382
		private readonly INotificationService m_notificationService;
	}
}
