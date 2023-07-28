using System;
using System.Xml;
using MasterServer.Common;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020003DD RID: 989
	[NotificationSerializerAttributes(NotificationType = ENotificationType.ItemGiven)]
	internal class ItemGivenNotificationSerializer : INotificationSerializer
	{
		// Token: 0x06001598 RID: 5528 RVA: 0x0005A4F8 File Offset: 0x000588F8
		public XmlElement Serialize(byte[] data, XmlElement element)
		{
			string typeFromByteArray = Utils.GetTypeFromByteArray<string>(data);
			element.OwnerDocument.LoadXml(typeFromByteArray);
			return element.OwnerDocument.DocumentElement;
		}

		// Token: 0x06001599 RID: 5529 RVA: 0x0005A523 File Offset: 0x00058923
		public object Deserialize(byte[] data)
		{
			return Utils.GetTypeFromByteArray<string>(data);
		}
	}
}
