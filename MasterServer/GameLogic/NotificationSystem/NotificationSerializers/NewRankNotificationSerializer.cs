using System;
using System.Xml;
using MasterServer.Common;

namespace MasterServer.GameLogic.NotificationSystem.NotificationSerializers
{
	// Token: 0x020003E3 RID: 995
	[NotificationSerializerAttributes(NotificationType = ENotificationType.NewRankReached)]
	public class NewRankNotificationSerializer : INotificationSerializer
	{
		// Token: 0x060015AA RID: 5546 RVA: 0x0005A61C File Offset: 0x00058A1C
		public XmlElement Serialize(byte[] data, XmlElement element)
		{
			NewRankNotification typeFromByteArray = Utils.GetTypeFromByteArray<NewRankNotification>(data);
			return typeFromByteArray.ToXml(element.OwnerDocument);
		}

		// Token: 0x060015AB RID: 5547 RVA: 0x0005A63C File Offset: 0x00058A3C
		public object Deserialize(byte[] data)
		{
			return Utils.GetTypeFromByteArray<NewRankNotification>(data);
		}
	}
}
