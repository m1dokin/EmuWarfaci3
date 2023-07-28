using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using HK2Net;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x0200057A RID: 1402
	[Contract]
	public interface INotificationService
	{
		// Token: 0x06001E1D RID: 7709
		IEnumerable<SNotification> GetPendingByType(ulong profileId, ENotificationType type);

		// Token: 0x06001E1E RID: 7710
		IEnumerable<SNotification> PopPending(ulong profileId);

		// Token: 0x06001E1F RID: 7711
		void DeletePendingByType(ulong profileId, ENotificationType type);

		// Token: 0x06001E20 RID: 7712
		Task AddNotification<T>(ulong profileId, ENotificationType type, T data, TimeSpan expiration, EDeliveryType delivery, EConfirmationType confirmation);

		// Token: 0x06001E21 RID: 7713
		Task AddNotifications<T>(ulong profileId, ENotificationType type, IEnumerable<T> data, TimeSpan expiration, EDeliveryType delivery, EConfirmationType confirmation);

		// Token: 0x06001E22 RID: 7714
		Task AddNotifications(ulong profileId, IEnumerable<SNotification> notifications, EDeliveryType delivery);

		// Token: 0x06001E23 RID: 7715
		void AddBroadcastNotifications<T>(List<string> receivers, ENotificationType type, IEnumerable<T> data, TimeSpan expiration, EDeliveryType delivery, EConfirmationType confirmation);

		// Token: 0x06001E24 RID: 7716
		void Confirm(ulong profileId, ulong notificationId, XmlNode confirmationNode);

		// Token: 0x06001E25 RID: 7717
		INotificationSerializer GetNotificationSerializer(ENotificationType notificationType);

		// Token: 0x1400006E RID: 110
		// (add) Token: 0x06001E26 RID: 7718
		// (remove) Token: 0x06001E27 RID: 7719
		event NotificationConfirmed OnNotificationConfirmed;

		// Token: 0x1400006F RID: 111
		// (add) Token: 0x06001E28 RID: 7720
		// (remove) Token: 0x06001E29 RID: 7721
		event NotificationExpired OnNotificationExpired;
	}
}
