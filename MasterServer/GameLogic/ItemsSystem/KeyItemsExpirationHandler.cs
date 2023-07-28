using System;
using HK2Net;
using MasterServer.Core.Services;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.Users;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x020000B1 RID: 177
	[OrphanService]
	[Singleton]
	internal class KeyItemsExpirationHandler : ServiceModule
	{
		// Token: 0x060002D5 RID: 725 RVA: 0x0000DD90 File Offset: 0x0000C190
		public KeyItemsExpirationHandler(IItemService itemService, IItemsExpiration itemsExpiration, IUserProxyRepository userProxyRepository, INotificationService notificationService)
		{
			this.m_itemService = itemService;
			this.m_userProxyRepository = userProxyRepository;
			this.m_notificationService = notificationService;
			this.m_itemsExpiration = itemsExpiration;
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x0000DDB5 File Offset: 0x0000C1B5
		public override void Init()
		{
			base.Init();
			this.m_itemsExpiration.OnItemExpired += this.OnItemExpired;
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x0000DDD4 File Offset: 0x0000C1D4
		public override void Stop()
		{
			this.m_itemsExpiration.OnItemExpired -= this.OnItemExpired;
			base.Stop();
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x0000DDF3 File Offset: 0x0000C1F3
		private void OnItemExpired(ulong userID, SProfileItem item)
		{
			if (item.GameItem.Type == "key")
			{
				this.HandleItemExpired(userID, item);
			}
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x0000DE18 File Offset: 0x0000C218
		private void HandleItemExpired(ulong userID, SProfileItem item)
		{
			UserInfo.User userOrProxyByUserId = this.m_userProxyRepository.GetUserOrProxyByUserId(userID);
			this.m_itemService.DeleteItem(userOrProxyByUserId, item.ProfileItemID);
			SNotification snotification = NotificationFactory.CreateNotification<ItemDeletedNotification>(ENotificationType.ItemDeleted, new ItemDeletedNotification(item.ProfileItemID), TimeSpan.FromHours(1.0), EConfirmationType.None);
			this.m_notificationService.AddNotifications(userOrProxyByUserId.ProfileID, new SNotification[]
			{
				snotification
			}, EDeliveryType.SendNow);
		}

		// Token: 0x04000134 RID: 308
		private const string ItemType = "key";

		// Token: 0x04000135 RID: 309
		private readonly IItemService m_itemService;

		// Token: 0x04000136 RID: 310
		private readonly IItemsExpiration m_itemsExpiration;

		// Token: 0x04000137 RID: 311
		private readonly IUserProxyRepository m_userProxyRepository;

		// Token: 0x04000138 RID: 312
		private readonly INotificationService m_notificationService;
	}
}
