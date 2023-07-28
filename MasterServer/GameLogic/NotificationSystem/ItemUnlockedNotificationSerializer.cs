using System;
using System.Xml;
using MasterServer.Common;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020003DF RID: 991
	[NotificationSerializerAttributes(NotificationType = ENotificationType.ItemUnlocked)]
	internal class ItemUnlockedNotificationSerializer : INotificationSerializer
	{
		// Token: 0x0600159E RID: 5534 RVA: 0x0005A56C File Offset: 0x0005896C
		public XmlElement Serialize(byte[] data, XmlElement element)
		{
			ItemUnlockedNotification typeFromByteArray = Utils.GetTypeFromByteArray<ItemUnlockedNotification>(data);
			return typeFromByteArray.ToXml(element.OwnerDocument);
		}

		// Token: 0x0600159F RID: 5535 RVA: 0x0005A58C File Offset: 0x0005898C
		public object Deserialize(byte[] data)
		{
			return Utils.GetTypeFromByteArray<ItemUnlockedNotification>(data);
		}
	}
}
