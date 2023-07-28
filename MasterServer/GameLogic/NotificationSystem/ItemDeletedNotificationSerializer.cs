using System;
using System.Xml;
using MasterServer.Common;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020000B6 RID: 182
	[NotificationSerializerAttributes(NotificationType = ENotificationType.ItemDeleted)]
	internal class ItemDeletedNotificationSerializer : INotificationSerializer
	{
		// Token: 0x060002EE RID: 750 RVA: 0x0000DFD1 File Offset: 0x0000C3D1
		public object Deserialize(byte[] data)
		{
			return Utils.GetTypeFromByteArray<ItemDeletedNotification>(data);
		}

		// Token: 0x060002EF RID: 751 RVA: 0x0000DFDC File Offset: 0x0000C3DC
		public XmlElement Serialize(byte[] data, XmlElement element)
		{
			ItemDeletedNotification typeFromByteArray = Utils.GetTypeFromByteArray<ItemDeletedNotification>(data);
			return typeFromByteArray.ToXml(element.OwnerDocument);
		}
	}
}
