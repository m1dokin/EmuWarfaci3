using System;
using System.Xml;
using MasterServer.Common;

namespace MasterServer.GameLogic.NotificationSystem.NotificationSerializers
{
	// Token: 0x020000B9 RID: 185
	[NotificationSerializerAttributes(NotificationType = ENotificationType.MissionUnlockMessage)]
	public class MissionUnlockNotificationSerializer : INotificationSerializer
	{
		// Token: 0x060002F6 RID: 758 RVA: 0x0000E0A4 File Offset: 0x0000C4A4
		public XmlElement Serialize(byte[] data, XmlElement element)
		{
			MissionUnlockNotification typeFromByteArray = Utils.GetTypeFromByteArray<MissionUnlockNotification>(data);
			return typeFromByteArray.ToXml(element.OwnerDocument);
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x0000E0C4 File Offset: 0x0000C4C4
		public object Deserialize(byte[] data)
		{
			return Utils.GetTypeFromByteArray<MissionUnlockNotification>(data);
		}
	}
}
