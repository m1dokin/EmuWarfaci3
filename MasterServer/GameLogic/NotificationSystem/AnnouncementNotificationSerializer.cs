using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.DAL;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020003D3 RID: 979
	[NotificationSerializerAttributes(NotificationType = ENotificationType.Announcement)]
	internal class AnnouncementNotificationSerializer : INotificationSerializer
	{
		// Token: 0x0600157D RID: 5501 RVA: 0x0005A31C File Offset: 0x0005871C
		public XmlElement Serialize(byte[] data, XmlElement element)
		{
			Announcement typeFromByteArray = Utils.GetTypeFromByteArray<Announcement>(data);
			return typeFromByteArray.ToXml(element.OwnerDocument);
		}

		// Token: 0x0600157E RID: 5502 RVA: 0x0005A33C File Offset: 0x0005873C
		public object Deserialize(byte[] data)
		{
			return Utils.GetTypeFromByteArray<Announcement>(data);
		}
	}
}
