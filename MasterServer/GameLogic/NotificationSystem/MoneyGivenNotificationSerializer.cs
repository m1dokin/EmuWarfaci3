using System;
using System.Xml;
using MasterServer.Common;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020003E2 RID: 994
	[NotificationSerializerAttributes(NotificationType = ENotificationType.MoneyGiven)]
	internal class MoneyGivenNotificationSerializer : INotificationSerializer
	{
		// Token: 0x060015A7 RID: 5543 RVA: 0x0005A5E0 File Offset: 0x000589E0
		public XmlElement Serialize(byte[] data, XmlElement element)
		{
			string typeFromByteArray = Utils.GetTypeFromByteArray<string>(data);
			element.OwnerDocument.LoadXml(typeFromByteArray);
			return element.OwnerDocument.DocumentElement;
		}

		// Token: 0x060015A8 RID: 5544 RVA: 0x0005A60B File Offset: 0x00058A0B
		public object Deserialize(byte[] data)
		{
			return Utils.GetTypeFromByteArray<string>(data);
		}
	}
}
