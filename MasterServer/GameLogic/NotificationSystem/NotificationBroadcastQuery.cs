using System;
using System.Collections.Generic;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x02000584 RID: 1412
	[QueryAttributes(TagName = "notification_broadcast")]
	internal class NotificationBroadcastQuery : BaseQuery
	{
		// Token: 0x06001E57 RID: 7767 RVA: 0x0007B288 File Offset: 0x00079688
		public NotificationBroadcastQuery(INotificationService notificationService)
		{
			this.m_notificationService = notificationService;
		}

		// Token: 0x06001E58 RID: 7768 RVA: 0x0007B298 File Offset: 0x00079698
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			string value = (string)queryParams[0];
			IEnumerable<SNotification> enumerable = queryParams[1] as IEnumerable<SNotification>;
			request.SetAttribute("bcast_receivers", value);
			foreach (SNotification snotification in enumerable)
			{
				request.AppendChild(snotification.ToXml(this.m_notificationService, request.OwnerDocument));
			}
		}

		// Token: 0x04000EC8 RID: 3784
		public const string Name = "notification_broadcast";

		// Token: 0x04000EC9 RID: 3785
		private readonly INotificationService m_notificationService;
	}
}
