using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.GameLogic.ItemsSystem;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020003DE RID: 990
	[NotificationSerializerAttributes(NotificationType = ENotificationType.ItemUnequipped)]
	internal class ItemUnequippedNotificationSerializer : INotificationSerializer
	{
		// Token: 0x0600159B RID: 5531 RVA: 0x0005A534 File Offset: 0x00058934
		public XmlElement Serialize(byte[] data, XmlElement element)
		{
			return Utils.GetTypeFromByteArray<SItemUnequippedNotification>(data).ToXml(element.OwnerDocument);
		}

		// Token: 0x0600159C RID: 5532 RVA: 0x0005A555 File Offset: 0x00058955
		public object Deserialize(byte[] data)
		{
			return Utils.GetTypeFromByteArray<SItemUnequippedNotification>(data);
		}
	}
}
