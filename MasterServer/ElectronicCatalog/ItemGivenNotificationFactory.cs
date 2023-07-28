using System;
using System.Xml;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.SpecialProfileRewards.Actions;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x02000235 RID: 565
	internal class ItemGivenNotificationFactory
	{
		// Token: 0x06000C19 RID: 3097 RVA: 0x0002E580 File Offset: 0x0002C980
		public static SNotification CreateItemGivenNotification(GiveItemResponse givenItem, TimeSpan notificationTTL, string customMessage = "", bool notify = true, XmlElement userData = null)
		{
			SItemRewardNotification sitemRewardNotification = new SItemRewardNotification
			{
				user_data = userData,
				name = givenItem.ItemGiven.Item.Name,
				offer_type = givenItem.ItemOfferType,
				extended_time = ((uint)givenItem.ItemGiven.ExpirationTime.TotalHours).ToString(),
				consumables_count = givenItem.ItemGiven.Quantity,
				notify = notify
			};
			notificationTTL = ((!(notificationTTL != TimeSpan.Zero)) ? NotificationService.DefaultNotificationTTL : notificationTTL);
			return NotificationFactory.CreateNotification<string>(ENotificationType.ItemGiven, sitemRewardNotification.ToXml().OuterXml, notificationTTL, EConfirmationType.Confirmation, customMessage);
		}
	}
}
