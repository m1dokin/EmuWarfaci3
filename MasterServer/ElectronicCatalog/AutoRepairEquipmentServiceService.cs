using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.PersistentSettingsSystem;
using MasterServer.Users;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x02000227 RID: 551
	[OrphanService]
	[Singleton]
	internal class AutoRepairEquipmentServiceService : ServiceModule
	{
		// Token: 0x06000BD2 RID: 3026 RVA: 0x0002CFBA File Offset: 0x0002B3BA
		public AutoRepairEquipmentServiceService(IItemService itemService, INotificationService notificationSystem, IPersistentSettingsService persistentSettingsService, ILogService logService, IItemsExpiration itemsExpiration)
		{
			this.m_itemService = itemService;
			this.m_notificationSystem = notificationSystem;
			this.m_persistentSettingsService = persistentSettingsService;
			this.m_logService = logService;
			this.m_itemsExpiration = itemsExpiration;
		}

		// Token: 0x06000BD3 RID: 3027 RVA: 0x0002CFE8 File Offset: 0x0002B3E8
		public override void Init()
		{
			base.Init();
			this.m_configAutoRepair = Resources.ModuleSettings.GetSection("AutoRepairEquipment");
			this.m_autoRepairEnabled = (int.Parse(this.m_configAutoRepair.Get("enabled")) > 0);
			this.m_configAutoRepair.OnConfigChanged += this.OnConfigChanged;
			this.m_itemsExpiration.OnGotBrokenPermanentItems += this.RepairAll;
		}

		// Token: 0x06000BD4 RID: 3028 RVA: 0x0002D05C File Offset: 0x0002B45C
		public override void Stop()
		{
			this.m_configAutoRepair.OnConfigChanged -= this.OnConfigChanged;
			this.m_itemsExpiration.OnGotBrokenPermanentItems -= this.RepairAll;
			base.Stop();
		}

		// Token: 0x06000BD5 RID: 3029 RVA: 0x0002D094 File Offset: 0x0002B494
		private void RepairAll(UserInfo.User user, string sessionId, IList<SProfileItem> expiredProfileItems)
		{
			if (!this.m_autoRepairEnabled)
			{
				return;
			}
			PersistentSettings profileSettings = this.m_persistentSettingsService.GetProfileSettings(user.ProfileID);
			int num;
			if (!int.TryParse(profileSettings.GetValue("options.gameplay.auto_repair"), out num) || num == 0)
			{
				return;
			}
			RepairEquipmentOperationResult repairEquipmentOperationResult = this.m_itemService.RepairMultipleItems(user, expiredProfileItems);
			this.m_notificationSystem.AddNotification<RepairEquipmentOperationResult>(repairEquipmentOperationResult.ProfileId, ENotificationType.AutoRepairEquipment, repairEquipmentOperationResult, TimeSpan.FromDays(1.0), EDeliveryType.SendNow, EConfirmationType.None);
			ulong gameMoneyAfterSession = repairEquipmentOperationResult.MoneyBeforeRepair;
			if (repairEquipmentOperationResult.OperationGlobalStatus == RepairStatus.Ok)
			{
				gameMoneyAfterSession = repairEquipmentOperationResult.MoneyBeforeRepair - repairEquipmentOperationResult.TotalRepairCost;
			}
			this.m_logService.Event.AutoRepairEquipmentResult(repairEquipmentOperationResult.ProfileId, user.UserID, sessionId, DateTime.UtcNow, repairEquipmentOperationResult.MoneyBeforeRepair, gameMoneyAfterSession, true, repairEquipmentOperationResult.OperationGlobalStatus, repairEquipmentOperationResult.TotalRepairCost);
		}

		// Token: 0x06000BD6 RID: 3030 RVA: 0x0002D16A File Offset: 0x0002B56A
		private void OnConfigChanged(ConfigEventArgs args)
		{
			if (string.Equals(args.Name, "enabled", StringComparison.InvariantCulture))
			{
				this.m_autoRepairEnabled = (args.iValue > 0);
			}
		}

		// Token: 0x0400057D RID: 1405
		private ConfigSection m_configAutoRepair;

		// Token: 0x0400057E RID: 1406
		private bool m_autoRepairEnabled;

		// Token: 0x0400057F RID: 1407
		private readonly IItemService m_itemService;

		// Token: 0x04000580 RID: 1408
		private readonly INotificationService m_notificationSystem;

		// Token: 0x04000581 RID: 1409
		private readonly IPersistentSettingsService m_persistentSettingsService;

		// Token: 0x04000582 RID: 1410
		private readonly ILogService m_logService;

		// Token: 0x04000583 RID: 1411
		private readonly IItemsExpiration m_itemsExpiration;
	}
}
