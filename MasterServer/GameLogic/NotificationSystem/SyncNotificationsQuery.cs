using System;
using System.Collections.Generic;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.Users;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x02000582 RID: 1410
	[QueryAttributes(TagName = "sync_notifications")]
	internal class SyncNotificationsQuery : BaseQuery
	{
		// Token: 0x06001E52 RID: 7762 RVA: 0x0007B119 File Offset: 0x00079519
		public SyncNotificationsQuery(INotificationService notificationService)
		{
			this.m_notificationService = notificationService;
		}

		// Token: 0x06001E53 RID: 7763 RVA: 0x0007B128 File Offset: 0x00079528
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			IEnumerable<SNotification> enumerable = queryParams[0] as IEnumerable<SNotification>;
			foreach (SNotification snotification in enumerable)
			{
				request.AppendChild(snotification.ToXml(this.m_notificationService, request.OwnerDocument));
			}
		}

		// Token: 0x06001E54 RID: 7764 RVA: 0x0007B198 File Offset: 0x00079598
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			UserInfo.User user;
			if (!base.GetClientInfo(fromJid, out user))
			{
				return -3;
			}
			ProfileProxy profile = new ProfileProxy(user);
			ProfileReader profileReader = new ProfileReader(profile);
			profileReader.ReadPendingNotifications(response);
			return 0;
		}

		// Token: 0x04000EC6 RID: 3782
		public const string Name = "sync_notifications";

		// Token: 0x04000EC7 RID: 3783
		private readonly INotificationService m_notificationService;
	}
}
